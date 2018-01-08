using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator01 : MonoBehaviour {

	[SerializeField]
	private GameObject prefabNomalBlock;   // 基本的なブロック

	[SerializeField]
	private float waitGenerate = 0.5f;      // 初回生成への遅延
	[SerializeField]
	private float intervalGenerate = 0.1f;	// 生成のインターバル

	private void Start ()
	{

		StartCoroutine(GenerateBlock());
			
	}

	// nullチェックを行い、オブジェクトを生成
	private void SetObject(GameObject prefab, Vector3 pos, float degree)
	{

		if(prefab == null)
		{

			return;

		}

		Instantiate(prefab, pos, Quaternion.Euler(0f, 0f, degree), transform);

	}

	// ステージ生成
	IEnumerator GenerateBlock()
	{

		yield return new WaitForSeconds(waitGenerate);

		for(int y = -3; y <= 3; y++)
		{

			for(int x = -3; x <= 3; x++)
			{

				Vector3 pos = transform.position + new Vector3(x, -y, 0f) * prefabNomalBlock.transform.lossyScale.x;
				SetObject(prefabNomalBlock, pos, 0);

			}

			yield return new WaitForSeconds(intervalGenerate);

		}
	}

}
