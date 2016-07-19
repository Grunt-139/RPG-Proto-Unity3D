using UnityEngine;
using System.Collections;
     
public class RPGCameraController : MonoBehaviour 
{  
	public Transform target;
    public float heightOffset = 1.7f;
    public float distance = 12.0f;
	public float offsetFromWall = 0.1f;
	public float maxDistance = 20f;
	public float minDistance = 0.6f;
	public float xSpeed = 200.0f;
    public float ySpeed = 200.0f;
    public float yMinLimit = -80f;
    public float yMaxLimit = 80f;
    public float zoomSpeed = 40f;
    public float autoRotationSpeed = 3.0f;
    public float autoZoomSpeed = 5.0f;
    public LayerMask collisionLayers = -1;
    public bool alwaysRotateToRearOfTarget = false;
    public bool allowMouseInputX = true;
    public bool allowMouseInputY = true;
     
    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private float correctedDistance;
    private bool rotateBehind = false;
    private bool mouseSideButton = false;
	private bool targetLock = false;

	private RPGCharacterController targetScript;
         
    void Start ()
    {      
        Vector3 angles = transform.eulerAngles;
        xDeg = angles.x;
        yDeg = angles.y;
        currentDistance = distance;
        desiredDistance = distance;
        correctedDistance = distance; 

		if (alwaysRotateToRearOfTarget)
		{
			rotateBehind = true;
		}

		targetScript = target.GetComponent<RPGCharacterController>();
	}
         
