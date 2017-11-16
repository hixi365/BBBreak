using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// テスト
// 敵 : 蛇クラス
// 自身のtransformの座標は頭の座標となる

public class TestSnake : MonoBehaviour {

	// preafab
	public GameObject prefabBlock;				// 構成するブロック 破壊可能なブロック
	public GameObject prefabDontBreakBlock;     // 構成するブロック 破壊不可能なブロック (跳ね返す)

	// preafabの大きさ (指定する)
	private Vector3 sizePrefabBlock;			// set preafab transform localScale
	private Vector3 sizePrefabDontBreakBlock;	// set preafab transform localScale

	// game object
	public GameObject[] objEye;             // 目 (予告)
	public GameObject[] objMouth;           // 口 (予告, 敵弾発射)
	public GameObject[] objTon;				// 舌 (防御用)

	public GameObject[] objHead;			// 頭
	public GameObject[] objBones;           // 身体

	// 変数
	private int phaseMove = -1;     // 行動パターン

	// 色
	public Color colHead = Color.green;			// 顔の色
	public Color colEyeWhite = Color.white;     // 目の色 白目
	public Color colEyeBlack = Color.black;     // 目の色 黒目

	// ブロックサイズ
	private int size_head_w = 10;		// 頭の長方形横幅
	private int size_head_h = 13;		// 頭の長方形縦幅




	private void Awake()
	{

		//	sizePrefabBlock = prefabBlock.transform.localScale;
		//	sizePrefabDontBreakBlock = prefabDontBreakBlock.transform.localScale;

		sizePrefabBlock = new Vector3(0.02f, 0.02f, 1f);
		sizePrefabDontBreakBlock = new Vector3(0.02f, 0.02f, 1f);

	}

	private void Start()
	{

		CreateSnake();

	}
	
	private void Update()
	{

	}


	private void CreateSnake()
	{

		// 頭の生成
		GameObject head = new GameObject("head");
		head.transform.parent = transform;

		CreateHead(head);


	}

	private void CreateHead(GameObject head)
	{

		
		// 長方形部分
		Vector3 offset_rect = new Vector3( // x = 0, y = 0 のオフセット座標
			-(size_head_w / 2 - 0.5f) * sizePrefabBlock.x,
			-(size_head_h / 2 - 0.5f) * sizePrefabBlock.y,
			0f);	
		for(int y = 0; y < size_head_h; y++)
		{
			for (int x = 0; x < size_head_w; x++)
			{
				Vector3 pos = head.transform.localPosition + new Vector3(x * sizePrefabBlock.x, y * sizePrefabBlock.y, 0f) + offset_rect;
				GameObject obj = Instantiate(prefabBlock, pos, Quaternion.identity, head.transform);
				obj.transform.localScale = sizePrefabBlock;
				obj.GetComponent<SpriteRenderer>().color = colHead;
			}
		}

		// 三角形部分 : 左側
		Vector3 offset_tri_l = offset_rect - new Vector3(sizePrefabBlock.x, 0f, 0f);
		for(int y = 0; y < size_head_h; y++)
		{
			int x_max = (y + 1) > size_head_h / 2 ? (size_head_h - y) : y + 1;
			for(int x = 0; x < x_max; x++)
			{
				Vector3 pos = head.transform.localPosition + new Vector3(- x * sizePrefabBlock.x, y * sizePrefabBlock.y, 0f) + offset_tri_l;
				GameObject obj = Instantiate(prefabBlock, pos, Quaternion.identity, head.transform);
				obj.transform.localScale = sizePrefabBlock;
				obj.GetComponent<SpriteRenderer>().color = colHead;
			}

		}

		// 三角形部分 : 右側
		Vector3 offset_tri_r = offset_rect + new Vector3(sizePrefabBlock.x * size_head_w, 0f, 0f);
		for (int y = 0; y < size_head_h; y++)
		{
			int x_max = (y + 1) > size_head_h / 2 ? (size_head_h - y) : y + 1;
			for (int x = 0; x < x_max; x++)
			{
				Vector3 pos = head.transform.localPosition + new Vector3(+x * sizePrefabBlock.x, y * sizePrefabBlock.y, 0f) + offset_tri_r;
				GameObject obj = Instantiate(prefabBlock, pos, Quaternion.identity, head.transform);
				obj.transform.localScale = sizePrefabBlock;
				obj.GetComponent<SpriteRenderer>().color = colHead;
			}

		}

	}

}
