using UnityEngine;
using System.Collections;

public class EarthquakeEffect : MonoBehaviour {
	
	//Lets make this a singleton
	private static EarthquakeEffect instance = null;
	public static EarthquakeEffect Instance {get{return instance;} }
	
	public Terrain terrain;

	public Transform player;

	//Earthquake variables
	public float earthquakeDelayMin = 10.0f;
	public float earthquakeDelayMax = 20.0f;
	public float crackWidthMin = 0f;
	public float crackWidthMax = 1f;
	public float crackHeightMin = -5f;
	public float crackHeightMax = 5f;
	public float crackLengthMin = 0;
	public float crackLengthMax = 5;
	public int numCracksMin = 1;
	public int numCracksMax = 5;
	public float camShake = 1f;

	public float distFromPlayerMin = 10f;
	public float distFromPlayerMax = 20f;


	private float[,] heightMapBackup;
	private float[, ,] alphaMapBackup;
	private float[,] curHeightMap;
	private float[, ,] curAlphaMap;
	private int hmHeight;
	private int hmWidth;
	private float terrHeight;
	private float terrWidth;
	private int alphaMapHeight;
	private int alphaMapWidth;

	//Timer for the quake
	private float curTime;
	private bool isShaking = false;
	private float earthquakeDelayHalf;
	private bool isBuilt = false;

	//Direction of the cracks
	private Vector3[] crackWaypoints;
	private Vector3 originPoint;
	private float mapHeight;
	private int curNumCracks;
	private int curHeight;
	private int curWidth;
	private float curEarthquakeDelay;

	//Conversion numbers
	//width/size and height/size
	private float heightConversion;
	private float widthConversion;

	//Earthquake effect
	Vector3 curPos;
	int curIndex;

	//States
	private enum STATES{IDLE,BUILD,SHAKING};
	private STATES curState = STATES.IDLE;

	void Awake()
	{
		if(instance != null && instance != this)
		{
			Destroy(this.gameObject);
			return;
		}
		else
		{
			instance = this;
		}
	}
	
	// Use this for initialization
	void Start () 
	{
		//Sets up the backups and such
		LoadInitialValues();

		//Set the timer to the delay
		curEarthquakeDelay = Random.Range(earthquakeDelayMin,earthquakeDelayMax);
		earthquakeDelayHalf = curEarthquakeDelay * 0.5f;
		curTime = curEarthquakeDelay;

		//Set up the arrays
		//Initialize the array so it is long enough to contain as many cracks as the designer wants
		crackWaypoints = new Vector3[Mathf.FloorToInt(crackLengthMax)];



		heightConversion = hmHeight / terrain.terrainData.size.z;
		widthConversion = hmWidth / terrain.terrainData.size.x;

	}
	
	// Update is called once per frame
	void Update () 
	{
		switch(curState)
		{
		case STATES.IDLE:
			curTime -= Time.deltaTime;

			if(curTime < earthquakeDelayHalf)
			{
				curState = STATES.BUILD;
			}

			break;
		case STATES.BUILD:
			curTime -= Time.deltaTime;
			if(!isBuilt)
			{
				BuildDirectionVectors();
				BuildQuakeDetails();
				isBuilt = true;
			}

			if(curTime <=0)
			{
				InitiateEarthquake();
				curState = STATES.SHAKING;
			}
			break;
		case STATES.SHAKING:
			ShakeTheWorld();
			AudioManager.Instance.PlayEarthquake();
			//End the state if the cracks are done
			if(curIndex == curNumCracks)
			{
				AudioManager.Instance.StopEarthquake();
				curEarthquakeDelay = Random.Range(earthquakeDelayMin,earthquakeDelayMax);
				earthquakeDelayHalf = curEarthquakeDelay * 0.5f;
				curTime = curEarthquakeDelay;
				curState = STATES.IDLE;
				isShaking = false;
				isBuilt = false;
			}
			break;
		}
	}
	
	
	void LoadInitialValues()
	{
		hmHeight = terrain.terrainData.heightmapHeight;
		hmWidth = terrain.terrainData.heightmapWidth;
		alphaMapWidth = terrain.terrainData.alphamapWidth;
		alphaMapWidth = terrain.terrainData.alphamapWidth;

		terrHeight = terrain.terrainData.size.z;
		terrWidth = terrain.terrainData.size.x;
		
		if(Debug.isDebugBuild)
		{
			heightMapBackup = terrain.terrainData.GetHeights(0,0,hmWidth,hmHeight);
			alphaMapBackup = terrain.terrainData.GetAlphamaps(0,0,alphaMapWidth,alphaMapHeight);
		}
		
		curHeightMap = terrain.terrainData.GetHeights(0,0,hmWidth,hmHeight);
		curAlphaMap = terrain.terrainData.GetAlphamaps(0,0,alphaMapWidth,alphaMapHeight);
	}

