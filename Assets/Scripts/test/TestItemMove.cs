using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestItemMove : MonoBehaviour {

	public Vector3 vecMove;         // 弾の移動ベクトル
	private Vector3 posCurrent;     // 弾の座標 transformへ書き込む

	public float angleRoll;         // z軸回転速度
	public float angleCurrent;      // z軸回転 transformへ書き込む

	public LayerMask layerRefrect;  // 反射するレイヤータグ
	public LayerMask layerPlayer;   // プレイヤーレイヤータグ
	public LayerMask layerFailed;   // ミスするレイヤータグ (弾が消える)
	public LayerMask layerBlock;    // ブロックレイヤータグ
	public LayerMask layerBroken;   // 破壊済ブロックレイヤータグ

	public float alphaMax = 1f;         // アニメーション α値 最大
	public float alphaMin = 0f;         // アニメーション α値 最小

	public float intervalBlink = 1f;    // スプライト点滅間隔
	private float time = 0f;            // 累積時間

	public float delay = 0f;            // ディレイ (別スクリプトにするべき)

	private SpriteRenderer renderer;

	private void Awake()
	{

		renderer = GetComponent<SpriteRenderer>();

	}

	private void Start()
	{

		posCurrent = transform.localPosition;
		angleCurrent = transform.localRotation.eulerAngles.z;

		//	float angle = -Mathf.PI / 2; // Random.Range(0, Mathf.PI * 2);
		//	Vector3 speed = 1.0f * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
		//	vecMove = speed;

		angleRoll = 0f;

		// delay
		if (delay > 0f)
		{
			GetComponent<Renderer>().enabled = false;
		}

	}

	private void Update()
	{

		// delay
		if (delay > 0f)
		{
			delay -= Time.deltaTime;
			return;
		}
		GetComponent<Renderer>().enabled = true;

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
		
		int layerMask = layerPlayer + layerFailed;

		RaycastHit2D hit = Physics2D.Raycast(pos, vec_n, vec_power, layerMask);
		if (hit.collider)
		{

			// ミス扱い
			if (1 << hit.collider.gameObject.layer == layerFailed.value)
			{
				Destroy(gameObject);
				return;
			}

			// プレイヤー
			else if (1 << hit.collider.gameObject.layer == layerPlayer.value)
			{
				Destroy(gameObject);
				return;
			}

		}

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

}
