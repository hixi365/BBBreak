using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// テスト
// ブロック出現

public class TestSetAppearBlocks : MonoBehaviour {


	private void Awake()
	{

		TestAppearBlock[] objs = GetComponentsInChildren<TestAppearBlock>(true);

		foreach (TestAppearBlock obj in objs)
		{
			obj.gameObject.SetActive(false);
		}

	}

	private void Start()
	{

		RandomAppear();

	}
		
	private void RandomAppear()
	{

		TestAppearBlock[] objs = GetComponentsInChildren<TestAppearBlock>(true);

		foreach (TestAppearBlock obj in objs)
		{

			obj.gameObject.SetActive(true);

			float a = Random.Range(0, 2 * Mathf.PI);
			float rad = 1.0f;
			float roll = Random.Range(-Mathf.PI, +Mathf.PI) * 3f;

			Vector3 offsetPos = new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0f) * rad;
			Vector3 offsetRot = new Vector3(0f, 0f, roll);

			obj.fromPos = obj.transform.localPosition - offsetPos;
			obj.fromRot = obj.transform.localRotation.eulerAngles - offsetRot;

			obj.toPos = obj.transform.localPosition;
			obj.toRot = obj.transform.localRotation.eulerAngles;

		}

	}

}
