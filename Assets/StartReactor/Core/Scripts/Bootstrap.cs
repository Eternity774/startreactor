using System;
using System.Threading;
using VContainer.Unity;

namespace StartReactor.Core
{
    public class Bootstrap : IStartable, IDisposable
    {
        private readonly BootstrapController _bootstrapController;

        private CancellationTokenSource _cancellationTokenSource = new();

        public Bootstrap(BootstrapController bootstrapController)
        {
            _bootstrapController = bootstrapController;
        }

        public void Start()
        {
            try
            {
                _bootstrapController.LaunchTree(_cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogError($"Can't start bootstrap controller {exception}");
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}

