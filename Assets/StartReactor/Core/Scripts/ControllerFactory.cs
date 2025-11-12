using Playtika.Controllers;
using VContainer;

namespace StartReactor.Core
{
    public class ControllerFactory : IControllerFactory
    {
        private readonly IObjectResolver _container;

        public ControllerFactory(IObjectResolver container)
        {
            _container = container;
        }

        public IController Create<T>() where T : class, IController
        {
            var controller = _container.Resolve<T>();
            return controller;
        }
    }
}

