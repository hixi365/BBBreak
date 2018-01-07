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

	// 生成したオブジェクト
	private GameObject objHead;
	private GameObject[] objBody;

	// 生成パラメータ
	[SerializeField]
	private int lengthBody; // 生成時の胴体の数
	[SerializeField]
	private float intervalBodytoBody; // ボディ間のオフセット 

	// head 制御
	private Vector2 wposHead;
	private float angleHead;

	// body 制御
	private Vector2[] wposBody;
	private float[] angleBody;

	

	private void Start()
	{

		CreateSnake();

	}

	private void Update ()
	{
		
	}


	// 上向きの蛇を作り出す
	private void CreateSnake()
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

		// 頭の生成
		Vector2 pos = transform.position;
		float angle = transform.rotation.eulerAngles.z;
		Vector2 vecInterval = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));

		objHead = Instantiate(prefabHead, (Vector3)pos, Quaternion.Euler(0f, 0f, angle), transform);
		GameObject objHeadObject = objHead.transform.GetChild(0).gameObject;

		if(objHeadObject != null)
		{
			pos += vecInterval * objHeadObject.transform.lossyScale.y / 2;
		}
		else
		{ 
			pos += vecInterval * objHead.transform.lossyScale.y / 2;
		}
		
		// 胴の生成
		GameObject parentBody = new GameObject("Bodies");
		parentBody.transform.parent = transform;

		for(int i = 0; i < lengthBody; i++)
		{

			// 0回目のときだけ、自身の大きさの半分ずらす
			if(i == 0)
			{

				pos += vecInterval * prefabBody.transform.lossyScale.y / 2;
				pos += vecInterval * intervalBodytoBody;

			}

			GameObject o = Instantiate(prefabBody, (Vector3)pos, Quaternion.Euler(0f, 0f, angle), parentBody.transform);
			pos += vecInterval * o.transform.lossyScale.y;
			pos += vecInterval * intervalBodytoBody;

		}

	}

}
