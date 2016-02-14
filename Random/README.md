Random

RandomUtils - A simple static class useful for creating Weighted Random Integers.

Usage: 

RandomUtils.RandomWeight(int[] weights)
	Generates the Sum of all weights
	return a Random Integer within the Range [0, weights.Length)


RandomUtils.RandomWeight(int[] weights, int sum)
	Use sum as the Sum of all weights
	return a Random Integer within the Range [0, weights.Length)


RandomWeight
Have a look at attached Scene. 
It allows us to configure an array of Weights in Editor, updating internal weight sum.
It also has a couple of useful methods to ask it for a Random Integer within the Range [0, weights.Length)