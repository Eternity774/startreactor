using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using StartReactor.Features.Environment;
using StartReactor.Features.UI;

namespace StartReactor.Features.Core
{
	public class GameLoopController : ControllerWithResultBase
	{
		private readonly GameModel _gameModel;

		public GameLoopController(
			IControllerFactory controllerFactory,
			GameModel gameModel)
			: base(controllerFactory)
		{
			_gameModel = gameModel;
		}

		protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
		{
			await _gameModel.Initialize(cancellationToken);
			cancellationToken.ThrowIfCancellationRequested();

			Execute<GameUIController>();
			await ExecuteAndWaitResultAsync<GameEnvironmentController>(cancellationToken);
		}
	}
}