using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_BallMove : MonoBehaviour {

	public Vector3 vecMove;			// 弾の移動ベクトル
	private Vector3 posCurrent;     // 弾の座標 transformへ書き込む

	public LayerMask layerRefrect;	// 反射するレイヤータグ
	public LayerMask layerFailed;	// ミスするレイヤータグ

	private void Start ()
	{

		posCurrent = transform.position;

		float angle = Random.Range(0, Mathf.PI * 2);
		Vector3 speed = 5.0f * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
		vecMove = speed;

	}

	private void Update()
	{

		Vector2 vec = (Vector2)vecMove * Time.deltaTime;
		Vector2 pos = (Vector2)posCurrent;

		int ray_try = 3; // 壁面反射計算回数 (低速であれば2か1)
		for (int i = 0; i < ray_try; i++)
		{

			// 移動方向へレイキャスト (弾の大きさは考慮していない)
			RaycastHit2D c = Physics2D.Raycast(pos, vec, Vector2.Distance(Vector2.zero, vec), layerRefrect + layerFailed);

			Debug.DrawLine(pos, pos + vec * 5);

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

			// 反射面であれば反射し、再度判定
			pos = c.point - (Vector2)Vector3.Normalize(vec) * (transform.localScale.x) / 128;
			vec = vec - 2 * c.normal * Vector2.Dot(vec, c.normal);
			vec *= (1 - c.fraction);

		}

		vecMove = Vector3.Distance(Vector3.zero, vecMove) * Vector3.Normalize(vec);
		posCurrent = pos + vec;
		transform.position = posCurrent;

	}	

}
