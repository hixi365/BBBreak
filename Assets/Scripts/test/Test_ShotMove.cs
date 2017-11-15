using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// テスト
// 汎用 弾の軌道

public class Test_ShotMove : MonoBehaviour {

	public Vector3 vecMove;         // 弾の移動ベクトル
	private Vector3 posCurrent;     // 弾の座標 transformへ書き込む

	public float angleRoll;			// z軸回転速度
	public float angleCurrent;      // z軸回転 transformへ書き込む

	public GameObject objCircle;	// 中央の円
	public GameObject objBar;		// 棒

	public LayerMask layerRefrect;  // 反射するレイヤータグ
	public LayerMask layerPlayer;   // プレイヤーレイヤータグ
	public LayerMask layerFailed;   // ミスするレイヤータグ
	public LayerMask layerBlock;    // ブロックレイヤータグ

	public bool isBarMode = false;  // モード : 回転する棒

	public float ySpriteAlpha1 = 0f;		// スプレイトアニメーション α=1 y座標
	public float ySpriteAlpha0 = -0.5f;		// スプライトアニメーション α=0 y座標

	public GameObject prefabItem;

	public float degMaxRefrect = 45;    // プレイヤーの端部に当たった最大反射角 (45 : 10.5時 ～ 1.5時)

	private SpriteRenderer rendererBar; // 棒のスプライトレンダラ

	private void Awake()
	{

		rendererBar = objBar.GetComponent<SpriteRenderer>();

	}

	private void Start()
	{

		posCurrent = transform.localPosition;
		angleCurrent = transform.localRotation.eulerAngles.z;

		float angle = -Mathf.PI / 2; // Random.Range(0, Mathf.PI * 2);
		Vector3 speed = 1.0f * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
		vecMove = speed;

		angleRoll = 0f;

	}

	private void Update()
	{

		Move();
		UpdateSprite();

	}

	// 移動
	private void Move()
	{

		float circle_r = objCircle.transform.lossyScale.x * 2 / (40 / 2); // 40 : sprite unit scale

		Vector2 vec_n = (Vector2)Vector3.Normalize(vecMove);    // 進む方向
		Vector2 pos = (Vector2)posCurrent;                      // 座標

		float vec_power = Vector3.Distance(Vector2.zero, vecMove) * Time.deltaTime; // 進む距離
		Vector2 new_vec_n = vec_n;                                                  // 計算用

		int ray_try = 3; // 壁面反射計算回数 (最低 2)
		for (int i_ray = 0; i_ray < ray_try; i_ray++)
		{

			// 円のレイキャスト
			Vector2[] offset = new Vector2[5];	// 0度[0] 45度[1, 2] 90度[3, 4]
			offset[0] = circle_r * vec_n;							// raycast offset (正面)
			offset[3] = new Vector2(-offset[0].y, +offset[0].x);    // raycast offset (90度)
			offset[4] = -offset[3];									// raycast offset (90度)
			offset[1] = (offset[0] + offset[3]) / 2;				// raycast offset (45度)
			offset[2] = (offset[0] + offset[4]) / 2;                // raycast offset (45度)

			// ray 確認
			Debug.DrawRay((Vector3)(pos + offset[0]) + new Vector3(0, 0, 1), vec_n * vec_power, new Color(1f, 0f, 0f));
			Debug.DrawRay((Vector3)(pos + offset[1]) + new Vector3(0, 0, 1), vec_n * vec_power, new Color(1f, 0f, 0f));
			Debug.DrawRay((Vector3)(pos + offset[2]) + new Vector3(0, 0, 1), vec_n * vec_power, new Color(1f, 0f, 0f));
			Debug.DrawRay((Vector3)(pos + offset[3]) + new Vector3(0, 0, 1), vec_n * vec_power, new Color(1f, 0f, 0f));
			Debug.DrawRay((Vector3)(pos + offset[4]) + new Vector3(0, 0, 1), vec_n * vec_power, new Color(1f, 0f, 0f));

			RaycastHit2D[] c_hit = new RaycastHit2D[5];
			for(int i = 0; i < 5; i++)
				c_hit[i] = Physics2D.Raycast(pos + offset[i], vec_n, vec_power, layerRefrect + layerPlayer + layerFailed + layerBlock);

			// 衝突
			int c_index = -1;
			for (int i = 0; i < 5; i++) {
				if (c_hit[i].collider && c_hit[i].fraction > 0f)
				{
					c_index = i;
					break;
				}
			}

			// 衝突がなければ
			if (c_index == -1)
			{
				break;
			}

			// 計算に用いる衝突情報
			RaycastHit2D c = c_hit[c_index];

			// 進んだ距離の計算
			float dis = c.fraction * vec_power;

			// ミス扱い
			if (1 << c.collider.gameObject.layer == layerFailed.value)
			{
				Destroy(gameObject);
				return;
			}

			// ブロックであれば破壊
			if (1 << c.collider.gameObject.layer == layerBlock.value)
			{

				// 一定確率でアイテム (テスト)
				if (Random.Range(0, 10) < 2f)
				{
					Instantiate(prefabItem, c.transform.position, Quaternion.identity);
				}

				Destroy(c.transform.gameObject);

			}

			// プレイヤーであれば角度を当たった位置で変更 (側面は端に当たったとして前方に飛ばす)
			if (1 << c.collider.gameObject.layer == layerPlayer.value)
			{

				// プレイヤー座標
				Transform tPlayer = c.collider.transform;
				Transform tPlayerParent = tPlayer.parent;
				float player_w = tPlayer.localScale.x;
				float x_left = tPlayerParent.localPosition.x - player_w;
				float x_right = tPlayerParent.localPosition.x + player_w;

				// プレイヤーのどこに当たったか  ( -1 < p < +1 )
				float p = (c.point.x - x_left) / (x_right - x_left);
				p = (p - 0.5f) * 2f;

			//	Debug.Log("player collide" + p);

				// x成分から反射角度の計算
				float abs_x = Mathf.Sin(degMaxRefrect);
				float new_x = abs_x * p;
				float angle = Mathf.Asin(new_x);

				//	// 射出角度の計算
				//	float angle = p * degMaxRefrect * Mathf.Deg2Rad;

				new_vec_n = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
				angleRoll = -angle * 1000f;

			}

			// 通常の反射面であれば反射
			else
			{

				new_vec_n = vec_n - 2 * c.normal * Vector2.Dot(vec_n, c.normal);
				angleRoll = -angleRoll;

			}

		//	if (c_circle_c.collider)
		//		pos = c.point - vec_n * circle_r;
		//	else
		//		pos = c.point - c_offset - vec_n * circle_r;

			pos = c.point - offset[c_index];

			vec_n = new_vec_n;
			vec_power -= dis;

		}

		vecMove = Vector3.Distance(Vector3.zero, vecMove) * Vector3.Normalize(vec_n);
		posCurrent = pos + vec_n * vec_power;

		angleCurrent += angleRoll * Time.deltaTime;

		transform.position = posCurrent;
		transform.rotation = Quaternion.Euler(0f, 0f, angleCurrent);

	}

	// 描画関連
	private void UpdateSprite()
	{

		Color col = rendererBar.color;
		
		if (isBarMode == false)
		{
			col.a = 0f;
		}

		else
		{

			if (vecMove.y > 0)
				col.a = 1f;

			else
			{
				col.a = Mathf.Clamp((posCurrent.y - ySpriteAlpha0) / (ySpriteAlpha1 - ySpriteAlpha0), 0f, 1f);
			}

		}

		rendererBar.color = col;

	}

}
