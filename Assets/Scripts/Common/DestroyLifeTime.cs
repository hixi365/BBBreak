
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 汎用
//  一定時間後にゲームオブジェクトを削除する

public class DestroyLifeTime : MonoBehaviour {

	[SerializeField]
	private float lifeTime = 1f;
	private float timeTotal = 0f;
	
	public void SetLifeTime(float seconds)
	{

		lifeTime = seconds;

	}

	private void Update ()
	{

		timeTotal += Time.deltaTime;

		if (timeTotal >= lifeTime)
		{
			Destroy(gameObject);
		}

	}

}
