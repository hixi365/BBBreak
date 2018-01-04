using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ゲームオブジェクト
//  目の動きを管理
public class EyeManager : MonoBehaviour {

	// 目の状態
	//  NONE      未指定 (動かない)
	//  ANGLE_RAD 角度と半径を直接指定する
	//  TO_POS    特定の座標を注視する
	enum EyeState { NONE, ANGLE_RAD, TO_POS };

	private EyeState state = EyeState.NONE;

	// 黒目の最大半径 (画像の幅 * この定数 = 最大移動半径)
	private const float MAX_BLACK_EYE_RADIUS_RATE = 0.8f;

	// スプライト 黒目部分の配列
	[SerializeField]
	private GameObject[] objBlackEye;

	// 黒目のscale
	private float[] scaleEye;

	// 角度 半径
	private float angle = 0f;	// radian
	private float radius = 0f;	// 0 ~ 1

	// 注視する座標
	private Vector2 to = Vector2.zero;

	void Start()
	{
		
	}
	
	void Update ()
	{
	
		// 目の方向の更新
		switch(state)
		{

		case EyeState.NONE:

			InitEyeAngle();
			break;

		case EyeState.TO_POS:

			UpdateUsingTo();
			break;

		case EyeState.ANGLE_RAD:

			UpdateUsingRadAngle();
			break;

		}
	
		
	}

	// 黒目オブジェクトのlocalPositionを0埋め
	private void InitEyes()
	{

		scaleEye = new float[objBlackEye.Length];
		for(int i = 0; i < objBlackEye.Length; i++)
		{

			objBlackEye[i].transform.localPosition = Vector3.zero;
			scaleEye[i] = objBlackEye[i].transform.localScale.x;

		}

	}

	// 目のリセット (未指定状態)
	private void InitEyeAngle()
	{

		for (int i = 0; i < objBlackEye.Length; i++)
		{

			objBlackEye[i].transform.localPosition = Vector2.zero;

		}

	}

	// 特定の座標を向く
	private void UpdateUsingTo()
	{

		for (int i = 0; i < objBlackEye.Length; i++)
		{

			Vector2 pos = objBlackEye[i].transform.position;
			Vector2 toVec = (Vector2)Vector3.Normalize((Vector3)(to - pos));

			float rad = scaleEye[i] * MAX_BLACK_EYE_RADIUS_RATE;

			objBlackEye[i].transform.localPosition = toVec * rad;

		}

	}

	// 半径、角度の方向を向く (半径は最大半径を上限とする)
	private void UpdateUsingRadAngle()
	{

		for (int i = 0; i < objBlackEye.Length; i++)
		{

			Vector2 toVec = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
			float rad = scaleEye[i] * Mathf.Min( radius , MAX_BLACK_EYE_RADIUS_RATE );

			objBlackEye[i].transform.localPosition = toVec * rad;

		}

	}

	// 注視する座標の指定
	public void SetToPOs(Vector2 toPos)
	{

		to = toPos;
		state = EyeState.TO_POS;

	}

	// 角度と半径による目の方向の指定
	public void SetRadAndAngle(float radius_rate, float radian_angle)
	{

		angle = radian_angle;
		radius = radius_rate;
		state = EyeState.ANGLE_RAD;

	}

}
