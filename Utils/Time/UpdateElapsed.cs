using UnityEngine;
using System.Collections;

public class UpdateElapsed: MonoBehaviour 
{
    [Range(0, 100)]
    public float updateTime = 1f;

    private float _time = 0f;    

	protected virtual void Start () 
    {
        _time = updateTime;
	}
	
	// Update is called once per frame
	protected virtual void Update ()
    {
        _time += Time.unscaledDeltaTime;
        if (_time >= updateTime )
        {
            _time = 0f;
            MyUpdate ();
        }
	}

    protected virtual void MyUpdate ()
    {
        Debug.Log ("[UpdateElapsed] UpdateElapsed");
    }
}
