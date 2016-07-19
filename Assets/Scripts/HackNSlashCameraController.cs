using UnityEngine;
using System.Collections;

public class HackNSlashCameraController : MonoBehaviour {

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

	public float targetLockHeightOffset = 2.0f;
	
	private float xDeg = 0.0f;
	private float yDeg = 0.0f;
	private float currentDistance;
	private float desiredDistance;
	private float correctedDistance;
	private bool freeLook = false;
	private bool targetLock = false;
	private float shakeTime;
	private Vector3 shakeDirection;
	
	private HackNSlashCharacterController targetScript;
	
	void Start ()
	{      
		Vector3 angles = transform.eulerAngles;
		xDeg = angles.x;
		yDeg = angles.y;
		currentDistance = distance;
		desiredDistance = distance;
		correctedDistance = distance; 
		
		targetScript = target.GetComponent<HackNSlashCharacterController>();
	}
	
	//Only Move camera after everything else has been updated
	void LateUpdate ()
	{
		if(target != null)
		{
			//auto-move button pressed
			if(Input.GetButtonUp("Toggle Look"))
			{
				freeLook = !freeLook;
			}
			// Free Look is activated
			if (freeLook)
			{
				xDeg += Input.GetAxis ("Mouse X") * xSpeed * 0.02f;
				yDeg -= Input.GetAxis ("Mouse Y") * ySpeed * 0.02f;
			}
			else 
			{
				yDeg -= Input.GetAxis ("Mouse Y") * ySpeed * 0.02f;
				RotateBehindTarget();
			}

			Quaternion rotation = Quaternion.identity;
			Vector3 position = transform.position;


			if(targetLock)
			{
				//ensure the pitch is within the camera constraints
				yDeg = ClampAngle (yDeg, yMinLimit * 0.5f, yMaxLimit * 0.5f);
				
				Vector3 lockPos = targetScript.GetTarget().position - target.position;
				lockPos.y -= targetLockHeightOffset;
				rotation = Quaternion.LookRotation(lockPos);
				
				// Calculate the desired distance
				desiredDistance -= Input.GetAxis ("Mouse ScrollWheel") * Time.deltaTime * (zoomSpeed * 1.25f) * Mathf.Abs (desiredDistance);
				desiredDistance = Mathf.Clamp (desiredDistance, minDistance, maxDistance);
				correctedDistance = desiredDistance;
				
				// Calculate desired camera position
				Vector3 vTargetOffset = new Vector3 (0, -heightOffset, 0);
				position = target.transform.position - (rotation * Vector3.forward * desiredDistance + vTargetOffset);
				
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

			}
			else
			{
				
				//ensure the pitch is within the camera constraints
				yDeg = ClampAngle (yDeg, yMinLimit, yMaxLimit);
				
				// Set camera rotation
				rotation = Quaternion.Euler (yDeg, xDeg, 0);
				
				// Calculate the desired distance
				desiredDistance -= Input.GetAxis ("Mouse ScrollWheel") * Time.deltaTime * zoomSpeed * Mathf.Abs (desiredDistance);
				desiredDistance = Mathf.Clamp (desiredDistance, minDistance, maxDistance);
				correctedDistance = desiredDistance;
				
				// Calculate desired camera position
				Vector3 vTargetOffset = new Vector3 (0, -heightOffset, 0);
				position = target.transform.position - (rotation * Vector3.forward * desiredDistance + vTargetOffset);
				
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

			}

			if(EarthquakeEffect.Instance.IsShaking())
			{
				Vector3 quakeVec = Random.insideUnitSphere * EarthquakeEffect.Instance.GetShake();
				quakeVec.z = 0;
				position += quakeVec * Time.deltaTime;
			}


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

	public bool GetFreeLook()
	{
		return freeLook;
	}
	
	public void SetTargetLock(bool lockOn)
	{
		targetLock = lockOn;
	}

	public bool GetTargetLock()
	{
		return targetLock;
	}
}
