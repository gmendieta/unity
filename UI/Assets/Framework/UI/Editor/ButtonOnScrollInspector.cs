using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using System.Collections;

[CustomEditor(typeof(ButtonOnScroll))]
public class ButtonOnScrollInspector : ButtonEditor 
{
    private ButtonOnScroll _target = null;
    protected override void OnEnable ()
    {
        base.OnEnable ();
        _target = (ButtonOnScroll) target;
    }

    public override void OnInspectorGUI ()
    {
        _target.sendEvents = EditorGUILayout.Toggle ("Spread Events to Scrolls", _target.sendEvents);
        _target.maxDownTime = EditorGUILayout.Slider ("Max Down Time", _target.maxDownTime, 0f, 5f);
        base.OnInspectorGUI ();
    }
	
}
