using UnityEngine;

namespace StartReactor.Features.Environment
{
    /// <summary>
    /// GameEnvironmentView is a container for PlayfieldView.
    /// This is the MonoBehaviour that holds the playfield view reference.
    /// </summary>
    public class GameEnvironmentView : MonoBehaviour
    {
        [SerializeField]
        private PlayfieldView _playfieldView;

        public PlayfieldView PlayfieldView => _playfieldView;
    }
}
