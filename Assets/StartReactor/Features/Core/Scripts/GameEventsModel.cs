namespace StartReactor.Features.Core
{
    /// <summary>
    /// GameEventsModel manages game events and requests for the Start Reactor game.
    /// It implements both IGameEventsModel and IGameEventsRequestsModel to handle event communication.
    /// </summary>
    public class GameEventsModel : IGameEventsModel, IGameEventsRequestsModel
    {
        public event System.Action RestartRequested;

        public void RequestRestart()
        {
            RestartRequested?.Invoke();
        }
    }
}

