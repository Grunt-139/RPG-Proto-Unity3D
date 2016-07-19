using UnityEngine;
using System.Collections;

public class GeneralColourSelector : MonoBehaviour 
{
	public enum Channel{ red, green, blue, alpha, all};
	public Channel changes;
	public Color startingColour = Color.grey;
	public float minValue = 0f;
	public float maxValue = 1f;
	public float xPositionOffset=0;
	public float yPositionOffset=0;
	private float colourValue;
	
	void Start()
	{
		colourValue = minValue+((maxValue - minValue)*0.5f);
		renderer.material.SetColor("_Color",startingColour);	
	}
	void OnGUI()
	{
		GUI.Label(new Rect(xPositionOffset, yPositionOffset, 90, 20), gameObject.name+":");
		colourValue = GUI.HorizontalSlider(new Rect(xPositionOffset, 15+yPositionOffset, 256,20), colourValue, minValue, maxValue);


		if (GUI.Button(new Rect(xPositionOffset, 25+yPositionOffset, 80, 20), "Save Value"))
		{
			PlayerPrefs.SetFloat(gameObject.name,colourValue);
		}
		
		switch(changes)
		{
			case (Channel.all):
				renderer.material.SetColor("_Color", new Color(startingColour.r*colourValue, startingColour.g*colourValue, startingColour.b*colourValue, 1));
			break;
			case (Channel.red):
				renderer.material.SetColor("_Color", new Color(colourValue, startingColour.g, startingColour.b, startingColour.a));
			break;
			case (Channel.blue):
				renderer.material.SetColor("_Color", new Color(startingColour.r, startingColour.g, colourValue, startingColour.a));
			break;
			case (Channel.green):
				renderer.material.SetColor("_Color", new Color(startingColour.r, colourValue, startingColour.b, startingColour.a));
			break;
			case (Channel.alpha):
				renderer.material.SetColor("_Color", new Color(startingColour.r, startingColour.g, startingColour.b, colourValue));
			break;
		}
	}
}
