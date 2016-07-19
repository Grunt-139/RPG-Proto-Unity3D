using UnityEngine;
using System.Collections;

public class EnemyWanderScript : MonoBehaviour {
	
	//Waypoint Container
	public Transform waypointContainer;
	public TargetingSensor sightSensor;
	public float searchDistance = 5.0f;
	public Character character;
	public int searchLength = 5;
	public float idleDelay = 1.0f;
	public float attackDelay = 1.0f;
	public float blockDelay = 0.5f;
	public float guardTime = 1.0f;
	public float gravity = 20.0f;

	//Waypoints continued
	private Transform[] waypoints;
	private Vector3[] searchWaypoints;
	private int currentWaypoint=0;
	private NavMeshAgent agent;
	private Animator animController;
	
	private bool grounded = false;
	private Vector3 moveDirection = Vector3.zero;
	private bool isWalking = false;
	private bool jumping = false;

	
	//Target
	private Transform target;

	//Combat
	public Collider weaponHitBox;
  	private int attackState;
	private int blockState; 
	private bool inRange;
	private bool isBlocking = false;
	private bool isAttacking = false;

	//States
	public enum STATES {IDLE,PATROL,SEARCHING,COMBAT};
	private STATES curState = STATES.PATROL;

	//Timers
	//Time to remain idle
	private float idleTime;
	//The cooldown time between attacks
	private float attackCooldown;
	//Cooldown time between blocking
	private float blockCooldown;
	//Amount of time spent blocking
	private float blockTime;

	
	// Use this for initialization
	void Start () 
	{
		agent = gameObject.GetComponent<NavMeshAgent>();
		waypointContainer = EnemySpawner.Instance.GetRandomRoute();
		GetWaypoints();
		
		animController = GetComponent<Animator>();

		searchWaypoints = new Vector3[searchLength];
		BuildSearchRoute();
		//Request a hash code of the attack animation state in the player’s animator.
    	attackState = Animator.StringToHash("UpperTorso.attack");
		blockState = Animator.StringToHash("UpperTorso.block");
    
    	//disable the weapon’s hit box collider
    	weaponHitBox.enabled=false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!character.IsDead())
		{
			switch(curState)
			{
			case STATES.IDLE:
				Idle();
				break;
			case STATES.PATROL:
				Patrol();
				break;
			case STATES.SEARCHING:
				Search();
				break;
			case STATES.COMBAT:
				Combat();
				if(!isAttacking)
				{	
					attackCooldown -= Time.deltaTime;
				}
				if(!isBlocking)
				{
					blockCooldown -= Time.deltaTime;
				}
				else
				{
					blockTime -= Time.deltaTime;
				}
				break;
			}
		
			if(target != null)
			{
				target = sightSensor.GetTarget();
				curState = STATES.COMBAT;
			}
			else
			{
				sightSensor.FindValidTarget();
				target = sightSensor.GetTarget();
			}

			if(inRange)
			{
				moveDirection = gameObject.transform.forward;
				moveDirection.z = 0;
			}
			else
			{
				moveDirection = gameObject.transform.forward;
			}

			//Apply gravity
			moveDirection.y -= gravity * Time.deltaTime;

			animController.SetBool("earthquake",EarthquakeEffect.Instance.IsShaking());
			
			if(EarthquakeEffect.Instance.IsShaking())
			{
				moveDirection = Vector3.zero;
			}

			//tell the animator what is going on.
	    	animController.SetFloat("Speed",moveDirection.z);
	    	animController.SetFloat("Direction",moveDirection.x);	
			animController.SetBool("isJumping",jumping);
			animController.SetBool("isBlocking",isBlocking);
			animController.SetBool("isAttacking",isAttacking);
			
//			//Move Charactercontroller and check if grounded
//			grounded = ((controller.Move(moveDirection * Time.deltaTime)) & CollisionFlags.Below) != 0;
//			
//			//Reset jumping after landing
//			jumping = grounded ? false : jumping;
		}
		else
		{	
			//tell the animator what is going on.
			animController.SetFloat("Speed",0);
			animController.SetFloat("Direction",0);	
			animController.SetBool("isJumping",false);
			animController.SetBool("isBlocking",false);
			weaponHitBox.enabled = false;
		}


	}

	void Idle()
	{
		idleTime -= Time.deltaTime;
		if(idleTime <=0)
		{
			curState = STATES.PATROL;
			currentWaypoint = 0;
		}
	}

	void Patrol()
	{
		Vector3 RelativeWaypointPosition = transform.InverseTransformPoint(new Vector3( waypoints[currentWaypoint].position.x, transform.position.y, waypoints[currentWaypoint].position.z ) );
		if ( RelativeWaypointPosition.magnitude <= agent.stoppingDistance ) 
		{
			currentWaypoint ++;
			
			//completed a lap
			if ( currentWaypoint >= waypoints.Length ) 
			{
				currentWaypoint = 0;
			}
			agent.SetDestination(waypoints[currentWaypoint].position);
		}
		agent.SetDestination(waypoints[currentWaypoint].position);
	}

	void Search()
	{
		Vector3 RelativeWaypointPosition = transform.InverseTransformPoint(new Vector3( searchWaypoints[currentWaypoint].x, transform.position.y, searchWaypoints[currentWaypoint].z ) );
		if ( RelativeWaypointPosition.magnitude <= agent.stoppingDistance ) 
		{
			currentWaypoint ++;
			
			//completed a lap
			if ( currentWaypoint >= searchWaypoints.Length ) 
			{
				curState = STATES.PATROL;
				currentWaypoint = 0;
			}
			agent.SetDestination(searchWaypoints[currentWaypoint]);
		}
		agent.SetDestination(searchWaypoints[currentWaypoint]);
	}

	void Combat()
	{
		if(target != null)
		{
			Vector3 targetPositionDist = transform.InverseTransformPoint(new Vector3( target.position.x, transform.position.y, target.position.z ));
			//is the player attacking?
			AnimatorStateInfo currentUpperTorsoState = animController.GetCurrentAnimatorStateInfo(1);

			//Range Check
			if(targetPositionDist.magnitude <=agent.stoppingDistance)
			{
				inRange = true; 
			}
			else
			{
				inRange = false;
			}

			//Block or Attack
			//If in range and the attackcooldown is less then 0 the enemy should attack
			//If hes in range and he cannot attack he should block
			//Otherwise just move forward

			if(currentUpperTorsoState.nameHash == attackState )
			{
				weaponHitBox.enabled = true;

			}
			else
			{
				if(inRange && !isAttacking && attackCooldown <=0)
				{
					//It can attack so schwing!
					agent.Stop();
					isAttacking = true;
					transform.rotation = Quaternion.RotateTowards(transform.rotation,target.rotation, 180f * Time.deltaTime);
					attackCooldown = attackDelay;
				}
				else if(!inRange && attackCooldown >0)
				{
					//Otherwise its out of range and wants to get closer
					isAttacking = false;
					agent.SetDestination(target.position);
					weaponHitBox.enabled = false;


					//Can I block?
					if(!isBlocking && blockCooldown <=0)
					{
						isBlocking = true;
						blockTime = guardTime;
					}

				}
			}	


		}
		else
		{
			BuildSearchRoute();
			curState = STATES.SEARCHING;
			currentWaypoint = 0;
		}
	}

	void BuildSearchRoute()
	{
		Vector3 randomPoint = Vector3.zero;
		randomPoint = Random.insideUnitSphere * searchDistance;
		randomPoint += transform.position;
		NavMeshHit hit;
		NavMesh.SamplePosition(randomPoint,out hit,searchDistance,1);
		searchWaypoints[0] = hit.position;

		for(int i=1; i < searchLength; i++)
		{
			randomPoint = Random.insideUnitSphere * searchDistance;
			randomPoint += searchWaypoints[i-1];
			NavMesh.SamplePosition(randomPoint,out hit,searchDistance,1);
			searchWaypoints[i] = hit.position;
		}
	}
	
	void GetWaypoints()
	{
		//NOTE: Unity named this function poorly it also returns the parent’s component.
		Transform[] potentialWaypoints = waypointContainer.GetComponentsInChildren<Transform>();
		
		//initialize the waypoints array so that is has enough space to store the nodes.
		waypoints = new Transform[ (potentialWaypoints.Length - 1) ];
		
		//loop through the list and copy the nodes into the array.
    	//start at 1 instead of 0 to skip the WaypointContainer’s transform.
		for (int i = 1; i < potentialWaypoints.Length; ++i ) 
		{
 			waypoints[ i-1 ] = potentialWaypoints[i];
		}
	}
	
	public Transform GetCurrentWaypoint()
	{
		return waypoints[currentWaypoint];	
	}
	
	public Transform GetLastWaypoint()
	{
		if(currentWaypoint - 1 < 0)
		{
			return waypoints[waypoints.Length - 1];
		}
		
		return waypoints[currentWaypoint - 1];
	}

//	void OnDrawGizmos () 
//	{
//		if(searchWaypoints != null)
//		{
// 			Vector3 last = searchWaypoints[searchWaypoints.Length-1];
//			for (int i = 1; i < waypoints.Length; i++ )
//			{
//				Gizmos.color = Color.red;
//				Gizmos.DrawSphere( searchWaypoints[i], 0.5f);
//				Gizmos.DrawLine(last,searchWaypoints[i]);
//				last = searchWaypoints[i];
//			}
//		}
//	}


	public void SetWaypointContainer(Transform container)
	{
		waypointContainer = container;
		GetWaypoints();
		currentWaypoint = 0;
	}
}
