using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	// 参照して制御するゲームオブジェクト
	[SerializeField]
	private PlayerMove playerMove;

	[SerializeField]
	private float waitForPlaying = 4.0f;	// ゲーム開始までのwait

	private void Awake()
	{

		InitializeGame();

	}

	private void Start ()
	{

		StartCoroutine(StartGame());


	}
	
	// ゲームの初期化
	private void InitializeGame()
	{

		playerMove.enabled = false;

	}

	// ゲーム開始コルーチン
	IEnumerator StartGame()
	{

		yield return new WaitForSeconds(waitForPlaying);
		playerMove.enabled = true;

	}

	void Update ()
	{
		
	}
}
