using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour 
{
	public int maxHitPoints = 100;
	public bool isPlayer = false;
	private int currentHealth;
	private Animator animController;
	private float hitDelay;
	private bool isDead = false;
	private bool audioPlayed = false;
	
	// Use this for initialization
	void Start () 
	{
		animController = GetComponent<Animator>();
		currentHealth = maxHitPoints;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(hitDelay<=0)
		{
			animController.SetBool("tookDamage",false);
		}
		else
		{
			hitDelay-=Time.deltaTime;	
		}
		
		if(currentHealth <= 0)
		{
			Die();	
		}
	}
	
	//The character has been hurt by something so apply the damage.
	public void ApplyDamage(int amount)
	{
	
		if(isPlayer)
		{
			AudioManager.Instance.PlayPlayerHit(transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEnemyHit(transform.position);
		}

		currentHealth -= amount;
		if(currentHealth <= 0)
		{
			hitDelay=3f;
			animController.SetBool("died",true);
		}
		else
		{
			hitDelay=0.1f;
			animController.SetBool("tookDamage",true);
		}
	}

	public int GetHealth()
	{
		return currentHealth;
	}

	public int GetMaxHealth()
	{
		return maxHitPoints;
	}
	
	private void Die()
	{

		if(isPlayer && !audioPlayed)
		{
			AudioManager.Instance.PlayDeath(transform.position);
			audioPlayed = true;
			HudGui.Instance.PlayerDied();
		}
		else if(!isPlayer && !audioPlayed)
		{
			AudioManager.Instance.PlayEnemyDeath(transform.position);
			audioPlayed = true;
		}

		isDead = true;
		if(hitDelay<=0)
		{
			if(!isPlayer)
			{
				EnemySpawner.Instance.PoolObject(gameObject);
			}
		}
	}

	public bool IsDead()
	{
		return isDead;
	}
}
