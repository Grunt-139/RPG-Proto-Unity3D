using UnityEngine;
using System.Collections;

public class WaypointGizmo : MonoBehaviour 
{
  public float size = 1f;
  public Color color = Color.yellow;
  private Transform[] waypoints;	
  void OnDrawGizmos () 
  {
    waypoints = gameObject.GetComponentsInChildren<Transform>( );
    Vector3 last = waypoints[waypoints.Length-1].position;
    for (int i = 1; i < waypoints.Length; i++ )
    {
      Gizmos.color = color;
      Gizmos.DrawSphere( waypoints[i].position, size);
      Gizmos.DrawLine(last,waypoints[i].position);
      last = waypoints[i].position;
    }
  }
}
