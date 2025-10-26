using System.Collections;
using UnityEngine.UI;

namespace Project.UIArchitecture.Usage{
    public sealed class StartupView : AppView<StartupViewState>
    {
        [UnityEngine.SerializeField] Button startButton;
        protected override IEnumerator InternalInitialize(StartupViewState viewState)
        {
            yield return null;
            IStartupViewState internalState = viewState;
            startButton.onClick.AddListener(internalState.OnClicked);
        }

        void OnDestroy(){
            if(startButton != null){
                startButton.onClick.RemoveAllListeners();
            }
        }
    }
}