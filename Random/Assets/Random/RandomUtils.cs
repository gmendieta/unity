using UnityEngine;
using System.Collections;


namespace com.GMM.RandomUtils
{
	/// <summary>
	/// RandomUtils
	/// Useful Random functions
	/// </summary>
	public static class RandomUtils
	{
		/// <summary>
		/// Generate a Random Index of an Array, based on Weights
		/// </summary>
		public static int RandomWeight(int[] weights)
		{
			#region Assert
			if (weights == null)
			{
				Debug.LogWarning("[RandomUtils] RandomIndependent weights == NULL");
				return 0;
			}
			if (weights.Length == 0)
			{
				Debug.LogWarning("[RandomUtils] RandomIndependent weights.Length == 0");
				return 0;
			}
			#endregion
			int sum = 0;
			for (int i=0; i<weights.Length; ++i)
			{
				sum += weights[i];
			}

			int r = Random.Range(0, sum);
			int index = 0;
			while (r >= weights[index])
			{
				r -= weights[index];
				++index;
			}
			return index;
		}

		/// <summary>
		/// Generate a Random Index of an Array, based on Weights
		/// </summary>
		public static int RandomWeight(int[] weights, int sum)
		{
			#region Assert
			if (weights == null)
			{
				Debug.LogWarning("[RandomUtils] RandomIndependent weights == NULL");
				return 0;
			}
			if (weights.Length == 0)
			{
				Debug.LogWarning("[RandomUtils] RandomIndependent weights.Length == 0");
				return 0;
			}
			if (sum <= 0)
			{
				Debug.LogWarning("[RandomUtils] RandomIndependent weights SUM <= 0");
				return 0;
			}
			#endregion
			int r = Random.Range(0, sum);
			int index = 0;
			while (r >= weights[index])
			{
				r -= weights[index];
				++index;
			}
			return index;
		}
	}
}