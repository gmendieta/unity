using UnityEngine;
using System.Collections;

public class SimpleMono : MonoBehaviour 
{
	public RandomWeight myWeightedRandom;

	// Update is called once per frame
	void Update () 
	{
#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.Space))
		{
			myWeightedRandom.TestRandomWeight();
		}
	}
#endif
}