    //Only Move camera after everything else has been updated
    void LateUpdate ()
    {
        //auto-move button pressed
        if(Input.GetButton("Toggle Move"))
		{
        	mouseSideButton = !mouseSideButton;
        }
		
		//player moved or otherwise interrupted the auto-move.
		if(mouseSideButton && (Input.GetAxis("Vertical") != 0 || Input.GetButton("Jump")) || (Input.GetMouseButton(0) && Input.GetMouseButton(1)))
		{
			mouseSideButton = false;		
		}
		                   
        // If either mouse buttons are down, let the mouse govern camera position
        if (GUIUtility.hotControl == 0)
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                //Check to see if mouse input is allowed on the axis
                if (allowMouseInputX)
				{
                    xDeg += Input.GetAxis ("Mouse X") * xSpeed * 0.02f;
				}
                else
				{
                    RotateBehindTarget();
				}
				
                if (allowMouseInputY)
				{
                    yDeg -= Input.GetAxis ("Mouse Y") * ySpeed * 0.02f;
				}
               
                //Interrupt rotating behind if mouse wants to control rotation
                if (!alwaysRotateToRearOfTarget)
				{
                    rotateBehind = false;
				}
            }
            // otherwise, ease behind the target if any of the directional keys are pressed
            else if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0 || rotateBehind || mouseSideButton)
            {
                RotateBehindTarget();
            }
        }

     	if(targetLock)
		{
			//ensure the pitch is within the camera constraints
			yDeg = ClampAngle (yDeg, yMinLimit * 0.5f, yMaxLimit * 0.5f);

			Vector3 lockPos = targetScript.GetTarget().position - target.position;
			lockPos.y -= 2f;
			Quaternion rotation = Quaternion.LookRotation(lockPos);

			// Calculate the desired distance
			desiredDistance -= Input.GetAxis ("Mouse ScrollWheel") * Time.deltaTime * (zoomSpeed * 1.25f) * Mathf.Abs (desiredDistance);
			desiredDistance = Mathf.Clamp (desiredDistance, minDistance, maxDistance);
			correctedDistance = desiredDistance;

			// Calculate desired camera position
			Vector3 vTargetOffset = new Vector3 (0, -heightOffset, 0);
			Vector3 position = target.transform.position - (rotation * Vector3.forward * desiredDistance + vTargetOffset);
			
			// Check for collision using the true target's desired registration point as set by user using height
			RaycastHit collisionHit;
			Vector3 trueTargetPosition = new Vector3 (target.transform.position.x, target.transform.position.y + heightOffset, target.transform.position.z);
			
			// If there was a collision, correct the camera position and calculate the corrected distance
			bool isCorrected = false;
			if (Physics.Linecast (trueTargetPosition, position, out collisionHit, collisionLayers))
			{
				// Calculate the distance from the original estimated position to the collision location,
				// subtracting out a safety "offset" distance from the object we hit.  The offset will help
				// keep the camera from being right on top of the surface we hit, which usually shows up as
				// the surface geometry getting partially clipped by the camera's front clipping plane.
				correctedDistance = Vector3.Distance (trueTargetPosition, collisionHit.point) - offsetFromWall;
				isCorrected = true;
			}
			
			//Smooth the transition by lerping the distance, if it was corrected.
			if(!isCorrected || correctedDistance > currentDistance)
			{
				currentDistance = Mathf.Lerp (currentDistance, correctedDistance, Time.deltaTime * autoZoomSpeed);
			}
			else
			{
				currentDistance = correctedDistance;
			}
			
			// Keep within limits
			currentDistance = Mathf.Clamp (currentDistance, minDistance, maxDistance);
			
			// Recalculate position based on the new currentDistance
			position = target.transform.position - (rotation * Vector3.forward * currentDistance + vTargetOffset);
			
			//Finally Set rotation and position of camera
			transform.rotation = rotation;
			transform.position = position;
		}
		else
		{

			//ensure the pitch is within the camera constraints
			yDeg = ClampAngle (yDeg, yMinLimit, yMaxLimit);

	        // Set camera rotation
	        Quaternion rotation = Quaternion.Euler (yDeg, xDeg, 0);
	     
	        // Calculate the desired distance
	        desiredDistance -= Input.GetAxis ("Mouse ScrollWheel") * Time.deltaTime * zoomSpeed * Mathf.Abs (desiredDistance);
	        desiredDistance = Mathf.Clamp (desiredDistance, minDistance, maxDistance);
	        correctedDistance = desiredDistance;
	     
	        // Calculate desired camera position
	        Vector3 vTargetOffset = new Vector3 (0, -heightOffset, 0);
	        Vector3 position = target.transform.position - (rotation * Vector3.forward * desiredDistance + vTargetOffset);
	     
	        // Check for collision using the true target's desired registration point as set by user using height
	        RaycastHit collisionHit;
	        Vector3 trueTargetPosition = new Vector3 (target.transform.position.x, target.transform.position.y + heightOffset, target.transform.position.z);
	     
	        // If there was a collision, correct the camera position and calculate the corrected distance
	        bool isCorrected = false;
	        if (Physics.Linecast (trueTargetPosition, position, out collisionHit, collisionLayers))
	        {
	            // Calculate the distance from the original estimated position to the collision location,
	            // subtracting out a safety "offset" distance from the object we hit.  The offset will help
	            // keep the camera from being right on top of the surface we hit, which usually shows up as
	            // the surface geometry getting partially clipped by the camera's front clipping plane.
	            correctedDistance = Vector3.Distance (trueTargetPosition, collisionHit.point) - offsetFromWall;
	            isCorrected = true;
	        }
	     
	        //Smooth the transition by lerping the distance, if it was corrected.
			if(!isCorrected || correctedDistance > currentDistance)
			{
				currentDistance = Mathf.Lerp (currentDistance, correctedDistance, Time.deltaTime * autoZoomSpeed);
			}
			else
			{
				currentDistance = correctedDistance;
			}
	        
			// Keep within limits
	        currentDistance = Mathf.Clamp (currentDistance, minDistance, maxDistance);

			// Recalculate position based on the new currentDistance
			position = target.transform.position - (rotation * Vector3.forward * currentDistance + vTargetOffset);
			
			//Finally Set rotation and position of camera
			transform.rotation = rotation;
			transform.position = position;
		}
    }
     
    private void RotateBehindTarget()
    {
        float targetRotationAngle = target.transform.eulerAngles.y;
        float currentRotationAngle = transform.eulerAngles.y;
        xDeg = Mathf.LerpAngle (currentRotationAngle, targetRotationAngle, autoRotationSpeed * Time.deltaTime);
       
        // Stop rotating behind if not completed
        if (targetRotationAngle == currentRotationAngle)
        {
            if (!alwaysRotateToRearOfTarget)
			{
                rotateBehind = false;
			}
        }
        else
		{
            rotateBehind = true;
		}
     
    }
    
	//keeps the camera between a min and max angle which falls between -360 and +360.
	private float ClampAngle (float angle, float min, float max)
	{
		if (angle < -360f)
		{
	      angle += 360f;
		}
		
		if (angle > 360f)
		{
	      angle -= 360f;
		}
		
		return Mathf.Clamp (angle, min, max);
	}

	public void SetTargetLock(bool lockOn)
	{
		targetLock = lockOn;
	}
}