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
	public GameObject prefabDontBreakBall;      // 構成するブロック 破壊不可能なブロック (跳ね返す)

	public GameObject prefabEnemyShot;			// 敵弾

	// preafabの大きさ (指定する)
	private Vector3 sizePrefabBlock;			// set preafab transform localScale
	private Vector3 sizePrefabDontBreakBlock;	// set preafab transform localScale
	private Vector3 sizePrefabDontBreakBall;    // set preafab transform localScale

	private Vector3 sizePrefabBlockEye;         // set preafab transform localScale


	// game object
	public GameObject objFase;		// 顔
	public GameObject[] objEye;		// 目 (予告)
	public GameObject objMouth;		// 口 (予告, 敵弾発射)
	public GameObject objTon;		// 舌 (防御用)

	public GameObject objHead;		// 頭
	public GameObject objBody;		// 身体 (子)
	public ArrayList objBodies = new ArrayList();	// 身体 (追従してくるリスト)

	// 変数
	private int phaseMove = -1;     // 行動パターン

	public Vector3 vecMove;			// 移動速度
	public Vector3 posCurrent;      // 現在の座標

	// 色
	public Color colHead = Color.green;			// 顔の色
	public Color colEyeWhite = Color.white;     // 目の色 白目
	public Color colEyeBlack = Color.black;     // 目の色 黒目
	public Color colMouth = Color.red;			// 口の色

	// ブロックサイズ
	private int size_head_w = 10;		// 頭の長方形横幅
	private int size_head_h = 13;       // 頭の長方形縦幅


	private float time = 0;
	private float timeTestShot = 0;

	private void Awake()
	{

		// 大きさの初期化 (本来はinspectorで)
		sizePrefabBlock = new Vector3(0.02f, 0.02f, 1f);
		sizePrefabDontBreakBlock = new Vector3(0.02f, 0.02f, 1f);
		sizePrefabDontBreakBall = new Vector3(0.5f, 0.5f, 1f);

		sizePrefabBlockEye = new Vector3(0.01f, 0.02f, 1f);

	}

	private void Start()
	{

		CreateSnake();

		posCurrent = new Vector3();
		vecMove = new Vector3();

	}
	
	private void Update()
	{
		time += Time.deltaTime;

		Vector3 v = objHead.transform.localScale;
		v.x = v.y = Mathf.Cos(time * 2f) * 0.05f + 1f;
		objHead.transform.localScale = v;

		//Vector3 p = new Vector3(0f, Mathf.Cos(time * 2f) * 0.2f, 0f);
		//objHead.transform.position = p;

		// テスト弾発射
		timeTestShot += Time.deltaTime;
		if(timeTestShot > 3f)
		{

			float base_angle = - Mathf.PI / 2 + Random.Range(-1, 1) * Mathf.PI / 8;

			for(int i = 0; i < 10; i++)
			{

				float angle = base_angle + i * 0.01f;
				
				Vector3 pos = objHead.transform.localPosition;
				GameObject obj = Instantiate(prefabEnemyShot, pos, Quaternion.identity);
				obj.transform.SetParent(transform, true);
				TestEnemyShot shot = obj.GetComponent<TestEnemyShot>();
				shot.vecMove = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * 0.5f;
				shot.delay = i * 0.03f;
			}

			timeTestShot = 0f;

		}

		// 頭の移動
		MoveHead();

		// 身体の移動
		MoveBody();

	}


	private void CreateSnake()
	{

		// 頭の生成
		GameObject head = new GameObject("head");
		head.transform.SetParent(transform, false);

		CreateHead(head);
		objHead = head;

		// 顔の生成
		GameObject fase = new GameObject("fase");
		fase.transform.SetParent(head.transform, false);

		CreateFase(fase);
		objFase = fase;

		// 身体の生成
		GameObject body = new GameObject("body");
		body.transform.SetParent(transform, true);

		CreateBody(body);
		objBody = body;

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
				obj.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Head");
				AddRandomAppear(obj);
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
				obj.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Head");
				AddRandomAppear(obj);
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
				obj.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Head");
				AddRandomAppear(obj);
			}

		}

	}

	private void CreateFase(GameObject fase)
	{

		// 左目
		Vector3 offset_eye_l = new Vector3(
			-(size_head_w / 2 - 0.5f) * sizePrefabBlock.x / 2,
			+(size_head_h / 2 - 0.5f) * sizePrefabBlock.y / 2,
			0f);

		// 右目
		Vector3 offset_eye_r = new Vector3(
			+(size_head_w / 2 - 0.5f) * sizePrefabBlock.x / 2,
			+(size_head_h / 2 - 0.5f) * sizePrefabBlock.y / 2,
			0f);


		// 白目
		{

			Vector3 pos_l = fase.transform.localPosition + offset_eye_l;
			GameObject obj_l = Instantiate(prefabDontBreakBall, pos_l, Quaternion.identity, fase.transform);
			obj_l.transform.localScale = sizePrefabDontBreakBall;
			obj_l.GetComponent<SpriteRenderer>().color = colEyeWhite;
			obj_l.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Fase");

			Vector3 pos_r = fase.transform.localPosition + offset_eye_r;
			GameObject obj_r = Instantiate(prefabDontBreakBall, pos_r, Quaternion.identity, fase.transform);
			obj_r.transform.localScale = sizePrefabDontBreakBall;
			obj_r.GetComponent<SpriteRenderer>().color = colEyeWhite;
			obj_r.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Fase");

		}

		// 黒目
		for (int y = -1; y <= 1; y++)
		{
			for(int x = 0; x <= 0; x++)
			{

				Vector3 pos_l = fase.transform.localPosition + new Vector3(x * sizePrefabBlockEye.x, y * sizePrefabBlockEye.y, 0f) + offset_eye_l;
				GameObject obj_l = Instantiate(prefabBlock, pos_l, Quaternion.identity, fase.transform);
				obj_l.transform.localScale = sizePrefabBlockEye;
				obj_l.GetComponent<SpriteRenderer>().color = colEyeBlack;
				obj_l.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Eye");

				Vector3 pos_r = fase.transform.localPosition + new Vector3(x * sizePrefabBlockEye.x, y * sizePrefabBlockEye.y, 0f) + offset_eye_r;
				GameObject obj_r = Instantiate(prefabBlock, pos_r, Quaternion.identity, fase.transform);
				obj_r.transform.localScale = sizePrefabBlockEye;
				obj_r.GetComponent<SpriteRenderer>().color = colEyeBlack;
				obj_r.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Eye");

			}
		}

	}

	private void CreateBody(GameObject body)
	{

		for(int l = 0; l < 30; l++)
		{

			GameObject b = new GameObject("body " + l);
			objBodies.Add(b);
 
			b.transform.SetParent(body.transform, true);
			b.transform.position = body.transform.position;

			for (int w = -5; w <= 5; w++)
			{

				Vector3 pos = new Vector3(w * sizePrefabBlock.x, 0f, 0f);
				GameObject obj = Instantiate(prefabBlock, pos, Quaternion.identity, b.transform);
				obj.transform.localScale = sizePrefabBlock;
				obj.GetComponent<SpriteRenderer>().color = l % 2 == 0 ? Color.black : Color.green;
				AddRandomAppear(obj, l * 0.2f);
			}

		}

	}

	private void MoveBody()
	{

		if (objBodies.Count == 0)
			return;

		if (Vector3.Distance((objBodies[0] as GameObject).transform.position, objHead.transform.position) < sizePrefabBlock.x)
			return;

		int c = objBodies.Count;

		for(int i = c - 1; i >= 0; i--)
		{
			Transform target = i == 0 ? objHead.transform : (objBodies[i - 1] as GameObject).transform;
			GameObject obj_current = objBodies[i] as GameObject;
			
			obj_current.transform.position = Vector3.Lerp(obj_current.transform.position, target.position, 0.3f);
			obj_current.transform.rotation = Quaternion.Lerp(obj_current.transform.rotation, target.rotation, 0.3f);
		}

	}

	private void MoveHead()
	{

		Vector3 vec = new Vector3(Mathf.Cos(time / 2), Mathf.Sin(time / 2), 0f) * 0.1f;
		vecMove = vec;

		posCurrent += vecMove * Time.deltaTime;

		objHead.transform.position = posCurrent;
		objHead.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(-vec.x, vec.y) * Mathf.Rad2Deg);

	}

	private void AddRandomAppear(GameObject obj, float delay = 0f)
	{

		TestAppearBlock appear = obj.AddComponent<TestAppearBlock>();

		float a = Random.Range(0, 2 * Mathf.PI);
		float rad = 1.0f;
		float roll = Random.Range(-Mathf.PI, +Mathf.PI) * 3f;

		Vector3 offsetPos = new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0f) * rad;
		Vector3 offsetRot = new Vector3(0f, 0f, roll);

		appear.fromPos = appear.transform.localPosition - offsetPos;
		appear.fromRot = appear.transform.localRotation.eulerAngles - offsetRot;

		appear.toPos = appear.transform.localPosition;
		appear.toRot = appear.transform.localRotation.eulerAngles;

		appear.delay = delay;

	}

}
