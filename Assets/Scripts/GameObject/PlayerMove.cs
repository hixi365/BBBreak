using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ゲームオブジェクト
//  プレイヤーの移動 : マウス入力

public class PlayerMove : MonoBehaviour {

	[SerializeField]
	private float widthPlayerMove = 0.5f;   // プレイヤーが移動可能な幅 (16:9 で 0.56)
	[SerializeField]
	public Transform transformPlayer;       // プレイヤー移動用 transform
	[SerializeField]
	public Transform transformPlayerObject; // プレイヤーの大きさ取得用 spriteを持つobject

	[SerializeField]
	private Camera cameraMain;			// メインカメラ (スクリーン座標 → ワールド座標 変換用)

	private float yPlayerBase;			// 開始時のプレイヤーy座標
	private Vector3 posSMouseCurrent;   // マウスのスクリーン座標
	private Vector3 posWMouseCurrent;	// マウスのワールド座標

	private void Start ()
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
		x = Mathf.Min(x, +widthPlayerMove - transformPlayerObject.localScale.x / 2);
		x = Mathf.Max(x, -widthPlayerMove + transformPlayerObject.localScale.x / 2);
		transformPlayer.position = new Vector3(x, yPlayerBase, 0f);

	}
}
