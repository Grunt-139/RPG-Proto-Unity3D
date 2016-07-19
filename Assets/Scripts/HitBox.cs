using UnityEngine;
using System.Collections;

public class HitBox : MonoBehaviour 
{
	public int minDamage = 1;
	public int maxDamage = 5;
	
	void OnTriggerEnter(Collider other)
	{
		//otherwise we must have hit a character, so tell it to take damage.
		Character script = other.GetComponent<Character>();
		if(script!=null)
		{
			script.ApplyDamage(Random.Range(minDamage, maxDamage));
		}
	}
}
