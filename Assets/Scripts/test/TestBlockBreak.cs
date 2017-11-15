using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// テスト
// 壊れるブロック処理

public class TestBlockBreak : MonoBehaviour {

	public LayerMask layerBlock;    // ブロックレイヤータグ
	public LayerMask layerBroken;   // 破壊済ブロックレイヤータグ

	private void Update ()
	{
	
		if (1 << gameObject.layer == layerBroken.value)
		{

			Destroy(gameObject);

		}
		
	}

}
