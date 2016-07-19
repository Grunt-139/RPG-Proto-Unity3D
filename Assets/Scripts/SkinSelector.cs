using UnityEngine;
using System.Collections;

public class SkinSelector : MonoBehaviour 
{
	public float skinValue = 0.5f;
	public float skinTint = 0.05f;

	public float xValuePositionOffset=0;
	public float yValuePositionOffset=0;

	public float xTintPositionOffset=0;
	public float yTintPositionOffset=0;

	public float xButtonOffset=0;
	public float yButtonOffset=0;

	void Start()
	{
		//Skin value offset
		xValuePositionOffset *= 0.01f;
		yValuePositionOffset *= 0.01f;
//		xValuePositionOffset = Screen.width * (xValuePositionOffset * 0.01f);
//		yValuePositionOffset = Screen.height * (yValuePositionOffset * 0.01f);
		xValuePositionOffset *= Screen.width;
		yValuePositionOffset *= Screen.height;
		//Skin tint offset
		xTintPositionOffset *= 0.01f;
		yTintPositionOffset *= 0.01f;
//		xTintPositionOffset = Screen.width * (xTintPositionOffset * 0.01f);
//		yTintPositionOffset = Screen.height * (yTintPositionOffset *0.01f);
		xTintPositionOffset *= Screen.width;
		yTintPositionOffset *= Screen.height;
		//button offset
		xButtonOffset *= 0.01f;
		yButtonOffset *= 0.01f;
//		xButtonOffset = Screen.width * (xButtonOffset *0.01f);
//		yButtonOffset = Screen.height * (yButtonOffset *0.01f);
		xButtonOffset *= Screen.width;
		yButtonOffset *= Screen.height;

	}

	void OnGUI()
	{
		GUI.Label(new Rect(xValuePositionOffset, 15 + yValuePositionOffset, 90, 20), "Skin Shade:");
		skinValue = GUI.HorizontalSlider(new Rect(xValuePositionOffset, yValuePositionOffset, 256,20), skinValue, 0.4f, 1.0f);
		GUI.Label(new Rect(xTintPositionOffset, 15 + yTintPositionOffset, 90, 20), "Skin Tint:");
		skinTint = GUI.HorizontalSlider(new Rect(xTintPositionOffset, yTintPositionOffset, 256,20), skinTint, 0f, 0.1f);

		if (GUI.Button(new Rect(xButtonOffset, yButtonOffset, 80, 20), "Save Skin"))
		{
			PlayerPrefs.SetFloat("skinValue",skinValue);
			PlayerPrefs.SetFloat("skinTint",skinTint);
		}
		
		renderer.material.SetColor("_Colour", new Color(skinValue+skinTint, skinValue+(skinTint+skinTint), skinValue, 1));
	}
}
