using UnityEngine;
using System.Collections;

public class SelectTexture : MonoBehaviour
{
	public Texture2D[] faces;
	public Texture2D[] tatoos;
	void OnGUI()
	{
		for (int i = 0; i < faces.Length; i++)
		{
			if (GUI.Button(new Rect(0, i * 64, 128, 64),faces[i].name))
			{
				ChangeMaterial("faces", i);
			}
		}
		for (int j = 0; j < tatoos.Length; j++)
		{
			if (GUI.Button(new Rect(128, j * 64, 128, 64),tatoos[j].name))
			{
				ChangeMaterial("tatoos", j);
			}
		}
	}
	
	void ChangeMaterial(string category, int index)
	{
		if (category == "faces")
		{
			renderer.material.mainTexture = faces[index];
			PlayerPrefs.SetInt("faceTex",index);
		}
		if (category == "tatoos")
		{
			renderer.material.SetTexture("_DecalTex",tatoos[index]);
			PlayerPrefs.SetInt("tatTex",index);
		}
	}
}