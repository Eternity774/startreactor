using Playtika.Controllers;
using StartReactor.Core;
using StartReactor.Features.Environment;
using StartReactor.Features.Gameplay;
using StartReactor.Features.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace StartReactor.Features.Core
{
    /// <summary>
    /// RootLifetimeScope sets up the DI container for the game application, configuring essential components such as
    /// the entry point (Bootstrap), controllers, and services. This setup ensures efficient dependency management and lifecycle handling during gameplay.
    /// </summary>
    public class RootLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private GameView _gameView;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<Bootstrap>();
            builder.Register<IControllerFactory, ControllerFactory>(Lifetime.Scoped);
            builder.Register<BootstrapController>(Lifetime.Transient);
            builder.Register<GameLoopController>(Lifetime.Transient);

            builder.RegisterInstance(_gameView.EnvironmentView);
            
            // Register PlayfieldView as IPlayfieldView interface
            if (_gameView.EnvironmentView.PlayfieldView != null)
            {
                builder.RegisterInstance<IPlayfieldView>(_gameView.EnvironmentView.PlayfieldView);
            }
            
            builder.RegisterInstance(_gameView.UiView);

            builder.Register<GameModel>(Lifetime.Singleton);
            builder.Register<IGameEventsModel, IGameEventsRequestsModel, GameEventsModel>(Lifetime.Singleton);

            builder.Register<GameEnvironmentController>(Lifetime.Transient);
            builder.Register<SequenceController>(Lifetime.Transient);
            builder.Register<WinController>(Lifetime.Transient);
            builder.Register<LoseController>(Lifetime.Transient);

            builder.Register<GameUIController>(Lifetime.Transient);

            builder.Register<ResourcesProvider>(Lifetime.Singleton);
            builder.Register<LevelsProvider>(Lifetime.Singleton);
            builder.Register<IGridFactory, GridFactory>(Lifetime.Singleton);
            builder.Register<IGridLayoutConfigurator, GridLayoutConfigurator>(Lifetime.Singleton);
            
            builder.Register<IPopupFactory, PopupFactory>(Lifetime.Singleton);
            
            builder.Register<IWinStateHandler, WinStateHandler>(Lifetime.Singleton);
            builder.Register<ILoseStateHandler, LoseStateHandler>(Lifetime.Singleton);
        }
    }
}
