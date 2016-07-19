using UnityEngine;
using System.Collections;

public class FollowTarget : MonoBehaviour 
{
	public Transform target;
	private NavMeshAgent agent;

	
	// Use this for initialization
	void Start () 
	{
		agent = GetComponent<NavMeshAgent>(); 
	}
	
	// Update is called once per frame
	void Update () 
	{
		agent.SetDestination(target.position);
	}
}
