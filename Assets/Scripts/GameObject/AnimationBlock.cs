using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ゲームオブジェクト
//  ブロック生成時のアニメーション

public class AnimationBlock : MonoBehaviour {

	[SerializeField]
	private float delay = 0f;		// 遅延時間
	[SerializeField]
	private float timeMove = 1f;	// 移動に掛かる時間

	[SerializeField]
	private Vector3 fromPos;		// 開始座標
	[SerializeField]
	private Vector3 fromRot;		// 開始回転

	[SerializeField]
	private Vector3 toPos;			// 終了座標
	[SerializeField]
	private Vector3 toRot;			// 終了回転

	[SerializeField]
	private float fromAlpha = 0f;	// 開始α値
	[SerializeField]
	private float toAlpha = 1f;		// 終了α値

	private float time = 0f;		// 時間

	private SpriteRenderer renderer;

	private void Start()
	{

		RandomAppear();

		BoxCollider2D c = GetComponent<BoxCollider2D>();
		c.enabled = false;

		transform.localPosition = fromPos;
		transform.localRotation = Quaternion.Euler(fromRot);

		renderer = GetComponent<SpriteRenderer>();
		if (renderer)
		{
			Color col = renderer.color;
			col.a = fromAlpha;
			renderer.color = col;
		}

		if (delay > 0f)
		{
			Color col = renderer.color;
			col.a = 0f;
			renderer.color = col;
		}

	}

	private void Update()
	{

		if (delay > 0f)
		{
			delay -= Time.deltaTime;
			return;
		}

		time += Time.deltaTime;

		if (time < timeMove)
		{

			float a = time / timeMove;
			Vector3 p = Vector3.Lerp(fromPos, toPos, a);
			Quaternion q = Quaternion.Lerp(Quaternion.Euler(fromRot), Quaternion.Euler(toRot), a);

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
		if (c)
			c.enabled = true;

		Destroy(this);

	}

	private void RandomAppear()
	{

		float a = Random.Range(0, 2 * Mathf.PI);
		float rad = 1.0f;
		float roll = Random.Range(-Mathf.PI, +Mathf.PI) * 5f * Mathf.Rad2Deg;

		Vector3 offsetPos = new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0f) * rad;
		Vector3 offsetRot = new Vector3(0f, 0f, roll);

		fromPos = transform.localPosition - offsetPos;
		fromRot = transform.localRotation.eulerAngles - offsetRot;

		toPos = transform.localPosition;
		toRot = transform.localRotation.eulerAngles;

	}

}
