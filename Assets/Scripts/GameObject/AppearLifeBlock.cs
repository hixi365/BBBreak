using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ゲームオブジェクト
//  ライフブロックの生成アニメーション

public class AppearLifeBlock : MonoBehaviour {

	[SerializeField]
	private GameObject objLifeBlockParent;

	private List<GameObject> listLifeBlock;

	[SerializeField]
	private float delayAppearInterval = 0.1f;	// 1個当たりの生成遅延

	private void Awake()
	{

		if(objLifeBlockParent == null)
		{
			objLifeBlockParent = gameObject;
		}

		listLifeBlock = new List<GameObject>();

		for(int i = 0; i < objLifeBlockParent.transform.childCount; i++)
		{
			listLifeBlock.Add(objLifeBlockParent.transform.GetChild(i).gameObject);
		}

		foreach(GameObject obj in listLifeBlock)
		{

			obj.SetActive(false);

		}

	}

	private void Start()
	{

		StartCoroutine(SetAppearBlock());

	}

	// 生成アニメーション
	IEnumerator SetAppearBlock()
	{

		foreach (GameObject obj in listLifeBlock)
		{

			obj.SetActive(true);
			yield return new WaitForSeconds(delayAppearInterval);

		}

	}

}
