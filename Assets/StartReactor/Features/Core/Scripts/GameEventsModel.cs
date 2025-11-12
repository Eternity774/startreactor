namespace StartReactor.Features.Core
{
    public class GameEventsModel : IGameEventsModel, IGameEventsRequestsModel
    {
        public event System.Action RestartRequested;

        public void RequestRestart()
        {
            RestartRequested?.Invoke();
        }
    }
}

