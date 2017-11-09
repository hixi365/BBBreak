using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// テスト
// ブロックの配置

public class TestBlockGanarator : MonoBehaviour {

	public GameObject prefab;
	public float blockScaleW = 0.1f;
	public float blockScaleH = 0.1f;
	public float intervalW = 0.01f;
	public float intervalH = 0.01f;

	public int numW = 3;
	public int numH = 3;

	private void Start()
	{

		for(int y = -numH; y <= numH; y++)
		{
			for(int x = -numW; x <= numW; x++)
			{
				GameObject obj = Instantiate(prefab, transform.position, transform.rotation, transform);
				obj.transform.position = new Vector3(x * (blockScaleW + intervalW), y * (blockScaleH + intervalH), 0f) + transform.position;
				obj.transform.localScale = new Vector3(blockScaleW, blockScaleH, 1f);
			}
		}


	}

}
