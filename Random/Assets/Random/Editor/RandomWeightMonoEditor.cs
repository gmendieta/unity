using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RandomWeightMono))]
public class RandomWeightEditor : Editor
{
	private SerializedObject m_Object = null;

	public void OnEnable()
	{
		m_Object = new SerializedObject(target);
	}

	public override void OnInspectorGUI ()
	{
		EditorGUI.BeginChangeCheck();
		DrawDefaultInspector();
		if (EditorGUI.EndChangeCheck() == true)
		{
			(m_Object.targetObject as RandomWeightMono).UpdateWeightSum();
		}

		EditorGUILayout.Separator();

		if (GUILayout.Button(new GUIContent("Test Random Weight")) == true)
		{
			(m_Object.targetObject as RandomWeightMono).TestRandomWeight();
		}
	}
}
