using UnityEngine;
using UnityEditor;
using System.Collections;

public class AlignWindow :  EditorWindow
{
    private bool x_position = true;
    private bool y_position = true;
    private bool z_position = true;

    private bool x_rotation = false;
    private bool y_rotation = false;
    private bool z_rotation = false;

    [MenuItem ("GMM/AlignWindow")]
    public static void ShowWindow ()
    {
        EditorWindow.GetWindow (typeof (AlignWindow));
    }

    void OnGUI ()
    {
        GUILayout.Label ("Align Selected Objects:", EditorStyles.boldLabel);        
        EditorGUILayout.Space ();        
                
        GUILayout.Label ("Align Position (World Space)");
        EditorGUILayout.BeginHorizontal ();
        x_position = GUILayout.Toggle (x_position, "X Position");
        y_position = GUILayout.Toggle (y_position, "Y Position");
        z_position = GUILayout.Toggle (z_position, "Z Position");
        EditorGUILayout.EndHorizontal ();

        EditorGUILayout.Space();

        GUILayout.Label ("Align Rotation (World Space)");
        EditorGUILayout.BeginHorizontal ();
        x_rotation = GUILayout.Toggle (x_rotation, "X Rotation");
        y_rotation = GUILayout.Toggle (y_rotation, "Y Rotation");
        z_rotation = GUILayout.Toggle (z_rotation, "Z Rotation");
        EditorGUILayout.EndHorizontal ();

        EditorGUILayout.BeginVertical ();

        EditorGUILayout.EndVertical ();
        if (GUILayout.Button ("Align"))
        {
            DoAlign ();
        }
    }

    private void DoAlign ()
    {
        GameObject[] selection = Selection.gameObjects;        
        for (int i = 0; i < selection.Length; ++i)
        {
            if (selection[i] != Selection.activeGameObject)
            {                
                DoAlign (selection[i].transform, Selection.activeGameObject.transform);
            }
        }
    }

    private void DoAlign (Transform dest, Transform src)
    {
        // GMM Undo
        Undo.RecordObject (dest, "Align");

        // GMM Position
        Vector3 _p = new Vector3 (dest.position.x, dest.position.y, dest.position.z);
        if (x_position == true) _p.x = src.position.x;
        if (y_position == true) _p.y = src.position.y;        
        if (z_position == true) _p.z = src.position.z;
        dest.position = _p;

        // GMM Rotation
        Vector3 _euler = dest.rotation.eulerAngles;
        if (x_rotation == true) _euler.x = src.rotation.eulerAngles.x;
        if (y_rotation == true) _euler.y = src.rotation.eulerAngles.y;
        if (z_rotation == true) _euler.z = src.rotation.eulerAngles.z;
        dest.rotation = Quaternion.Euler (_euler.x, _euler.y, _euler.z);
    }
}
