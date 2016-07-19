using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {
	
	private static AudioManager instance = null;
	public static AudioManager Instance { get {return instance;}}
	
	//Sounds
	public AudioSource[] music;
	public AudioClip deathSound;
	public AudioClip enemyDeathSound;
	public AudioClip swordSwing;
	public AudioClip playerHitSound;
	public AudioClip enemyHitSound;
	public AudioSource earthquakeSound;

	private int curSong = 0;
	
	void Awake()
	{
	    if (instance != null && instance != this)
	    {
	      Destroy(this.gameObject);
	      return;        
	    } else {
	      instance = this;
	    }
	    DontDestroyOnLoad(this.gameObject);
	}
	
	// Use this for initialization
	void Start () 
	{
		music[curSong].Play();
	}
	
	// Update is called once per frame
	void Update () 
	{
//		if(!music[curSong].isPlaying)
//		{
//			curSong++;
//			if(curSong > music.Length)
//			{
//				curSong =0;
//			}
//			music[curSong].Play();
//		}
	}

	public void PlayDeath(Vector3 pos)
	{
		AudioSource.PlayClipAtPoint(deathSound,pos);
	}

	public void PlayEnemyDeath(Vector3 pos)
	{
		AudioSource.PlayClipAtPoint(enemyDeathSound,pos);
	}

	public void PlaySwordSwing(Vector3 pos)
	{
		AudioSource.PlayClipAtPoint(swordSwing,pos);
	}

	public void PlayEnemyHit(Vector3 pos)
	{
		AudioSource.PlayClipAtPoint(enemyHitSound,pos);
	}

	public void PlayPlayerHit(Vector3 pos)
	{
		AudioSource.PlayClipAtPoint(playerHitSound,pos);
	}

	public void PlayEarthquake()
	{
		if(!earthquakeSound.isPlaying)
		{
			earthquakeSound.Play();
		}
	}

	public void StopEarthquake()
	{
		earthquakeSound.Stop();
	}


}
