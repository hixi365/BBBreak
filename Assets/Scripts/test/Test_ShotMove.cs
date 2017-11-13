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

	public GameObject prefabItem;

	public float degMaxRefrect = 45;    // プレイヤーの端部に当たった最大反射角 (45 : 10.5時 ～ 1.5時)

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

		float circle_r = objCircle.transform.lossyScale.x * 2 / (40 / 2); // 40 : sprite unit scale

		Vector2 vec_n = (Vector2)Vector3.Normalize(vecMove);	// 進む方向
		Vector2 pos = (Vector2)posCurrent;						// 座標

		float vec_power = Vector3.Distance(Vector2.zero, vecMove) * Time.deltaTime; // 進む距離
		Vector2 new_vec_n = vec_n;													// 計算用

		int ray_try = 3; // 壁面反射計算回数 (最低 2)
		for (int i = 0; i < ray_try; i++)
		{

			// 円のレイキャスト
			Vector2 c_offset_c = circle_r * vec_n;								// 移動方向分の弾の大きさ分オフセット
			Vector2 c_offset_0 = new Vector2(-c_offset_c.y, +c_offset_c.x);		// 移動方向法線方向分の弾の大きさ分オフセット
			Vector2 c_offset_1 = -c_offset_0;                                   // 移動方向法線方向分の弾の大きさ分オフセット
			RaycastHit2D c_circle_c = Physics2D.Raycast(pos + c_offset_c, vec_n, vec_power, layerRefrect + layerPlayer + layerFailed + layerBlock);
			RaycastHit2D c_circle_0 = Physics2D.Raycast(pos + c_offset_0, vec_n, vec_power, layerRefrect + layerPlayer + layerFailed + layerBlock);
			RaycastHit2D c_circle_1 = Physics2D.Raycast(pos + c_offset_1, vec_n, vec_power, layerRefrect + layerPlayer + layerFailed + layerBlock);

			Debug.DrawRay((Vector3)(pos + c_offset_c) + new Vector3(0, 0, 1), vec_n * vec_power, new Color(1f, 0f, 0f));
			Debug.DrawRay((Vector3)(pos + c_offset_0) + new Vector3(0, 0, 1), vec_n * vec_power, new Color(1f, 0f, 0f));
			Debug.DrawRay((Vector3)(pos + c_offset_1) + new Vector3(0, 0, 1), vec_n * vec_power, new Color(1f, 0f, 0f));

			// 衝突
			Vector2 c_offset = c_offset_c;	// 衝突座標オフセット (ずらす)
			RaycastHit2D c = c_circle_c;    // 衝突情報
			if (c.collider == null) { c = c_circle_0; c_offset = c_offset_0; }
			if (c.collider == null) { c = c_circle_1; c_offset = c_offset_1; }

			// 衝突がなければ
			if (c.collider == null)
			{
				break;
			}

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

				Debug.Log("player collide" + p);

				// x成分から反射角度の計算
				float abs_x = Mathf.Sin(degMaxRefrect);
				float new_x = abs_x * p;
				float angle = Mathf.Asin(new_x);

				//	// 射出角度の計算
				//	float angle = p * degMaxRefrect * Mathf.Deg2Rad;

				new_vec_n = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));

			}

			// 通常の反射面であれば反射
			else
			{

				new_vec_n = vec_n - 2 * c.normal * Vector2.Dot(vec_n, c.normal);

			}

			if(c_circle_c.collider)
				pos = c.point - vec_n * circle_r;
			else
				pos = c.point - c_offset - vec_n * circle_r;

			vec_n = new_vec_n;
			vec_power -= dis;

		}

		vecMove = Vector3.Distance(Vector3.zero, vecMove) * Vector3.Normalize(vec_n);
		posCurrent = pos + vec_n * vec_power;
		transform.position = posCurrent;

	}

}
