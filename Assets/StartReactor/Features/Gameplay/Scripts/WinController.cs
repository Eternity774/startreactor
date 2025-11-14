using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using StartReactor.Features.Core;
using StartReactor.Features.UI;

namespace StartReactor.Features.Gameplay
{
	public class WinController : ControllerWithResultBase
	{
		private readonly GameModel _gameModel;

		public WinController(
			IControllerFactory controllerFactory,
			GameModel gameModel)
			: base(controllerFactory)
		{
			_gameModel = gameModel;
		}

		protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
		{
			await ExecuteAndWaitResultAsync<WinPopupController>(cancellationToken);

			_gameModel.CompleteLevel();

			Complete();
		}
	}
}