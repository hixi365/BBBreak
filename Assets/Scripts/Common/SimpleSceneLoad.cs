using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 汎用
//  シンプルなシーン遷移

public class SimpleSceneLoad : MonoBehaviour {

	[SerializeField]
	private string nameScene;

	public void GoScene()
	{

		UnityEngine.SceneManagement.SceneManager.LoadScene(nameScene);

	}

}
