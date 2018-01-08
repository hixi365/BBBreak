using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ゲームオブジェクト
//  ブロックの破壊処理

public class DestroyBlock : MonoBehaviour {

	[SerializeField]
	private LayerMask layerBroken;	// 破壊済ブロックレイヤータグ
	private int layerValueBroken;	// 破壊済ブロックレイヤー (書き込み用)

	[SerializeField]
	private float rateDrop = 1f;    // アイテムドロップ率 
	[SerializeField]
	private GameObject prefabDrop;	// ドロップアイテム

	private void Start()
	{

		// 破壊済みブロックのレイヤ値の計算
		layerValueBroken = 0;
		int l = layerBroken;
		while (0 < (l >>= 1))
		{
			layerValueBroken++;
		}

	}

	private void Update ()
	{

		if(gameObject.layer == layerValueBroken)
		{

			if (prefabDrop != null)
			{
				if (Random.Range(0f, 1f) <= rateDrop)
				{
					Instantiate(prefabDrop, transform.position, Quaternion.identity);
				}
			}

			Destroy(gameObject);
			return;

		}

	}

}
