using UnityEngine;
using System.Collections;
public class ColourSelector : MonoBehaviour
{
	public float redValue = 1.0f;
	public float greenValue = 1.0f;
	public float blueValue = 1.0f;
	bool selectorOn = false;
	private float redReset = 1.0f;
	private float greenReset = 1.0f;
	private float blueReset = 1.0f;
	
	void OnMouseUp()
	{
		selectorOn = true;
	}
	void OnGUI()
	{
		if (selectorOn)
		{
			GUI.Label(new Rect(10, 30, 90, 20), "Red: " + Mathf.RoundToInt(redValue * 255));
			redValue = GUI.HorizontalSlider(new Rect(80, 30, 256,20), redValue, 0.0f, 1.0f);
			GUI.Label(new Rect(10, 50, 90, 20), "Green: " + Mathf.RoundToInt(greenValue * 255));
			greenValue = GUI.HorizontalSlider(new Rect(80, 50,256, 20), greenValue, 0.0f, 1.0f);
			GUI.Label(new Rect(10, 70, 90, 20), "Blue: " + Mathf.RoundToInt(blueValue * 255));
			blueValue = GUI.HorizontalSlider(new Rect(80, 70, 256,20), blueValue, 0.0f, 1.0f);
			
			if (GUI.Button(new Rect(10, 110, 50, 20), "Ok"))
			{
				selectorOn = false;
				redReset = redValue;
				greenReset = greenValue;
				blueReset = blueValue;
			}
			
			if (GUI.Button(new Rect(70, 110, 80, 20), "Reset"))
			{
				redValue = redReset;
				greenValue = greenReset;
				blueValue = blueReset;
			}
			
			renderer.material.SetColor("_Color", new Color(redValue, greenValue, blueValue, 1));
		}
		else
		{
			GUI.Label(new Rect(10, 10, 500, 20), "Click the mace to change color.");
		}
	}
}
