using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour
{
	public Texture lifeIcon;
	public Texture damageIcon;
	public Character character;
	public Vector2 startPoint = Vector2.one;
	public int maxIconsPerRow = 5;
	public int maxRows = 2;
	private float maxHealth;
	private float currentHealth;
	private int totalHearts;
	private float conversion;
	private int maxIcons;

	void Start()
	{
		maxHealth = character.GetMaxHealth();
		currentHealth = character.GetHealth();

		maxIcons = maxRows * maxIconsPerRow;

		conversion = maxIcons / maxHealth;

		maxHealth *= conversion;
		currentHealth *= conversion;

		Mathf.FloorToInt(maxHealth);
		Mathf.FloorToInt(currentHealth);
	}

	void Update()
	{
		currentHealth = character.GetHealth();
	}

	//display the health bar.
	private void OnGUI()
	{
		int lifeIcons = Mathf.FloorToInt(currentHealth * conversion);
		int damIcons = maxIcons - lifeIcons;
		//print(currentHealth + " " + currentHealth/maxHealth);
		Rect rect = new Rect(startPoint.x,startPoint.y,Screen.width - startPoint.x, Screen.height-startPoint.y);
		GUILayout.BeginArea(rect);
		do
		{

			//print a row of icons up to the max allowed or the health left, whichever is less.
			GUILayout.BeginHorizontal();
				
			//if there are more icons left than we can fit on a row make a full row.
			if(lifeIcons > maxIconsPerRow)
			{
				ImagesForInteger(maxIconsPerRow,true);
			}
			//else create a row with just the remainder
			else
			{
				ImagesForInteger(lifeIcons,true);
			}
				
			//decrement our icon counter
			lifeIcons -= maxIconsPerRow;
		
			if(lifeIcons <=0)
			{
				do
				{
					//if there are more icons left than we can fit on a row make a full row.
					if(damIcons > maxIconsPerRow)
					{
						ImagesForInteger(maxIconsPerRow,false);
					}
					//else create a row with just the remainder
					else
					{
						ImagesForInteger(damIcons,false);
					}

					damIcons -= maxIconsPerRow;

				}while(damIcons >0);
			}

			//fill in the rest of the screen with blank space.
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
		}while(lifeIcons>0 && damIcons >0);
		
		GUILayout.EndArea();
	}

	//display a certain amount of icons
	private void ImagesForInteger(int total, bool life)
	{
		if(life)
		{
			for(int i=0; i < total; i++)
			{
				GUILayout.Label(lifeIcon);
			}
		}
		else
		{
			for(int i=0; i < total; i++)
			{
				GUILayout.Label(damageIcon);
			}
		}
	}
	
	//set the health bar to a specific value.
	public void SetHealth(int hitPoints)
	{
		currentHealth = hitPoints;
	}
	
	//alter the health bar by a certain amount.
	public void AlterHealth(int amount)
	{
		currentHealth += amount;
	}
}