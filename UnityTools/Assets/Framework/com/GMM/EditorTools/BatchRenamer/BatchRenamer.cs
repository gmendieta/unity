using UnityEngine;
using UnityEditor;
using System.Collections;

public class BatchRenamer : ScriptableWizard
{
	public string nameFormat = "MyObject_{0:D2}";
	public int start = 0;
	public int increment = 1;

	[MenuItem("GMM/Batch Rename...")]
	static void CreateWizard()
	{
		ScriptableWizard.DisplayWizard("Batch Rename", typeof(BatchRenamer), "Rename");
	}

	//Called when the window first appears
	void OnEnable()
	{
		UpdateSelectionHelper();
	}

	// Function called when selection changes in scene
	void OnSelectionChange()
	{
		UpdateSelectionHelper();
	}

	// Update selection counter
	void UpdateSelectionHelper()
	{
		helpString = "";
		if (Selection.objects != null) 
		{
			helpString = "Number of objects selected: " + Selection.objects.Length;
		}
	}

	//Rename
	void OnWizardCreate()
	{
		if (Selection.objects == null)
		{
			return;
		}

		GameObject[] obArray = Selection.gameObjects;
		int count = start;
		for (int i=0; i<obArray.Length; ++i)
		{
			obArray[i].name = string.Format(nameFormat, count);
			count += increment;
		}
	}
}
