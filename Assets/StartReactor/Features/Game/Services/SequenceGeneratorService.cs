using System;
using System.Collections.Generic;

namespace StartReactor.Features.Game
{
	public class SequenceGeneratorService : ISequenceGenerator
	{
		private readonly Random _random = new();

		public List<int> GenerateSequence(int gridSize, int sequenceLength)
		{
			var sequence = new List<int>(sequenceLength);

			for (var i = 0; i < sequenceLength; i++)
			{
				var buttonIndex = _random.Next(0, gridSize);
				sequence.Add(buttonIndex);
			}

			return sequence;
		}
	}
}