	void InitiateEarthquake()
	{
		curPos = originPoint;
		curIndex = 0;
		isShaking = true;
	}

	void ShakeTheWorld()
	{
		Vector3 relativeDistance = crackWaypoints[curIndex] - curPos;

		DeformTerrain(curPos,curWidth);

		curPos += (crackWaypoints[curIndex] - curPos).normalized;

		if(relativeDistance.magnitude < 0.5f)
		{
			curIndex++;
		}


	}
	
	protected void DeformTerrain(Vector3 pos, float craterSizeInMeters)
	{
		//get the heights only once keep it and reuse, precalculate as much as possible
		Vector3 terrainPos = GetRelativeTerrainPositionFromPos(pos,terrain,hmWidth,hmHeight);//terr.terrainData.heightmapResolution/terr.terrainData.heightmapWidth
		int heightMapCraterWidth = (int)(craterSizeInMeters * (widthConversion));
		int heightMapCraterLength = (int)(craterSizeInMeters * (heightConversion));
		int heightMapStartPosX = (int)(terrainPos.x - (heightMapCraterWidth / 2));
		int heightMapStartPosZ = (int)(terrainPos.z - (heightMapCraterLength / 2));
		
		float[,] heights = terrain.terrainData.GetHeights(heightMapStartPosX, heightMapStartPosZ, heightMapCraterWidth, heightMapCraterLength);
		float circlePosX;
		float circlePosY;
		float distanceFromCenter;
		float depthMultiplier;
		
		float deformationDepth = curHeight / terrain.terrainData.size.y;
		
		// we set each sample of the terrain in the size to the desired height
		for (int i = 0; i < heightMapCraterLength; i++) //width
		{
			for (int j = 0; j < heightMapCraterWidth; j++) //height
			{
				circlePosX = (j - (heightMapCraterWidth / 2)) / (hmWidth / terrain.terrainData.size.x);
				circlePosY = (i - (heightMapCraterLength / 2)) / (hmHeight / terrain.terrainData.size.z);
				distanceFromCenter = Mathf.Abs(Mathf.Sqrt(circlePosX * circlePosX + circlePosY * circlePosY));
				//convert back to values without skew
				
				if (distanceFromCenter < (craterSizeInMeters / 2.0f))
				{
					depthMultiplier = ((craterSizeInMeters / 2.0f - distanceFromCenter) / (craterSizeInMeters / 2.0f));
					
					depthMultiplier += 0.1f;
					depthMultiplier += Random.value * .1f;
					
					depthMultiplier = Mathf.Clamp(depthMultiplier, 0, 1);
					heights[i, j] = Mathf.Clamp(heights[i, j] - deformationDepth * depthMultiplier, 0, 1);
				}
				
			}
		}
		
		// set the new height
		terrain.terrainData.SetHeights(heightMapStartPosX, heightMapStartPosZ, heights);
	}
	
