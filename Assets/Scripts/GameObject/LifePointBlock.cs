using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ゲームオブジェクト
//  下部ライフ代替ブロック

public class LifePointBlock : MonoBehaviour {

	[SerializeField]
	private Color[] colors;

	private SpriteRenderer renderer;
	private int countHit = 0;
	private int allowCountHit = -1;

	[SerializeField]
	private float timeAppear = 0.3f;    // 初回生成時アニメーション時間 fadein
	[SerializeField]
	private float timeColor = 0.3f;     // 初回生成時アニメーション時間 color

	[SerializeField]
	private float scaleAppear = 3f;     // フェードイン leap localscale
	private float scaleBaseX = 1f;		// scale 初回保存用
	private float scaleBaseY = 1f;		// scale 初回保存用

	private float timeTotal = 0f;		// 制御用時間

	private void Awake()
	{

		// 色の数をライフとして取得
		allowCountHit = colors.Length;

		// spriteの取得
		renderer = GetComponent<SpriteRenderer>();

		// scaleの保存
		scaleBaseX = transform.localScale.x;
		scaleBaseY = transform.localScale.y;

	}

	private void Start()
	{

		if (renderer == null)
		{

			Debug.Log("LifePointBlock : SpriteRenderer is null");
			return;

		}

		if (colors.Length == 0)
		{

			Debug.Log("LifePointBlock : color is zero");
			return;

		}

		StartCoroutine(AppearBlock());

	}

	// 生成アニメーション
	IEnumerator AppearBlock()
	{

		// 最終色を指定
		Color colorCurrent = colors[colors.Length - 1];

		while (timeTotal < timeAppear)
		{

			float alpha = Mathf.Lerp(0f, 1f, timeTotal / timeAppear);
			colorCurrent.a = alpha;
			renderer.color = colorCurrent;

			float scale = Mathf.Lerp(scaleAppear, 1f, timeTotal / timeAppear);
			transform.localScale = new Vector3(scaleBaseX, scaleBaseY, 1f) * scale;

			timeTotal += Time.deltaTime;
			yield return null;

		}

		colorCurrent = colors[0];
		transform.localScale = new Vector3(scaleBaseX, scaleBaseY, 1f);

		while (timeTotal < timeAppear + timeColor)
		{

			int phase = (int)((colors.Length - 1) * (1f - (timeTotal - timeAppear) / timeColor));
			colorCurrent = colors[phase];
			renderer.color = colorCurrent;

			timeTotal += Time.deltaTime;
			yield return null;

		}

	}

	// 接触回数の判定
	private bool IsDead()
	{

		if (countHit >= allowCountHit)
			return true;

		return false;

	}

	// 色の更新
	private void UpdateColor()
	{

		// 初期色として、透明(白)
		Color color = new Color(1f, 1f, 1f, 0f);

		// 色配列の範囲内であれば、色の更新
		if (colors.Length > countHit)
		{
			color = colors[countHit];
		}

		// 更新
		if(renderer != null)
		{

			renderer.color = color;

		}

	}

	// 接触処理
	public void OnHit()
	{

		countHit++;
		
		// 破壊時
		if(IsDead() == true)
		{

			Destroy(gameObject);
			return;

		}

		UpdateColor();

	}

}
