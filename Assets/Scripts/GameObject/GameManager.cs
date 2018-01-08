using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	// 参照して制御するゲームオブジェクト
	[SerializeField]
	private PlayerMove playerMove;

	// ミス扱いのゲームオブジェクト
	[SerializeField]
	private GameObject objMiss;

	// クリア扱いのゲームオブジェクト
	[SerializeField]
	private GameObject objBlockParent;

	// 終了時に出すキャンバス
	[SerializeField]
	private GameObject objCanvas;

	[SerializeField]
	private LayerMask layerBroken;   // 破壊済ブロックレイヤータグ
	private int layerValueBroken;         // 破壊済ブロックレイヤー (書き込み用)

	[SerializeField]
	private float waitForPlaying = 4.0f;    // ゲーム開始までのwait

	// フラグ
	private bool flgEnd = true;		// 終了
	private bool flgMiss = false;   // ミス
	private bool flgClear = false;	// クリア

	private void Awake()
	{

		InitializeGame();

	}

	private void Start ()
	{

		InitLayerValue();
		StartCoroutine(StartGame());

	}

	// 比較・代入用レイヤー値の計算
	private void InitLayerValue()
	{

		layerValueBroken = 0;
		int lBroken = layerBroken;
		while (0 < (lBroken >>= 1))
		{
			layerValueBroken++;

		}

	}

	// ゲームの初期化
	private void InitializeGame()
	{

		playerMove.enabled = false;

		if (objCanvas != null)
		{
			objCanvas.SetActive(false);
		}

	}

	// ゲーム開始コルーチン
	IEnumerator StartGame()
	{

		yield return new WaitForSeconds(waitForPlaying);
		playerMove.enabled = true;

	}

	private void Update ()
	{

		bool f = CheckEnd();
		if(f == true)
		{

			flgEnd = true;
			DrawResult();

		}

	}

	// 結果の表示
	private void DrawResult()
	{

		if (objCanvas == null)
		{
			return;
		}

		objCanvas.SetActive(true);

	}

	// 終了条件の確認
	private bool CheckEnd()
	{

		// 再入ははじく
		if (flgEnd == true)
		{

			return false;

		}

		// クリア
		if (objBlockParent == null || objBlockParent.transform.childCount == 0)
		{
			
			flgClear = true;
			return true;

		}

		// ミス
		else if (objMiss == null || objMiss.layer == layerValueBroken)
		{

			flgMiss = true;
			return true;

		}

		return false;

	}

}
