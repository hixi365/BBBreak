using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {



	// 参照して制御するゲームオブジェクト
	[SerializeField]
	private PlayerMove playerMove;

	private void Awake()
	{

		InitializeGame();

	}

	private void Start ()
	{
		


	}
	
	// ゲームの初期化
	private void InitializeGame()
	{

		playerMove.enabled = false;

	}

	void Update ()
	{
		
	}
}
