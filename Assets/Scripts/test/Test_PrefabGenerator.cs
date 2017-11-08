using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// テスト
// 任意のPrefabを生成

public class Test_PrefabGenerator : MonoBehaviour {

	public GameObject prefab;
	public float interval = 1f;

	private float time = 0f;
	
	private void Update ()
	{

		time += Time.deltaTime;

		while(time > interval)
		{
			Instantiate(prefab, transform.position, transform.rotation, transform);
			time -= interval;
		}
			
	}

}
