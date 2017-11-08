using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 一定時間後にゲームオブジェクトを削除する

public class DestroyLifeTime : MonoBehaviour {

	public float lifeTime = 1f;
	private float timeTotal = 0f;
	
	private void Update ()
	{

		timeTotal += Time.deltaTime;
		if (timeTotal >= lifeTime)
			Destroy(gameObject);
	}

}
