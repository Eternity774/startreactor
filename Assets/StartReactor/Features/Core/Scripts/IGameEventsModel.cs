namespace StartReactor.Features.Core
{
    /// <summary>
    /// Interface for game events model that handles game event requests.
    /// </summary>
    public interface IGameEventsModel
    {
        event System.Action RestartRequested;
    }
}

