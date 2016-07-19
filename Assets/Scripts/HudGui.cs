using UnityEngine;
using System.Collections;

public class HudGui : MonoBehaviour {

	public static HudGui instance = null;
	public static HudGui Instance {get { return instance;} }

	//Start button offset
	public float startOffsetX = 0;
	public float startOffsetY =0;
	//Quit Button offset
	public float quitOffsetX =0;
	public float quitOffsetY=0;
	//Continue Button Offset
	public float continueOffsetX = 0;
	public float continueOffsetY =0;
	//Menu button offset
	public float menuOffsetX =0;
	public float menuOffsetY=0;


	private enum STATES{MENU,CREATOR, GAME,PLAYER_DEAD};
	private STATES curState = STATES.MENU;

	// Use this for initialization
	void Start () 
	{
		//Start button
		startOffsetX *=0.01f;
		startOffsetY *=0.01f;
		startOffsetX *= Screen.width;
		startOffsetY *= Screen.height;
		//Quit button
		quitOffsetX *=0.01f;
		quitOffsetY *= 0.01f;
		quitOffsetX *= Screen.width;
		quitOffsetY *= Screen.height;
		//Continue button
		continueOffsetX *=0.01f;
		continueOffsetY *= 0.01f;
		continueOffsetX *= Screen.width;
		continueOffsetY *= Screen.height;
		//Menu button
		menuOffsetX *=0.01f;
		menuOffsetY *=0.01f;
		menuOffsetX *= Screen.width;
		menuOffsetY *= Screen.height;

		if (instance != null && instance != this)
		{
			Destroy(this.gameObject);
			return;        
		} else {
			instance = this;
		}
		DontDestroyOnLoad(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnGUI()
	{
		switch(curState)
		{
		case STATES.MENU:
			if( GUI.Button(new Rect(startOffsetX,startOffsetY,200f,100f), "Start"))
			{
				Application.LoadLevel(Application.loadedLevel+1);
				curState = STATES.CREATOR;
			}

			if( GUI.Button(new Rect(quitOffsetX,quitOffsetY,200f,100f), "Quit"))
			{
				Application.Quit();
			}

			break;
		case STATES.CREATOR:
			if( GUI.Button(new Rect(continueOffsetX,continueOffsetY,200f,100f), "Continue"))
			{
				Application.LoadLevel(Application.loadedLevel+1);
				curState = STATES.GAME;
			}
			break;
		case STATES.GAME:
			if( GUI.Button(new Rect(menuOffsetX,menuOffsetY,50f,30f), "Menu"))
			{
				Application.LoadLevel("Menu");
				curState = STATES.MENU;
			}
			break;
		case STATES.PLAYER_DEAD:
			GUI.Label(new Rect(Screen.width *0.5f,Screen.height *0.5f,200f,100f), "You died");
			if( GUI.Button(new Rect(Screen.width *0.5f,Screen.height * 0.55f,100f,50f), "Main Menu"))
			{
				Application.LoadLevel("Menu");
				curState = STATES.MENU;
			}
			break;
		}
	}

	public void PlayerDied()
	{
		curState = STATES.PLAYER_DEAD;
	}
}
