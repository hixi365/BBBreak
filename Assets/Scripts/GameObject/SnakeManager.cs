using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ゲームオブジェクト
//  蛇 クラス

public class SnakeManager : MonoBehaviour {

	// 壁面での操作
	//  NONE      未指定 (何もしない)
	//  REFLECT   即座に反射する
	//  DELETE    削除する
	enum MoveAreaState { NONE, REFLECT, DELETE };

	[SerializeField]
	private MoveAreaState stateMoveArea = MoveAreaState.NONE;

	// 蛇の目の動き方
	//  NONE	未指定 (何もしない)
	//	CENTER	中央を見る
	//	MOVE	移動方向を向く
	//	PLAYER	プレイヤーの方を向く
	enum EyeState { NONE, CENTER, MOVE, PLAYER };

	[SerializeField]
	private EyeState stateEye = EyeState.NONE;

	// prefab
	[SerializeField]
	private GameObject prefabHead;
	[SerializeField]
	private GameObject prefabBody;

	// layer
	[SerializeField]
	private LayerMask layerBlock;       // ブロックレイヤータグ
	[SerializeField]
	private LayerMask layerBroken;   // 破壊済ブロックレイヤータグ

	// layer value
	private int layerValueBlock;         // 未破壊ブロックレイヤー (書き込み用)
	private int layerValueBroken;         // 破壊済ブロックレイヤー (書き込み用)

	// 生成したオブジェクト
	private GameObject objHead;
	private List<GameObject> listBody = new List<GameObject>();
	private GameObject objParentBody;
	private GameObject objHeadObject; // 頭のSprite部分

	// 生成パラメータ
	[SerializeField]
	private int lengthBody; // 生成時の胴体の数
	[SerializeField]
	private float intervalBodytoBody; // ボディ間のオフセット 

	// 生成した胴の長さ
	private float scaleBody;

	// 壁面
	[SerializeField]
	private float minXReflect;
	[SerializeField]
	private float maxXReflect;
	[SerializeField]
	private float minYReflect;
	[SerializeField]
	private float maxYReflect;

	// 移動パラメータ
	[SerializeField]
	private float angleMove;
	[SerializeField]
	private float speedMove;

	// 胴パラメータ
	private int currentLength;	// 現在の長さ

	// head 制御
	private Vector2 wposHead;
	private float angleHead;

	// body 制御
	private Vector2[] wposBody;
	private float[] angleBody;

	// head 制御 途中
	private Vector2 wposBufHead;
	private float angleBufHead;

	// body 制御 途中
	private Vector2[] wposBufBody;
	private float[] angleBufBody;

	// 目 制御
	private EyeManager managerEye;

	// 色 制御
	[SerializeField]
	private Color colorHead = Color.white;
	[SerializeField]
	private Color[] colorBody;
	[SerializeField]
	private float intervalBlink = 0.25f;
	[SerializeField]
	private float timeBlink = 0.5f;
	private float timerBlink = 0f;

	private void Start()
	{

		// 胴部分の色がなければ、頭の色を代入
		if(colorBody.Length == 0)
		{
			colorBody = new Color[1];
			colorBody[0] = colorHead;
		}

		InitLayerValue();
		CreateRightSnake();
		UpdateSnakeColor();

	}

	// 比較・代入用レイヤー値の計算
	private void InitLayerValue()
	{

		layerValueBlock = 0;
		int lBlock = layerBlock;
		while (0 < (lBlock >>= 1))
		{
			layerValueBlock++;

		}

		layerValueBroken = 0;
		int lBroken = layerBroken;
		while (0 < (lBroken >>= 1))
		{
			layerValueBroken++;

		}


	}

	private void Update ()
	{

		UpdateDestroyBlock();
		ReflectSnake();
		MoveSnake();
		UpdateSnakeColor();

	}


