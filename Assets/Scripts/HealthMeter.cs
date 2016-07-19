using UnityEngine;
using System.Collections;

public class HealthMeter : MonoBehaviour
{	
	public Vector2 startPoint;
	public Texture2D[] images;
	private float hitPoints=0;
	private float maxHitPoints=0;
	
	private void OnGUI()
	{
		float normalisedHealth = (float)hitPoints / maxHitPoints;
		Rect rect = new Rect(startPoint.x,startPoint.y,Screen.width - startPoint.x, Screen.height-startPoint.y);
		GUILayout.BeginArea(rect);
		GUILayout.Label( HealthBarImage(normalisedHealth) );
		GUILayout.EndArea();
	}
	
	//figure out which image to show.
	private Texture2D HealthBarImage(float health)
	{
		for(int i=0; i < images.Length; i++)
		{
			if( health >= 1f - ((1f / (images.Length-1)) * i) )
			{
				return images[i];
			}
		}
		
		return null;
	}
				
	//set the range covered by the health bar to a specific value.
	public void SetMaxHitPoints(int newValue)
	{
		maxHitPoints = newValue;
		hitPoints = maxHitPoints;
	}
	
	//alter the current amount of health in the health bar.
	public void AlterHealth(int amount)
	{
		hitPoints += amount;
	}
}