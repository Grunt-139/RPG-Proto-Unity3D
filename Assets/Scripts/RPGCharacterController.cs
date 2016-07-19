using UnityEngine;

public class RPGCharacterController : MonoBehaviour 
{
	public string moveStatus = "idle";
	public bool walkByDefault = true;
	public float gravity = 20.0f;
	
	//Movement speeds
	public float jumpSpeed = 8.0f;
	public float runSpeed = 10.0f;
	public float walkSpeed = 4.0f;
	public float turnSpeed = 250.0f;
	public float moveBackwardsMultiplier = 0.75f;
	
	//Internal vars to work with
	private float speedMultiplier = 0.0f;
	private bool grounded = false;
	private Vector3 moveDirection = Vector3.zero;
	private bool isWalking = false;
	private bool jumping = false;
	private bool mouseSideButton = false;
	private CharacterController controller;	
	
	//Combat
	public Collider weaponHitBox;
  	private int attackState;
	
	//Targeting sensor
	public TargetingSensor targetSensor;
	public RPGCameraController camera;
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
    
    	//disable the weapon’s hit box collider
    	weaponHitBox.enabled=false;

	}
	
	void Update ()
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
		if(grounded) 
		{	
			//if the player is steering with the right mouse button, A/D strafe instead of turn.
			if(Input.GetMouseButton(1) )
			{
				moveDirection = new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical"));
			}
			else
			{
				moveDirection = new Vector3(0,0,Input.GetAxis("Vertical"));
			}

			//Auto-move button pressed
			if(Input.GetButtonDown("Toggle Move"))
			{
			    mouseSideButton = !mouseSideButton;
			}
    		
			//player moved or otherwise interrupted the auto-move.
			if(mouseSideButton && (Input.GetAxis("Vertical") != 0 || Input.GetButton("Jump")) || (Input.GetMouseButton(0) && Input.GetMouseButton(1)))
			{
				mouseSideButton = false;			
			}
			
			//L+R MouseButton Movement
			if ((Input.GetMouseButton(0) && Input.GetMouseButton(1)) || mouseSideButton)
			{
				moveDirection.z = 1;
			}
			
			//If not strafing with right mouse and horizontal, check for strafe keys
			if(!(Input.GetMouseButton(1) && Input.GetAxis("Horizontal") != 0))
			{
				moveDirection.x -= Input.GetAxis("Strafing");
			}
			    
		  	//if moving forward/backward and sideways at the same time, compensate for distance
			if(((Input.GetMouseButton(1) && Input.GetAxis("Horizontal") != 0) || Input.GetAxis("Strafing") != 0) && Input.GetAxis("Vertical") != 0) 
			{
				moveDirection *= 0.707f;
		  	}
		  					
			//apply the move backwards multiplier if not moving forwards only.
			if((Input.GetMouseButton(1) && Input.GetAxis("Horizontal") != 0) || Input.GetAxis("Strafing") != 0 || Input.GetAxis("Vertical") < 0)
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
				moveStatus = isWalking ? "walking" : "running";
			}
			
			if(moveDirection.z < 0)
			{
				moveStatus = isWalking ? "walkingBack" : "runningBack";
			}
			
			if(moveDirection.x > 0)
			{
				moveStatus = isWalking ? "walkingRight" : "runningRight";
			}
			
			if(moveDirection.x < 0)
			{
				moveStatus = isWalking ? "walkingLeft" : "runningLeft";
			}
			
			//tell the animator what is going on.
      		animController.SetFloat("Speed",moveDirection.z);
      		animController.SetFloat("Direction",moveDirection.x);	
			animController.SetBool("isJumping",jumping);
			
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
		      }
		      //attack has finished disable hitbox and state flag
		      else
		      {
		        animController.SetBool("isAttacking", false);
		        weaponHitBox.enabled = false;
		      }
			}
		}
		
		//Targeting
		if(Input.GetButtonUp("Target Cycle"))
		{
			targetSensor.NextTarget();
			target = targetSensor.GetTarget();

			if(target != null)
			{
				camera.SetTargetLock(true);
			}
			else
			{
				camera.SetTargetLock(false);
			}
		}
		
		
		//Character must face the same direction as the Camera when the right mouse button is down.
		if(Input.GetMouseButton(1)) 
		{
			transform.rotation = Quaternion.Euler(0,Camera.main.transform.eulerAngles.y,0);
		}
		else 
		{
			transform.Rotate(0,Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime, 0);
		}

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

	public Transform GetTarget()
	{
		return target;
	}
}
