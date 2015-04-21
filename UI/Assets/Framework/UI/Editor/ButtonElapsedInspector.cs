using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(ButtonElapsed))]
public class ButtonElapsedInspector : ButtonEditor
{
    private ButtonElapsed _target = null;
	
	protected override void OnEnable ()
    {
        _target = (ButtonElapsed)target;
        base.OnEnable ();
	}

    public override void OnInspectorGUI ()
    {
        _target.maxDownTime = EditorGUILayout.Slider ("Max Down Time", _target.maxDownTime, 0f, 5f);
        base.OnInspectorGUI ();
    }
}
