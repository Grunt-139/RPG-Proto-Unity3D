using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeightmapGrabber : MonoBehaviour {

	public Terrain terrain;

	private float[,] heightMapBackup;
	private int height;
	private int width;

	void Awake()
	{
		height = terrain.terrainData.heightmapHeight;
		width = terrain.terrainData.heightmapWidth;
		print(height + " " + width);
		heightMapBackup = new float[width,height];
		for(int i = 0; i < width; i++)
		{
			for(int j=0; j < height; j++)
			{
				heightMapBackup[i,j] = terrain.terrainData.GetHeight(i,j);
			}
		}
	}

	//this has to be done because terrains for some reason or another terrains don't reset after you run the app
	void OnApplicationQuit()
	{
		if (Debug.isDebugBuild)
		{
			//terrain.terrainData.SetHeights(0, 0, heightMapBackup);
		}
	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		float[,] heights = new float[width,height];
		for(int i=0; i < width; i++)
		{
			for(int j=0; j < height; j++)
			{
				heights[i,j] = heightMapBackup[i,j];
			}
		}

		if(Input.GetKeyUp(KeyCode.J))
		{
			for(int i = 0; i < width; i++)
			{
				for(int j=0; j < height; j++)
				{
					terrain.terrainData.SetHeights(i,j,heights);
				}
			}
		}

		if(Input.GetKeyUp(KeyCode.K))
		{
			for(int i = 0; i < width; i++)
			{
				for(int j=0; j < height; j++)
				{
					terrain.terrainData.SetHeights(i,j,heightMapBackup);
				}
			}
		}
		



	}
}
