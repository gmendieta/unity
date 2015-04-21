using UnityEngine;
using System.Collections;

public class ScreenSizeManager : MonoBehaviour 
{	
    public delegate void OnScreenSizeChangeDelegate (Vector2 v);
    public event OnScreenSizeChangeDelegate OnScreenSizeChange;

    private Vector2 currentResolution;

    void Start()
    {
        currentResolution = new Vector2(Screen.width, Screen.height);
    }	
	
	// GMM Control if the resolution changes
	void Update () 
    {
        if (currentResolution.x != Screen.width || currentResolution.y != Screen.height)
        {
            currentResolution = new Vector2 (Screen.width, Screen.height);
            if (OnScreenSizeChange != null)
            {
                OnScreenSizeChange (currentResolution);
            }
        }
	}
}
