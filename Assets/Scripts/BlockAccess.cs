using UnityEngine;
using System.Collections;

public class BlockAccess : MonoBehaviour 
{
  public bool checkDomain = true;
  public bool fullURL = true;
  public string[] DomainList;
  public string message;
  private bool illegalCopy = true;
	
  private void Start()
  {
    if (Application.isWebPlayer && checkDomain)
    {
      int i = 0;
      for (i = 0; i < DomainList.Length; i++)
      {
        if (Application.absoluteURL == DomainList[i]){
          illegalCopy = false;
        }else if (Application.absoluteURL.Contains(DomainList[i]) && !fullURL) {
          illegalCopy = false;
        }
      }
    }
	else if(!Application.isWebPlayer)
	{
		illegalCopy = false;
	}

  }
	
  private void OnGUI()
  {
    if (illegalCopy)
    {    
      GUI.Label(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.5f - 10, 400, 32), message);
    }
    else
    {
      Application.LoadLevel(Application.loadedLevel + 1);
    }
  }
}