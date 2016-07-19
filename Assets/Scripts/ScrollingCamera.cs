using UnityEngine;
using System.Collections;

public class ScrollingCamera : MonoBehaviour {
	
	public float distance = 6.0f;
	public float zoomSpeed = 40.0f;
	public float minDistance = 0.6f;
	public float maxDistance = 12.0f;
	public float heightOffset = 2.0f;
	public float xSpeed = 200.0f;
	public float ySpeed = 200.0f;
	public float yMinLimit = -80f;
	public float yMaxLimit = 80f;
	private float xDeg = 0.0f;
	private float yDeg = 0.0f;
	public Transform target;
	
	private float desiredDistance;
	
	// Use this for initialization
	void Start () 
	{
		Vector3 angles = transform.eulerAngles;
		xDeg = angles.x;
		yDeg = angles.y;

		desiredDistance = distance;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	
	void LateUpdate()
	{
		// If either mouse buttons are down, let the mouse govern camera position
		if (GUIUtility.hotControl == 0)
		{
			if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
			{
				xDeg += Input.GetAxis ("Mouse X") * xSpeed * 0.02f;
				yDeg -= Input.GetAxis ("Mouse Y") * ySpeed * 0.02f;
			}
		}
		
		// Set camera rotation
		Quaternion rotation = Quaternion.Euler (yDeg, xDeg, 0);
		
		// Calculate the desired distance
		desiredDistance -= Input.GetAxis ("Mouse ScrollWheel") * Time.deltaTime * zoomSpeed * Mathf.Abs (desiredDistance);
		desiredDistance = Mathf.Clamp (desiredDistance, minDistance, maxDistance);

		// Calculate desired camera position
		Vector3 vTargetOffset = new Vector3 (0, -heightOffset, 0);
		Vector3 position = target.transform.position - (rotation * Vector3.forward * desiredDistance + vTargetOffset);
		
		// Keep within limits
		desiredDistance = Mathf.Clamp (desiredDistance, minDistance, maxDistance);
		
		// Recalculate position based on the new currentDistance
		position = target.transform.position - (rotation * Vector3.forward * desiredDistance + vTargetOffset);
		
		//Finally Set rotation and position of camera
		transform.rotation = rotation;
		transform.position = position;
		
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
	
}
