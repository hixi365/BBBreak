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

	// 破壊可能な蛇のゲームオブジェクト
	[SerializeField]
	private GameObject[] objCanDestroySnake;
	private int numSnake;

	// 弾を撃つ蛇のゲームオブジェクト
	[SerializeField]
	private GameObject[] objShotSnake;

	// 終了時に出すキャンバス
	[SerializeField]
	private GameObject objCanvas;
	[SerializeField]
	private UnityEngine.UI.Text textResultHead;
	[SerializeField]
	private UnityEngine.UI.Text textResultBody;
	[SerializeField]
	private GameObject objButton;

	[SerializeField]
	private LayerMask layerBroken;   // 破壊済ブロックレイヤータグ
	private int layerValueBroken;         // 破壊済ブロックレイヤー (書き込み用)

	[SerializeField]
	private float waitForPlaying = 4.0f;    // ゲーム開始までのwait

	// フラグ
	private bool flgEnd = false;	// 終了
	private bool flgMiss = false;   // ミス
	private bool flgClear = false;  // クリア
	private bool flgInGame = false;	// ゲーム中か否か


	private void Awake()
	{

		InitializeGame();

	}

	private void Start()
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

		// プレイヤーの移動を無効に
		playerMove.enabled = false;
	
		// 破壊可能な蛇の数をカウント
		numSnake = objCanDestroySnake.Length;

		// Result表示のキャンバスを消す
		if (objCanvas != null)
		{
			objCanvas.SetActive(false);
		}

		// ゲーム開始まで蛇が弾を撃たないようにする
		foreach(GameObject obj in objShotSnake)
		{

			SnakeManager snake = obj.GetComponent<SnakeManager>();
			if(snake)
			{

				snake.SetShotState(0);

			}

		}

	}

	// ゲーム開始コルーチン
	IEnumerator StartGame()
	{

		yield return new WaitForSeconds(waitForPlaying);
		playerMove.enabled = true;
		flgInGame = true;

		// 蛇が弾を撃つようにする
		foreach (GameObject obj in objShotSnake)
		{

			SnakeManager snake = obj.GetComponent<SnakeManager>();
			if (snake)
			{

				snake.SetShotState(1);

			}

		}

	}

	private void Update()
	{

		if(flgInGame == false)
		{

			return;

		}

		bool f = CheckEnd();
		if(f == true)
		{

			flgEnd = true;
			StartCoroutine(ResultCoroutin());
		}

	}

	// 結果の表示コルーチン
	IEnumerator ResultCoroutin()
	{

		if (textResultHead != null)
		{
			if (flgMiss)
			{
				textResultHead.text = "Failed";
			}
		}

		if (objCanvas != null)
		{
			objCanvas.SetActive(true);
		}

		yield return new WaitForSeconds(1f);

		if (textResultBody != null)
		{

			textResultBody.gameObject.SetActive(true);

			textResultBody.text = "";
			textResultBody.text += "壊したブロック\n";
			textResultBody.text += (49 - objBlockParent.transform.childCount).ToString() + "\n";　// まずい
			textResultBody.text += "\n";
			textResultBody.text += "倒した蛇\n";
			textResultBody.text += (numSnake - objCanDestroySnake.Length).ToString() + "\n";

		}

		yield return new WaitForSeconds(1f);

		if (objButton != null)
		{

			objButton.SetActive(true);

		}

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
