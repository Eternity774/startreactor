using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using StartReactor.Features.Gameplay;

namespace StartReactor.Features.Environment
{
	public class GameEnvironmentController : ControllerWithResultBase
	{
		public GameEnvironmentController(IControllerFactory controllerFactory) : base(controllerFactory)
		{
		}

		protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
		{
			await ExecuteAndWaitResultAsync<SequenceController>(cancellationToken);

			Complete();
		}
	}
}