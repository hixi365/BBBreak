using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 汎用
//  一定時間後にオブジェクトを有効にする
public class AppearObject : MonoBehaviour {

	[SerializeField]
	private GameObject obj;

	[SerializeField]
	private float delay = 1f;

	private float timeTotal = 0f;

	private void Awake()
	{

		if(obj == null)
		{

			return;

		}

		obj.SetActive(false);

	}

	private void Update()
	{

		timeTotal += Time.deltaTime;

		if(timeTotal > delay)
		{

			obj.SetActive(true);
			Destroy(this);

		}

	}

}
