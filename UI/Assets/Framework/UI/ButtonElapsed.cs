using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// ButtonElapsed has the possibility of configure which is the maximum time that could be pressed
/// before simulate a Release
/// </summary>
public class ButtonElapsed : Button
{
    public float maxDownTime = 0.25f;

    private float _elapsedTime = 0f;
    private bool _down = false; // Performance

    protected virtual void Start()
    {
        base.Start ();        
        this.onClick.AddListener (this.DoDebug);
    }
    
    public override void OnPointerDown (PointerEventData eventData)
    {
        base.OnPointerDown (eventData);
        _down = true;
        _elapsedTime = Time.unscaledTime;
    }

    protected virtual void Update()
    {        
        if (_down == true && ( Time.unscaledTime - _elapsedTime > maxDownTime ))
        {
            // GMM Force the transitions and state update
            this.OnPointerUp (new PointerEventData (EventSystem.current));            
            _down = false;
        }
    }

    public override void OnPointerUp (PointerEventData eventData)
    {
        _down = false;
        base.OnPointerUp (eventData);
    }

    public override void OnPointerClick (PointerEventData eventData)
    {
        // GMM only send the OnClick if Time is less than maxDownTime
        if (Time.unscaledTime - _elapsedTime < maxDownTime)
        {
            base.OnPointerClick (eventData);
        }
    }

    public void DoDebug ()
    {
        Debug.Log (string.Format("[ButtonExt] {0} Pressed", this.gameObject.name ));
    }
}
