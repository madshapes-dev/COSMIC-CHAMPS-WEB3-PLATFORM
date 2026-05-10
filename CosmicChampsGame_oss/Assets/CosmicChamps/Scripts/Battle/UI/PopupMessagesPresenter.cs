using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using Zenject;

namespace CosmicChamps.Battle.UI
{
    public class PopupMessagesPresenter : MonoBehaviour
    {
        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
        [Inject]
        private PopupMessagePresenter.Factory _messageFactory;

        public void Display (string message)
        {
            async UniTaskVoid InternalDisplay ()
            {
                var localizedMessage = await new LocalizedString (Localization.UICaptionsTable, message)
                    .GetLocalizedStringAsync ();

                _messageFactory
                    .Create ()
                    .Display (localizedMessage);
            }

            InternalDisplay ().Forget ();
        }
        #endif
    }
}