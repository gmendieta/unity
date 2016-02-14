using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using System.Text; // StringBuilder
#endif

using com.GMM.RandomUtils;

/// <summary>
/// Random Weight
/// </summary>
public class RandomWeightMono : MonoBehaviour 
{
	public bool updateSumEachRandom = false;
	[Range(0, 100)]
	public int[] weights;
	public int _sum = 0; // Private 
	public int WeightSum { get { return _sum; } }

	private bool _initialized = false;

	private void Awake()
	{
		UpdateWeightSum();
	}

	/// <summary>
	/// Updates the sum of all weights
	/// </summary>
	public bool UpdateWeightSum()
	{
		if (weights == null || weights.Length == 0)
		{
			Debug.LogWarning("[RandomWeight] weights == NULL");
			_initialized = false;
			return false;
		}

		_sum = 0;
		for (int i=0; i<weights.Length; ++i)
		{
			_sum += weights[i];
		}
		_initialized = true;
		return true;
	}

	/// <summary>
	/// Gets the weight with index index
	/// </summary>
	public int GetWeight(int index)
	{
		if (index >= 0 && index < weights.Length)
		{
			return weights[index];
		}
		return -1;
	}

	/// <summary>
	/// Set the weight with index index
	/// </summary>
	public bool SetWeight(int index, int weight)
	{
		if (index >= 0 && index < weights.Length)
		{
			weights[index] = weight;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Gets a copy of the array of weights
	/// </summary>
	public int[] GetWeights()
	{
		if (weights != null)
		{
			return (int[])weights.Clone();
		}
		return null;
	}

	/// <summary>
	/// Sets the array of weights and update weights sum
	/// </summary>
	public bool SetWeights(int[] weights)
	{
		if (weights != null)
		{
			this.weights = (int[])weights.Clone();
			UpdateWeightSum();
			return true;
		}
		return false;
	}

	/// <summary>
	/// Gets a Random
	/// </summary>
	public int GetRandom()
	{
		if (updateSumEachRandom == true || _initialized == false)
		{
			UpdateWeightSum();
		}
		// if weights == NULL RandomUtils.RandomWeight returns 0
		return RandomUtils.RandomWeight(weights, _sum);
	}

#if UNITY_EDITOR
	private const int ITERATIONS = 1000;

	public void TestRandomWeight()
	{
		int[] results = new int[weights.Length];
		for (int i = 0; i<ITERATIONS; ++i)
		{
			++results[GetRandom()];
		}

		StringBuilder str = new StringBuilder();

		str.AppendLine(string.Format("---- RandomWeights ({0} Iterations) ----", ITERATIONS));
		for (int i=0; i<results.Length; ++i)
		{
			str.AppendLine(string.Format("\t{0} --> {1}%", i, ((float)results[i] / (float)ITERATIONS * 100f)));
		}
		Debug.Log (str);
	}
#endif
}
