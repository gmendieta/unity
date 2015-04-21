using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ButtonOnScroll is exactly as a Button from UI but it catch the Events than a possible 
/// parent scroll need to work properly, and send them to it.
/// This way, we are able to control if the Button is going to Click even OnDrag
/// Also, ButtonOnScroll has the possibility of configure which is the maximum time that could be pressed
/// before simulate a Release
/// </summary>
public class ButtonOnScroll : ButtonElapsed, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool sendEvents = false;

    private Transform _transform = null;
    private List<IScrollHandler> parentIScrollHandler = new List<IScrollHandler> ();

    protected override void Start ()
    {
        base.Start ();
        _transform = this.transform;        
        SetIScrollableParents ();        
    }

    /// <summary>
    /// SetIScrollableParents
    /// Search the Scrollable parent gameObjects
    /// </summary>
    public void SetIScrollableParents ()
    {        
        parentIScrollHandler.Clear ();
        // GMM Buscamos padres que implementen IScrollHandler, es decir scroll
        Transform parent = _transform.parent;
        while (parent != null)
        {
            Component[] _c = parent.GetComponents<Component> ();
            for (int i = 0; i < _c.Length; ++i)
            {
                if (_c[i] is IScrollHandler)
                {
                    parentIScrollHandler.Add ((IScrollHandler)_c[i]);
                }
            }            
            parent = parent.parent;
        }
    }

    public virtual void OnBeginDrag (PointerEventData eventData)
    {
        SendEvent<IBeginDragHandler> ((parent) => { parent.OnBeginDrag (eventData); });
    }

    public virtual void OnDrag (PointerEventData eventData)
    {
        SendEvent<IDragHandler> ((parent) => { parent.OnDrag (eventData); });
    }

    public virtual void OnEndDrag (PointerEventData eventData)
    {
        SendEvent<IEndDragHandler> ((parent) => { parent.OnEndDrag (eventData); });
    }

    protected void SendEvent<T> (Action<T> action) where T : IEventSystemHandler
    {
        if (sendEvents == true)
        {
            foreach (IScrollHandler _component in this.parentIScrollHandler)
            {
                action ((T)(IEventSystemHandler)_component);
            }
        }        
    }

}
