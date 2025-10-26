using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UIArchitecture.Usage
{
    public sealed class MainMenuButtonView : AppView<MainMenuButtonViewState>
    {
        public Button button;
        public GameObject lockedRoot;
        public GameObject unlockedRoot;
        protected override IEnumerator InternalInitialize(MainMenuButtonViewState viewState)
        {
            var internalState = (IMainMenuButtonViewState)viewState;
            lockedRoot.SetActive(viewState.IsLocked);
            unlockedRoot.SetActive(viewState.IsLocked == false);
            button.onClick.AddListener(internalState.OnClick);
            yield return null;
        }

        void OnDestroy(){
            button.onClick.RemoveAllListeners();
        }
    }
}