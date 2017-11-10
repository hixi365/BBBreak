using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// テスト
// ボールの動き

public class Test_BallMove : MonoBehaviour {

	public Vector3 vecMove;			// 弾の移動ベクトル
	private Vector3 posCurrent;     // 弾の座標 transformへ書き込む

	public LayerMask layerRefrect;	// 反射するレイヤータグ
	public LayerMask layerPlayer;	// プレイヤーレイヤータグ
	public LayerMask layerFailed;	// ミスするレイヤータグ
	public LayerMask layerBlock;    // ブロックレイヤータグ

	public GameObject prefabItem;

	public float degMaxRefrect = 45;	// プレイヤーの端部に当たった最大反射角 (45 : 10.5時 ～ 1.5時)

	private void Start ()
	{

		posCurrent = transform.position;

		float angle = Random.Range(0, Mathf.PI * 2);
		Vector3 speed = 1.0f * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
		vecMove = speed;

	}

	private void Update()
	{

		Vector2 vec = (Vector2)vecMove * Time.deltaTime;
		Vector2 pos = (Vector2)posCurrent;

		Vector2 new_vec = vec;

		int ray_try = 3; // 壁面反射計算回数 (低速であれば2か1)
		for (int i = 0; i < ray_try; i++)
		{

			// 移動方向へレイキャスト (弾の大きさは考慮していない)
			RaycastHit2D c = Physics2D.Raycast(pos, vec, Vector2.Distance(Vector2.zero, vec), layerRefrect + layerPlayer + layerFailed + layerBlock);

			// 衝突がなければ
			if (c.collider == null)
			{
				break;
			}
			
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

				Debug.Log("p" + p);

				// x成分から反射角度の計算
				float abs_x = Mathf.Sin(degMaxRefrect);
				float new_x = abs_x * p;
				float angle = Mathf.Asin(new_x);

			//	// 射出角度の計算
			//	float angle = p * degMaxRefrect * Mathf.Deg2Rad;

				new_vec = new Vector2( Mathf.Sin(angle), Mathf.Cos(angle) ) * Vector3.Distance(Vector3.zero, (Vector3)vec); 

			}

			// 通常の反射面であれば反射し、再度判定
			else
			{

				new_vec = vec - 2 * c.normal * Vector2.Dot(vec, c.normal);

			}

			pos = c.point - (Vector2)Vector3.Normalize(vec) * (transform.localScale.x) / 128;
			vec = new_vec;
			vec *= (1 - c.fraction);

		}

		vecMove = Vector3.Distance(Vector3.zero, vecMove) * Vector3.Normalize(vec);
		posCurrent = pos + vec;
		transform.position = posCurrent;

	}	

}
