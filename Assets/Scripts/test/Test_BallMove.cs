using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_BallMove : MonoBehaviour {

	public Vector3 vecMove;			// 弾の移動ベクトル
	private Vector3 posCurrent;		// 弾の座標 transformへ書き込む

	private void Start ()
	{

		posCurrent = transform.position;

		float angle = Random.Range(0, Mathf.PI * 2);
		Vector3 speed = 10.0f * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
		vecMove = speed;

	}

	private void Update()
	{

		posCurrent += vecMove * Time.deltaTime;
		transform.position = posCurrent;

	}

	

}
