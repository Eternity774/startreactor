using Playtika.Controllers;
using StartReactor.Core;
using StartReactor.Features.Environment;
using StartReactor.Features.Game;
using StartReactor.Features.Gameplay;
using StartReactor.Features.Presentation;
using StartReactor.Features.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace StartReactor.Features.Core
{
	public class RootLifetimeScope : LifetimeScope
	{
		[SerializeField] private GameView _gameView;

		protected override void Configure(IContainerBuilder builder)
		{
			builder.RegisterEntryPoint<Bootstrap>();
			builder.Register<IControllerFactory, ControllerFactory>(Lifetime.Scoped);
			builder.Register<BootstrapController>(Lifetime.Transient);
			builder.Register<GameLoopController>(Lifetime.Transient);

			builder.RegisterInstance(_gameView.GameEnvironmentView);
			builder.RegisterInstance<IPlayfieldView>(_gameView.GameEnvironmentView.PlayfieldView);
			builder.RegisterInstance(_gameView.UiView);

			builder.Register<ResourcesProvider>(Lifetime.Singleton);
			builder.Register<ILevelsProvider, LevelsProvider>(Lifetime.Singleton);
			builder.Register<ISequenceGenerator, SequenceGeneratorService>(Lifetime.Singleton);
			builder.Register<IGridFactory, GridFactory>(Lifetime.Singleton);
			builder.Register<IGridService, GridService>(Lifetime.Singleton);
			builder.Register<IPopupFactory, PopupFactory>(Lifetime.Singleton);

			builder.Register<GameModel>(Lifetime.Singleton);

			builder.Register<VisualFeedbackController>(Lifetime.Singleton);

			builder.Register<GameEnvironmentController>(Lifetime.Transient);
			builder.Register<GameStateController>(Lifetime.Transient);
			builder.Register<SequencePlaybackController>(Lifetime.Transient);
			builder.Register<InputPhaseController>(Lifetime.Transient);
			builder.Register<WinController>(Lifetime.Transient);
			builder.Register<WinPopupController>(Lifetime.Transient);
			builder.Register<GameUIController>(Lifetime.Transient);
		}
	}
}