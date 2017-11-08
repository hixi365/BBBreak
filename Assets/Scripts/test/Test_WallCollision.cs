using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// テスト
// 壁に侵入した際の跳ね返り

public class Test_WallCollision : MonoBehaviour {

	public bool isWall = true;      // falseの時、当たってはいけない壁
	public bool isVertical = true;  // 垂直な壁か (x座標で跳ね返す)
	public bool isPlus = false;		// 正の方向に返すか (x座標でfalse : 右壁)	

	private void Update ()
	{
		
	}

	private void OnTriggerEnter2D(Collider2D other)
	{

		Test_BallMove t = other.gameObject.GetComponent<Test_BallMove>();
		if (t == null)
			return;

		Vector3 pos_b = t.transform.position;
		if (isVertical)
		{
			if (isPlus)
			{
				pos_b.x = (pos_b.x - transform.position.x) - (transform.localScale.x + other.transform.localScale.x) + (transform.position.x + transform.localScale.x);
			}
			else
			{
				pos_b.x = (transform.position.x - pos_b.x) - (transform.localScale.x + other.transform.localScale.x) + (transform.position.x - transform.localScale.x);
			}

			t.vecMove.x = -t.vecMove.x;
		}
		else
		{
			if (isPlus)
			{
				pos_b.y = (pos_b.y - transform.position.y) - (transform.localScale.y + other.transform.localScale.y) + (transform.position.y + transform.localScale.y);
			}
			else
			{
				pos_b.y = (transform.position.y - pos_b.y) - (transform.localScale.y + other.transform.localScale.y) + (transform.position.y - transform.localScale.y);
			}

			t.vecMove.y = -t.vecMove.y;
		}

		t.transform.position = pos_b;

	}

}
