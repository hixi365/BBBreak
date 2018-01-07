using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UI
//  Image周期点滅
public class ImageBlink : MonoBehaviour {

	[SerializeField]
	private UnityEngine.UI.Image image;

	[SerializeField]
	private float interval = 1f;

	[SerializeField]
	private float defaultAlpha = 0f;

	private float timeTotal = 0f;


	private void Start ()
	{
	
		if(image == null)
		{

			image = GetComponent<UnityEngine.UI.Image>();

		}

		timeTotal += Mathf.Asin(defaultAlpha) * interval / Mathf.PI;	

	}
	
	private void Update ()
	{
	
		if(image == null)
		{

			return;

		}

		timeTotal += Time.deltaTime;
		float a = Mathf.Abs(Mathf.Sin(timeTotal / interval * Mathf.PI));

		Color color = image.color;
		color.a = a;
		image.color = color;

	}

}
