using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UI
//  Text周期点滅
public class TextBlink : MonoBehaviour {
	
	[SerializeField]
	private UnityEngine.UI.Text text;

	[SerializeField]
	private float interval = 1f;

	[SerializeField]
	private float defaultAlpha = 0f;

	private float timeTotal = 0f;


	private void Start()
	{

		if (text == null)
		{

			text = GetComponent<UnityEngine.UI.Text>();

		}

		timeTotal += Mathf.Asin(defaultAlpha) * interval / Mathf.PI;

	}

	private void Update()
	{

		if (text == null)
		{

			return;

		}

		timeTotal += Time.deltaTime;
		float a = Mathf.Abs(Mathf.Sin(timeTotal / interval * Mathf.PI));

		Color color = text.color;
		color.a = a;
		text.color = color;

	}

}
