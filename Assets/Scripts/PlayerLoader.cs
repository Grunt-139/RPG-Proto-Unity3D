using UnityEngine;
using System.Collections;

public class PlayerLoader : MonoBehaviour {

	public Transform target;
	public Texture2D[] faces;
	public Texture2D[] tatoos;
	// Use this for initialization
	void Start () 
	{
		float skinValue = PlayerPrefs.GetFloat("skinValue");
		float skinTint = PlayerPrefs.GetFloat("skinTint");
		float bulk = PlayerPrefs.GetFloat("bulk");
		int faceTex = PlayerPrefs.GetInt("faceTex");
		int tatooTex = PlayerPrefs.GetInt("tatTex");
		float width = PlayerPrefs.GetFloat("width");
		float height = PlayerPrefs.GetFloat("height");
		
		//Set the height and width
		target.localScale = new Vector3(width,height,target.localScale.z);

		//Set the Bulk
		renderer.material.SetFloat("_Amount", bulk);

		//Set the tint and shade
		renderer.material.SetColor("_Colour", new Color(skinValue+skinTint, skinValue+(skinTint+skinTint), skinValue, 1));
	
		//Set materials
		renderer.material.mainTexture = faces[faceTex];
		renderer.material.SetTexture("_DecalTex",tatoos[tatooTex]);

	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
