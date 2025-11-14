using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using StartReactor.Features.Core;

namespace StartReactor.Core
{
	public class BootstrapController : RootController
	{
		public BootstrapController(IControllerFactory controllerFactory)
			: base(controllerFactory)
		{
		}

		protected override void OnStart()
		{
			FlowAsync().Forget();

			base.OnStart();
		}

		private async UniTask FlowAsync()
		{
			while (!CancellationToken.IsCancellationRequested)
			{
				await ExecuteAndWaitResultAsync<GameLoopController>(CancellationToken);
			}
		}
	}
}