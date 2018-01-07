using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ゲームオブジェクト
//  蛇 クラス

public class SnakeManager : MonoBehaviour {

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

	// 生成パラメータ
	[SerializeField]
	private int lengthBody; // 生成時の胴体の数
	[SerializeField]
	private float intervalBodytoBody; // ボディ間のオフセット 

	// 生成した胴の長さ
	private float scaleBody;

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


	private void Start()
	{

		InitLayerValue();
		CreateRightSnake();
		
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
		MoveSnake();

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

		// 頭の生成
		Vector2 pos = transform.position;
		float angle = transform.rotation.eulerAngles.z;
		Vector2 vecInterval = - new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
		
		objHead = Instantiate(prefabHead, (Vector3)pos, Quaternion.Euler(0f, 0f, angle), transform);
		GameObject objHeadObject = objHead.transform.GetChild(0).gameObject;

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
		
		// 胴の生成
		objParentBody = new GameObject("Bodies");
		objParentBody.transform.parent = transform;

		for(int i = 0; i < lengthBody; i++)
		{

			// 0回目のときだけ、自身の大きさの半分ずらす
			if(i == 0)
			{

				scaleBody = prefabBody.transform.lossyScale.y;

				pos += vecInterval * scaleBody / 2;
				pos += vecInterval * intervalBodytoBody;

			}

			GameObject o = Instantiate(prefabBody, (Vector3)pos, Quaternion.Euler(0f, 0f, angle), objParentBody.transform);
			listBody.Add(o);

			wposBody[i] = pos;
			angleBody[i] = angle * Mathf.Deg2Rad;

			pos += vecInterval * scaleBody;
			pos += vecInterval * intervalBodytoBody;

		}

		angleMove = angleHead;
		speedMove = 0f;

	}

	// 破壊されたブロックの部分を埋める
	private void UpdateDestroyBlock()
	{

		foreach(GameObject obj in listBody)
		{

			// 破壊済であるならば、最後尾を削除 (objはlayerを戻す)
			if (obj.layer == layerValueBroken)
			{

				GameObject last = listBody[listBody.Count - 1];
				Destroy(last);

				obj.layer = layerValueBlock;

			}

		}

		currentLength = listBody.Count;

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
			wposBufHead = wposHead + rate * scaleBody * new Vector2(Mathf.Cos(angleMove), Mathf.Sin(angleMove));
			angleBufHead = angleMove;

			Debug.DrawRay((Vector3)(wposBufHead) + new Vector3(0, 0, 1), Vector3.up * scaleBody, new Color(1f, 0f, 0f));
			Debug.DrawRay((Vector3)(wposBufHead) + new Vector3(0, 0, 1), Vector3.right * scaleBody, new Color(0f, 1f, 0f));

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

	}

}
