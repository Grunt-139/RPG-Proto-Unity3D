using UnityEngine;
using System.Collections;

public class DestroyOnTouch : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnTriggerEnter(Collider other)
	{
		Character chara = other.GetComponent<Character>();

		if(chara != null)
		{
			chara.ApplyDamage(chara.GetHealth());
		}
		else
		{
			Destroy(other.gameObject);
		}
	}
}
