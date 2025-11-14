using System.Collections.Generic;

namespace StartReactor.Features.Game
{
	public interface ISequenceGenerator
	{
		List<int> GenerateSequence(int gridSize, int sequenceLength);
	}
}