	// 右向きの蛇を作り出す
	private void CreateRightSnake()
	{

		// 胴体が無いものは生成しない
		if (lengthBody <= 0)
		{

			Debug.Log("SnakeManager : lengthBody is 0");
			return;

		}

		// prefabが未指定の場合は生成しない
		if(prefabBody == null || prefabHead == null)
		{

			Debug.Log("SnakeManager : prefab is null");
			return;

		}

		// 配列の確保
		wposBody = new Vector2[lengthBody];
		angleBody = new float[lengthBody];
		wposBufBody = new Vector2[lengthBody];
		angleBufBody = new float[lengthBody];

		Vector2 pos = transform.position;
		float angle = transform.rotation.eulerAngles.z;
		Vector2 vecInterval = -new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * transform.localScale.x;
		
		// 頭の生成
		objHead = Instantiate(prefabHead, (Vector3)pos, Quaternion.Euler(0f, 0f, angle), transform);
		if (objHead.transform.childCount >= 0)
		{
			objHeadObject = objHead.transform.GetChild(0).gameObject;
		}

		wposHead = pos;
		angleHead = angle * Mathf.Deg2Rad;

		if(objHeadObject != null)
		{
			pos += vecInterval * objHeadObject.transform.lossyScale.y / 2;
		}
		else
		{ 
			pos += vecInterval * objHead.transform.lossyScale.y / 2;
		}

		// 目の取得
		managerEye = objHead.GetComponent<EyeManager>();
		if(managerEye != null)
		{
		
			if (stateEye == EyeState.CENTER)
			{

				managerEye.SetToPOs(Vector2.zero);

			}

		}
		
		// 胴の生成
		objParentBody = new GameObject("Bodies");
		objParentBody.transform.parent = transform;
		objParentBody.transform.localScale = new Vector3(1f, 1f, 1f);

		for (int i = 0; i < lengthBody; i++)
		{

			// 0回目のときだけ、自身の大きさの半分ずらす
			if(i == 0)
			{

				scaleBody = prefabBody.transform.lossyScale.y * transform.localScale.x;

			//	pos += vecInterval * scaleBody / 2;
			//	pos += vecInterval * intervalBodytoBody;

			}

			GameObject o = Instantiate(prefabBody, (Vector3)pos, Quaternion.Euler(0f, 0f, angle), objParentBody.transform);
			listBody.Add(o);

			wposBody[i] = pos;
			angleBody[i] = angle * Mathf.Deg2Rad;

			pos += vecInterval * scaleBody / 2;
			pos += vecInterval * intervalBodytoBody;

		}

		angleMove = angleHead;
	//	speedMove = 0f;

	}

	// 破壊されたブロックの部分を埋める
	private void UpdateDestroyBlock()
	{

		// 頭に直撃した場合破壊
		if(objHeadObject.layer == layerValueBroken)
		{

			Destroy(gameObject);
			return;

		}

		int countDestroy = 0;

		foreach(GameObject obj in listBody)
		{

			// 破壊済であるならば、最後尾を削除 (objはlayerを戻す)
			if (obj.layer == layerValueBroken)
			{

				GameObject last = listBody[listBody.Count - 1];
				Destroy(last);

				countDestroy++;

				obj.layer = layerValueBlock;

				// 破壊があれば点滅
				timerBlink = timeBlink;

			}

		}

		listBody.RemoveRange(listBody.Count - countDestroy, countDestroy);
		currentLength = listBody.Count;

		// 長さが1以下の時破壊
		if(currentLength <= 1)
		{

			Destroy(gameObject);
			return;

		}

	}

