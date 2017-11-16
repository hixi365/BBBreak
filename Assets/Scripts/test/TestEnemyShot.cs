using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyShot : MonoBehaviour {

	public Vector3 vecMove;         // 弾の移動ベクトル
	private Vector3 posCurrent;     // 弾の座標 transformへ書き込む

	public float angleRoll;         // z軸回転速度
	public float angleCurrent;      // z軸回転 transformへ書き込む

	public LayerMask layerRefrect;  // 反射するレイヤータグ
	public LayerMask layerPlayer;   // プレイヤーレイヤータグ
	public LayerMask layerFailed;   // ミスするレイヤータグ (弾が消える)
	public LayerMask layerBlock;    // ブロックレイヤータグ
	public LayerMask layerBroken;   // 破壊済ブロックレイヤータグ

	public float degMaxRefrect = 45;    // プレイヤーの端部に当たった最大反射角 (45 : 10.5時 ～ 1.5時)

	public bool isPlayer;               // プレイヤーが跳ね返したか
	public int countRefrect = 0;        // 反射回数
	public int maxRefrect = 3;			// 最大反射回数

	private int layerSetBroken;         // 破壊済ブロックレイヤー (書き込み用)
	private SpriteRenderer renderer;    // 棒のスプライトレンダラ

	public float alphaMax = 1f;			// アニメーション α値 最大
	public float alphaMin = 0f;			// アニメーション α値 最小

	public float intervalBlink = 1f;    // スプライト点滅間隔
	private float time = 0f;			// 累積時間

	private void Awake()
	{

		renderer = GetComponent<SpriteRenderer>();

		layerSetBroken = 0;
		int l = layerBroken;
		while (0 < (l >>= 1))
			layerSetBroken++;

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

		time += Time.deltaTime;

		Move();
		UpdateSprite();

	}

	// 移動
	private void Move()
	{

		float circle_r = transform.lossyScale.x * 2 / (40 / 2); // 40 : sprite unit scale

		Vector2 vec_n = (Vector2)Vector3.Normalize(vecMove);    // 進む方向
		Vector2 pos = (Vector2)posCurrent;                      // 座標

		float vec_power = Vector3.Distance(Vector2.zero, vecMove) * Time.deltaTime; // 進む距離
		Vector2 new_vec_n = vec_n;                                                  // 計算用

		int layerMask = layerRefrect + layerPlayer + layerFailed;	// 敵弾である間はブロックとは判定を取らない
		if (isPlayer) layerMask += layerBlock;						// 自身の弾になればブロックと判定を取る

		int ray_try = 3; // 壁面反射計算回数 (最低 2)
		for (int i_ray = 0; i_ray < ray_try; i_ray++)
		{

			// 円のレイキャスト
			Vector2[] offset = new Vector2[5];  // 0度[0] 45度[1, 2] 90度[3, 4]
			offset[0] = circle_r * vec_n;                           // raycast offset (正面)
			offset[3] = new Vector2(-offset[0].y, +offset[0].x);    // raycast offset (90度)
			offset[4] = -offset[3];                                 // raycast offset (90度)
			offset[1] = (offset[0] + offset[3]) / 2;                // raycast offset (45度)
			offset[2] = (offset[0] + offset[4]) / 2;                // raycast offset (45度)

			RaycastHit2D[] c_hit = new RaycastHit2D[5];
			for (int i = 0; i < 5; i++)
				c_hit[i] = Physics2D.Raycast(pos + offset[i], vec_n, vec_power, layerMask);

			// 衝突
			int c_index = -1;
			for (int i = 0; i < 5; i++)
			{
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

				BrokenBlock(c.collider.gameObject);

				// 当たった時点で破壊
				Destroy(gameObject);
				return;

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

				isPlayer = true;

			}

			// 通常の反射面であれば反射
			else
			{

				new_vec_n = vec_n - 2 * c.normal * Vector2.Dot(vec_n, c.normal);
				angleRoll = -angleRoll;

			}

			pos = c.point - offset[c_index];

			vec_n = new_vec_n;
			vec_power -= dis;

			countRefrect++;
			if(maxRefrect == countRefrect)
			{
				Destroy(gameObject);
				return;
			}

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

		if (renderer == null)
			return;

		Color col = renderer.color;

		col.a = Mathf.Lerp(alphaMin, alphaMax, Mathf.Abs(Mathf.Cos(time / intervalBlink)));

		renderer.color = col;

	}

	// ブロックの破壊
	private void BrokenBlock(GameObject block)
	{

		block.layer = layerSetBroken;

	}

}
