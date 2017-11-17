using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRestartScene : MonoBehaviour {

	public void RestartScene()
	{

		UnityEngine.SceneManagement.SceneManager.LoadScene(0);

	}

}