	// 蛇を現在の向き、速度で移動させる
	private void MoveSnake()
	{

		// 胴の長さに対する移動量の比率を表す
		float moveRate = speedMove;;

		// 線形補間しながら進み続ける
		while (moveRate > 0)
		{

			float rate = moveRate;
			rate = Mathf.Min(1f, rate);
			moveRate -= rate;

			// 頭の移動
			wposBufHead = wposHead + rate * (scaleBody + intervalBodytoBody) * new Vector2(Mathf.Cos(angleMove), Mathf.Sin(angleMove));
			angleBufHead = angleMove;

			// 胴の移動
			for (int i = 0; i < currentLength; i++)
			{

				int i0 = i - 1;
				int i1 = i;
				float e0 = rate;

				Vector2 v0 = wposHead;
				Vector2 v1 = wposHead;

				float angle0 = angleHead;
				float angle1 = angleHead;

				if (i0 >= 0)
				{
					v0 = wposBody[i0];
					angle0 = angleBody[i0];
				}

				if (i1 >= 0)
				{
					v1 = wposBody[i1];
					angle1 = angleBody[i1];
				}

				Vector2 newPos = v0 * e0 + v1 * (1f - e0);
				float newAngle = angle0 * e0 + angle1 * (1f - e0);

				wposBufBody[i] = newPos;
				angleBufBody[i] = newAngle;

			}

			// バッファを書き込む
			wposHead = wposBufHead;
			angleHead = angleBufHead;

			for (int i = 0; i < currentLength; i++)
			{

				wposBody[i] = wposBufBody[i];
				angleBody[i] = angleBufBody[i];

			}

		}

		// 計算した座標を書き込む
		objHead.transform.position = (Vector3)wposHead;
		objHead.transform.rotation = Quaternion.Euler(0f, 0f, angleHead * Mathf.Rad2Deg);

		for(int i = 0; i < currentLength; i++)
		{

			GameObject obj = listBody[i];

			obj.transform.position = (Vector3)wposBody[i];
			obj.transform.rotation = Quaternion.Euler(0f, 0f, angleBody[i] * Mathf.Rad2Deg);

		}

		// 目の向き
		if (managerEye != null)
		{

			if (stateEye == EyeState.MOVE)
			{

				managerEye.SetRadAndAngle(1.2f, angleMove);

			}

		}

	}

	// 壁面設定時の動作
	private void ReflectSnake()
	{

		// 未動作
		if(stateMoveArea == MoveAreaState.NONE)
		{

			return;

		}

		Vector2 vec = new Vector2(Mathf.Cos(angleMove), Mathf.Sin(angleMove));

		if(wposHead.x > maxXReflect)
		{

			if(stateMoveArea == MoveAreaState.DELETE)
			{
				Destroy(gameObject);
				return;
			}	
			else if(stateMoveArea == MoveAreaState.REFLECT)
			{

				vec.x = -vec.x;

			}								

		}

		else if (wposHead.x < minXReflect)
		{

			if (stateMoveArea == MoveAreaState.DELETE)
			{
				Destroy(gameObject);
				return;
			}
			else if (stateMoveArea == MoveAreaState.REFLECT)
			{

				vec.x = -vec.x;

			}

		}

		if (wposHead.y > maxYReflect)
		{

			if (stateMoveArea == MoveAreaState.DELETE)
			{
				Destroy(gameObject);
				return;
			}
			else if (stateMoveArea == MoveAreaState.REFLECT)
			{

				vec.y = -vec.y;

			}

		}

		else if (wposHead.y < minYReflect)
		{

			if (stateMoveArea == MoveAreaState.DELETE)
			{
				Destroy(gameObject);
				return;
			}
			else if (stateMoveArea == MoveAreaState.REFLECT)
			{

				vec.y = -vec.y;

			}

		}

		angleMove = Mathf.Atan2(vec.y, vec.x);

	}

	// 蛇の色更新
	private void UpdateSnakeColor()
	{

		float alpha = 1f;
		
		if (timerBlink > 0f)
		{
			alpha = Mathf.Abs(Mathf.Sin(timerBlink / intervalBlink * Mathf.PI));
			timerBlink -= Time.deltaTime;
		}

		// 顔
		{

			SpriteRenderer renderer = objHeadObject.GetComponent<SpriteRenderer>();
			if(renderer != null)
			{

				Color color = colorHead;
				color.a = alpha;
				renderer.color = color;

			}

		}

		int lenColor = colorBody.Length;
		for(int i = 0; i < currentLength; i++)
		{

			int indexColor = i % lenColor;

			SpriteRenderer renderer = listBody[i].GetComponent<SpriteRenderer>();
			if (renderer != null)
			{

				Color color = colorBody[indexColor];
				color.a = alpha;
				renderer.color = color;

			}

		}

	}

}
