namespace StartReactor.Features.Gameplay
{
    /// <summary>
    /// Interface for handling lose state logic (e.g., restarting current level).
    /// </summary>
    public interface ILoseStateHandler
    {
        void HandleLose();
    }
}

