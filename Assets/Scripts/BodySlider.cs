using UnityEngine;
using System.Collections;

public class BodySlider : MonoBehaviour 
{
	public float startPoint = 0.5f;
	public float minHeightValue = 0.8f;
	public float maxHeightValue = 1.2f;
	public float minWidthValue = 0.8f;
	public float maxWidthValue = 1.2f;
	public Transform target;

	//Width Offset
	public float xWidthPositionOffset=0;
	public float yWidthPositionOffset=0;

	//Height Offset
	public float xHeightPositionOffset=0;
	public float yHeightPositionOffset=0;

	//Bulk Offset
	public float xBulkPositionOffset=0;
	public float yBulkPositionOffset=0;

	//Button Offset
	public float xButtonOffset = 0;
	public float yButtonOffset = 0;

	private float heightValue;
	private float widthValue;

	public float minBulkValue = -0.5f;
	public float maxBulkValue = 0.5f;
	private float bulkValue;
	
	// Use this for initialization
	void Start () 
	{
		//Width offset
		xWidthPositionOffset = Screen.width * ( xWidthPositionOffset * 0.01f);
		yWidthPositionOffset = Screen.height * (yWidthPositionOffset *0.01f);
		//Height offset
		xHeightPositionOffset = Screen.width * ( xHeightPositionOffset * 0.01f);
		yHeightPositionOffset = Screen.height * (yHeightPositionOffset * 0.01f);
		//Bulk offset
		xBulkPositionOffset = Screen.width * (xBulkPositionOffset * 0.01f);
		yBulkPositionOffset = Screen.height * (yBulkPositionOffset *0.01f);
		//Button offset
		xButtonOffset = Screen.width * ( xButtonOffset *0.01f);
		yButtonOffset = Screen.height * ( yButtonOffset * 0.01f);

		//set initial values to the starting point set by design.
		heightValue = minHeightValue+((maxHeightValue - minHeightValue)*startPoint);
		widthValue = minWidthValue+((maxWidthValue - minWidthValue)*startPoint);
		bulkValue = minBulkValue + ((maxBulkValue - minBulkValue) * startPoint);
	}
	
	// Update is called once per frame
	void OnGUI () 
	{
		//Height
		GUI.Label(new Rect(xHeightPositionOffset,15 + yHeightPositionOffset, 90, 20), "Height:");
		heightValue = GUI.HorizontalSlider(new Rect(xHeightPositionOffset, yHeightPositionOffset, 256,20), heightValue, minHeightValue, maxHeightValue);
		
		//Width
		GUI.Label(new Rect(xWidthPositionOffset,15 + yWidthPositionOffset, 90, 20), "Width:");
		widthValue = GUI.HorizontalSlider(new Rect(xWidthPositionOffset, yWidthPositionOffset, 256,20), widthValue, minWidthValue, maxWidthValue);
		
		//apply the size change to the model.
  	  	target.localScale = new Vector3(widthValue, heightValue, target.localScale.z);
	
		//Bulk
		GUI.Label(new Rect(xBulkPositionOffset,15 + yBulkPositionOffset,90,20), "Bulk");
		bulkValue = GUI.HorizontalSlider(new Rect(xBulkPositionOffset,yBulkPositionOffset,256,20), bulkValue,minBulkValue,maxBulkValue);
		renderer.material.SetFloat("_Amount", bulkValue);

		if(GUI.Button(new Rect(xButtonOffset, yButtonOffset, 80, 20), "Save Body"))
		{
			//Set the height and width
			PlayerPrefs.SetFloat("width",widthValue);
			PlayerPrefs.SetFloat("height",heightValue);
			PlayerPrefs.SetFloat("bulk",bulkValue);
		}
	
	}
}
