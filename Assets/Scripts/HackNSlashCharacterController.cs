using UnityEngine;
using System.Collections;

public class HackNSlashCharacterController : MonoBehaviour {

	public string moveStatus = "idle";
	public bool walkByDefault = true;
	public float gravity = 20.0f;

	//Character
	public Character character;

	//Movement speeds
	public float jumpSpeed = 8.0f;
	public float runSpeed = 10.0f;
	public float walkSpeed = 4.0f;
	public float turnSpeed = 250.0f;
	public float moveBackwardsMultiplier = 0.75f;
	public float faceSpeed = 1.0f;
	
	//Internal vars to work with
	private float speedMultiplier = 0.0f;
	private bool grounded = false;
	private Vector3 moveDirection = Vector3.zero;
	private bool isWalking = false;
	private bool jumping = false;
	private bool isBlocking = false;
	private bool isRunning = false;
	private CharacterController controller;	


	//Auto lock
	private bool targetLocked = false;
	
	//Combat
	public Collider weaponHitBox;
	private int attackState;
	private int blockState;
	
	//Targeting sensor
	public TargetingSensor targetSensor;
	public HackNSlashCameraController myCamera;
	private Transform target;
	
	
	//Animator
	private Animator animController;
	
	void Awake()
	{
		//Get CharacterController
		controller = GetComponent<CharacterController>();	
		
		animController = GetComponent<Animator>();
		
		//Request a hash code of the attack animation state in the player’s animator.
		attackState = Animator.StringToHash("UpperTorso.attack");
		blockState = Animator.StringToHash("UpperTorso.block");
		
		//disable the weapon’s hit box collider
		weaponHitBox.enabled=false;
		
	}
	
	void Update ()
	{
		if(!character.IsDead())
		{
			//Set idle animation
			moveStatus = "idle";
			isWalking = walkByDefault;
			
			// Hold "Run" to run
			if(Input.GetAxis("Run") != 0)
			{
				isWalking = !walkByDefault;
			}
			
			// Only allow movement and jumps while grounded
			if(grounded && !myCamera.GetFreeLook()) 
			{	

				moveDirection = new Vector3(Input.GetAxis("Strafing"),0,Input.GetAxis("Vertical"));

				//if moving forward/backward and sideways at the same time, compensate for distance
				if(( Input.GetAxis("Strafing") != 0) && Input.GetAxis("Vertical") != 0) 
				{
					moveDirection *= 0.707f;
				}
				
				//apply the move backwards multiplier if not moving forwards only.
				if(Input.GetAxis("Strafing") != 0 || Input.GetAxis("Vertical") < 0)
				{
					speedMultiplier = moveBackwardsMultiplier;
				}
				else
				{
					speedMultiplier = 1f;	
				}
				
				//Use run or walkspeed
				moveDirection *= isWalking ? walkSpeed * speedMultiplier : runSpeed * speedMultiplier;
				
				// Jump!
				if(Input.GetButton("Jump"))
				{
					jumping = true;
					moveDirection.y = jumpSpeed;
				}
				
				//Determine our moveStatus state (for animations).    		
				if((moveDirection.x == 0 ) && (moveDirection.z == 0))
				{
					moveStatus = "idle";
				}
				
				if(moveDirection.z > 0)
				{
					if(isWalking)
					{
						moveStatus = "walking";
						isRunning = false;
					}
					else
					{
						moveStatus = "running";
						isRunning = true;
					}
				}
				
				if(moveDirection.z < 0)
				{
					if(isWalking)
					{
						moveStatus = "walkingBack";
						isRunning = false;
					}
					else
					{
						moveStatus = "runningBack";
						isRunning = true;
					}
				}
				
				if(moveDirection.x > 0)
				{
					if(isWalking)
					{
						moveStatus = "walkingRight";
						isRunning = false;
					}
					else
					{
						moveStatus = "runningRight";
						isRunning = true;
					}
				}
				
				if(moveDirection.x < 0)
				{
					if(isWalking)
					{
						moveStatus = "walkingLeft";
						isRunning = false;
					}
					else
					{
						moveStatus = "runningLeft";
						isRunning = true;
					}
				}
				
				//tell the animator what is going on.
				animController.SetFloat("Speed",moveDirection.z);
				animController.SetFloat("Direction",moveDirection.x);	
				animController.SetBool("isJumping",jumping);
				animController.SetBool("earthquake",EarthquakeEffect.Instance.IsShaking());

				if(EarthquakeEffect.Instance.IsShaking())
				{
					moveDirection = Vector3.zero;
				}
				
				//transform direction
				moveDirection = transform.TransformDirection(moveDirection);
				
				//is the player attacking?
				AnimatorStateInfo currentUpperTorsoState = animController.GetCurrentAnimatorStateInfo(1);
				if(currentUpperTorsoState.nameHash == attackState )
				{
					weaponHitBox.enabled = true;
				
				}
				else
				{
					if(Input.GetButtonDown("Attack"))
					{
						animController.SetBool("isAttacking", true);
						AudioManager.Instance.PlaySwordSwing(transform.position);
					}
					//attack has finished disable hitbox and state flag
					else
					{
						animController.SetBool("isAttacking", false);
						weaponHitBox.enabled = false;
					}

					if(Input.GetButtonDown("Block") && !isRunning)
					{
						//Shield combat?
						isBlocking = true;
					}
					else if( Input.GetButtonUp("Block") || isRunning)
					{
						isBlocking = false;
					}
				}

				if(currentUpperTorsoState.nameHash == blockState)
				{
					//TO DO or Decide
					//Shield combat?
					if(Input.GetButtonUp("Block") || isRunning)
					{
						isBlocking = false;
					}
				}

				if(target == null)
				{
					transform.Rotate(0,Input.GetAxis("Mouse X") * turnSpeed * Time.deltaTime, 0);
				}
				else
				{
					transform.rotation = Quaternion.Euler(0,Camera.main.transform.eulerAngles.y,0);
				}
			}
			animController.SetBool("isBlocking",isBlocking);
			
			//Targeting
			if(Input.GetButtonUp("Next Target") )
			{
				targetSensor.NextTarget();
				target = targetSensor.GetTarget();
				targetLocked = true;
			}
			else if(Input.GetButtonUp("Previous Target") )
			{
				targetSensor.PrevTarget();
				target = targetSensor.GetTarget();
				targetLocked = true;
			}

			if(targetSensor.GetTarget() == null || !targetSensor.GetTarget().gameObject.activeSelf)
			{
				targetLocked = false;
				target = null;
			}

			myCamera.SetTargetLock(targetLocked);

			//Apply gravity
			moveDirection.y -= gravity * Time.deltaTime;
			
			//Move Charactercontroller and check if grounded
			grounded = ((controller.Move(moveDirection * Time.deltaTime)) & CollisionFlags.Below) != 0;
			
			//Reset jumping after landing
			jumping = grounded ? false : jumping;
			
			//movestatus jump/swimup (for animations)      
			if(jumping)
			{
				moveStatus = "jump";
			}
			
			animController.SetBool("isJumping",jumping);

		}
	}
	
	public Transform GetTarget()
	{
		return target;
	}
}