	void BuildDirectionVectors()
	{
		//Build the direction of the vectors using these values
		float length = Random.Range(crackLengthMin,crackLengthMax);
		Vector3 randomDirection = Vector3.zero;

		float xSpread = Random.Range(distFromPlayerMin,distFromPlayerMax);
		float zSpread = Random.Range(distFromPlayerMin,distFromPlayerMax);


		float xMax;
		float xMin;
		
		float zMax;
		float zMin;

		float xDirection = Random.Range(0,100);
		float zDirection = Random.Range(0,100);

		if(xDirection > 50)
		{
			//This sends it to the right

			xMin = player.transform.position.x + xSpread;
			xMax = xMin + xSpread;
		}
		else
		{
			//this sends it all to the left
			xMin = player.transform.position.x - xSpread;
			xMax = xMin - xSpread;
		}

		if(zDirection > 50)
		{
			zMin = player.transform.position.z + zSpread;
			zMax = zMin + zSpread;
		}
		else
		{
			zMin = player.transform.position.z - zSpread;
			zMax = zMin + zSpread;
		}


		xMax = xMax > terrain.terrainData.size.x ? terrain.terrainData.size.x : xMax;
		xMin = xMin < 0 ? 0 : xMin;

		zMax = zMax > terrain.terrainData.size.z ? terrain.terrainData.size.z : zMax;
		zMin = zMin < 0 ? 0 : zMin;

		originPoint = new Vector3( Random.Range(xMin,xMax) , 0 , Random.Range(zMin,zMax) );



		curNumCracks = Random.Range(numCracksMin,numCracksMax);

		originPoint.y = terrain.terrainData.GetHeight(Mathf.FloorToInt(originPoint.x),Mathf.FloorToInt(originPoint.z));

		//Crack direction is the unit vector of the cracks direction
		//Crack waypoints are the starting and ending points of each crack
		
		
		//First waypoint is the origin point
		crackWaypoints[0] = originPoint;
		
		//Currently each crack is really how many times the direction changes
		for(int i=1; i < curNumCracks; i++)
		{
			length = Random.Range(crackLengthMin,crackLengthMax);
			randomDirection = Random.insideUnitSphere;
			//Now build the next way point
			crackWaypoints[i] = randomDirection * length;
			crackWaypoints[i] += crackWaypoints[i-1];
			crackWaypoints[i].y = terrain.terrainData.GetHeight(Mathf.FloorToInt(crackWaypoints[i].x),Mathf.FloorToInt(crackWaypoints[i].z));
		}

		
	}
	
	void BuildQuakeDetails()
	{
		curWidth = Random.Range( Mathf.FloorToInt(crackWidthMin) ,  Mathf.FloorToInt(crackWidthMax));
		curHeight = Random.Range(  Mathf.FloorToInt(crackHeightMin) ,  Mathf.FloorToInt(crackHeightMax));
	}
	
	
	void OnApplicationQuit()
	{
		if(Debug.isDebugBuild)
		{
			terrain.terrainData.SetHeights(0,0,heightMapBackup);
			terrain.terrainData.SetAlphamaps(0,0,alphaMapBackup);
		}
	}
	
	
	public bool IsShaking()
	{
		return isShaking;
	}
	
	protected Vector3 GetNormalizedPositionRelativeToTerrain(Vector3 pos, Terrain terr)
	{
		//code based on: http://answers.unity3d.com/questions/3633/modifying-terrain-height-under-a-gameobject-at-runtime
		// get the normalized position of this game object relative to the terrain
		Vector3 tempCoord = (pos - terr.gameObject.transform.position);
		Vector3 coord;
		coord.x = tempCoord.x / terrain.terrainData.size.x;
		coord.y = tempCoord.y / terrain.terrainData.size.y;
		coord.z = tempCoord.z / terrain.terrainData.size.z;
		
		return coord;
	}
	
	protected Vector3 GetRelativeTerrainPositionFromPos(Vector3 pos,Terrain terr, int mapWidth, int mapHeight)
	{
		Vector3 coord = GetNormalizedPositionRelativeToTerrain(pos, terr);
		// get the position of the terrain heightmap where this game object is
		return new Vector3((coord.x * mapWidth), 0, (coord.z * mapHeight));
	}     

	
//	void OnDrawGizmos () 
//	{
//		if(crackWaypoints[0] != null && curNumCracks >0)
//		{
//			Vector3 last = crackWaypoints[curNumCracks-1];
//			for (int i = 0; i < crackWaypoints.Length; i++ )
//			{
//				Gizmos.color = Color.red;
//				Gizmos.DrawSphere( crackWaypoints[i], 0.5f);
//				Gizmos.DrawLine(last,crackWaypoints[i]);
//				last = crackWaypoints[i];
//			}
//		}
//	}

	public float GetShake()
	{
		if(isShaking)
		{
			return camShake;
		}
		else
		{
			return 0;
		}
	}
	

}

