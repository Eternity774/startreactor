using System.Collections.Generic;

namespace StartReactor.Features.Game
{
	public class SequenceState
	{
		public List<int> CurrentSequence { get; set; }
		public int CurrentInputIndex { get; private set; }
		public int CurrentSequenceIndex { get; set; }

		public SequenceState()
		{
			CurrentSequence = new List<int>();
			CurrentInputIndex = 0;
			CurrentSequenceIndex = 0;
		}

		public void Reset()
		{
			CurrentSequence.Clear();
			CurrentInputIndex = 0;
			CurrentSequenceIndex = 0;
		}

		public void ResetInput()
		{
			CurrentInputIndex = 0;
		}

		public void AdvanceInput()
		{
			CurrentInputIndex++;
		}
	}
}

