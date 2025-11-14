using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using StartReactor.Features.Game;

namespace StartReactor.Features.Environment
{
	public class GameEnvironmentController : ControllerWithResultBase
	{
		public GameEnvironmentController(IControllerFactory controllerFactory) : base(controllerFactory)
		{
		}

		protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
		{
			await ExecuteAndWaitResultAsync<GameStateController>(cancellationToken);

			Complete();
		}
	}
}