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

	
	private void Awake()
	{

		// 色の数をライフとして取得
		allowCountHit = colors.Length;

		// spriteの取得
		renderer = GetComponent<SpriteRenderer>();

	}

	private void Start()
	{

		UpdateColor();

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
