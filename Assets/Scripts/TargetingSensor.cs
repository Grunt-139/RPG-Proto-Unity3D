using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class TargetingSensor : MonoBehaviour {

	public GameObject equippedTo;
	private List<Transform> targets;
	private int curTarget = 0;
	
	// Use this for initialization
	void Start () 
	{
		targets = new List<Transform>();
		targets.Add(null);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject != equippedTo && other.gameObject.tag != equippedTo.gameObject.tag)
		{
				targets.Add(other.transform);
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		targets.Remove(other.transform);
	}


	public void FindValidTarget()
	{
		curTarget = Random.Range(0,targets.Count);
	}

	public Transform GetTarget()
	{
		if(curTarget >= targets.Count)
		{
			curTarget = 0;
		}
		return targets[curTarget];
	}

	public void NextTarget()
	{
		curTarget++;
		if(curTarget >= targets.Count)
		{
			curTarget = 0;
		}
	}

	public void PrevTarget()
	{
		curTarget--;
		if(curTarget < 0)
		{
			curTarget = targets.Count - 1;
		}
	}
	
}
