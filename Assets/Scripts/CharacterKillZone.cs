using UnityEngine;
using System.Collections;

public class CharacterKillZone : MonoBehaviour {

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

		chara.ApplyDamage(chara.GetHealth());
	}
}
