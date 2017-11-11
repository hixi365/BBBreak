using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// テスト
// ブロック発生

public class TestAppearBlock : MonoBehaviour {


	public float timeMove = 1f; // 移動に掛かる時間

	public Vector3 fromPos;		// 開始座標
	public Vector3 fromRot;		// 開始回転

	public Vector3 toPos;		// 終了座標
	public Vector3 toRot;       // 終了回転

	public float fromAlpha = 0f;	// 開始α値
	public float toAlpha = 1f;		// 終了α値

	private float time = 0f;    // 時間

	private SpriteRenderer renderer;

	private void Start()
	{

		BoxCollider2D c = GetComponent<BoxCollider2D>();
		c.enabled = false;

		transform.localPosition = fromPos;
		transform.localRotation = Quaternion.Euler(fromRot);

		renderer = GetComponent<SpriteRenderer>();
		if (renderer) {
			Color col = renderer.color;
			col.a = fromAlpha;
			renderer.color = col;
		}

	}

	private void Update()
	{

		time += Time.deltaTime;

		if(time < timeMove)
		{

			float a = time / timeMove;
			Vector3 p = Vector3.Lerp(fromPos, toPos, a);
			Quaternion q = Quaternion.Lerp(Quaternion.Euler(fromPos), Quaternion.Euler(toPos), a);

			transform.localPosition = p;
			transform.localRotation = q;

			if (renderer)
			{
				Color col = renderer.color;
				col.a = Mathf.Lerp(fromAlpha, toAlpha, a);
				renderer.color = col;
			}

			return;

		}

		transform.localPosition = toPos;
		transform.localRotation = Quaternion.Euler(toRot);

		if (renderer)
		{
			Color col = renderer.color;
			col.a = toAlpha;
			renderer.color = col;
		}

		BoxCollider2D c = GetComponent<BoxCollider2D>();
		if(c)
			c.enabled = true;

		Destroy(this);

	}

}
