using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// テスト
// マウス入力により、プレイヤーを移動させる

public class Test_PlayerMove : MonoBehaviour {

	public float widthPlayerMove = 0.5f;	// プレイヤーが移動可能な幅 (16:9で 0.56)
	public Transform transformPlayer;		// プレイヤー移動用
	public Transform transformSizePlayer;   // プレイヤーの大きさ取得用

	public Camera cameraMain;               // スクリーン座標 -> ワールド座標変換用

	private float yPlayerBase;				// 開始時のプレイヤーy座標

	private Vector3 posSMouseCurrent;		// マウスの現在スクリーン座標
	private Vector3 posWMouseCurrent;		// マウスの現在ワールド座標

	private void Start()
	{

		yPlayerBase = transformPlayer.position.y;

	}

	private void Update ()
	{

		// マウス座標の取得
		posSMouseCurrent = Input.mousePosition;
		posWMouseCurrent = cameraMain.ScreenToWorldPoint(posSMouseCurrent);

		// プレイヤーに座標書き込み
		float x = posWMouseCurrent.x;
		x = Mathf.Min(x, +widthPlayerMove - transformSizePlayer.localScale.x / 2);
		x = Mathf.Max(x, -widthPlayerMove + transformSizePlayer.localScale.x / 2);
		transformPlayer.position = new Vector3(x, yPlayerBase, 0f);
			
	}

}
