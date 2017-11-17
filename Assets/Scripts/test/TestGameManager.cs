using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// テスト
// ゲームマネージャー

public class TestGameManager : MonoBehaviour {

	public UnityEngine.UI.Image imgSkillBar;
	public UnityEngine.UI.Button buttonSkillButton;

	public GameObject objPlayer;
	public GameObject objPlayerShot;

	private float amountSkill;
	private bool usingSkill;

	private void Update()
	{

		imgSkillBar.fillAmount = amountSkill;
		buttonSkillButton.interactable = amountSkill >= 1f;

		// 時間での増加
		AddAmount(Time.deltaTime / 30f);

		if(usingSkill)
		{
			amountSkill -= Time.deltaTime / 15f;
			if(amountSkill <= 0f)
			{
				usingSkill = false;
				amountSkill = 0f;
				objPlayerShot.GetComponent<Test_ShotMove>().isBarMode = false;
			}
		}

	}

	public void OnSkillButton()
	{

		usingSkill = true;
		objPlayerShot.GetComponent<Test_ShotMove>().isBarMode = true;

	}

	public void AddAmount(float a)
	{

		if (usingSkill)
			return;

		amountSkill = Mathf.Min(amountSkill + a, 1f);

	}
	
}
