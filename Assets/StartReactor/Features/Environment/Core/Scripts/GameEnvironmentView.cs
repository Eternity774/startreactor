using UnityEngine;

namespace StartReactor.Features.Environment
{
    public class GameEnvironmentView : MonoBehaviour
    {
        [SerializeField]
        private PlayfieldView _playfieldView;

        public PlayfieldView PlayfieldView => _playfieldView;
    }
}
