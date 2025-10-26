using System.Collections;
using UnityEngine.Assertions;
using UnityScreenNavigator.Runtime.Core.Page;

namespace Project.UIArchitecture
{
    /// <summary>
    /// Adapt screen Navigation and Presenter framework
    /// </summary>
    public abstract class AppViewPage<TRootView, TViewState> 
        : Page
        where TRootView : AppView<TViewState>
        where TViewState : AppViewState
    {
        [UnityEngine.SerializeField] TRootView m_rootView;
        public TRootView RootView => m_rootView;

        private TViewState m_viewState;
        private bool m_isInitialized;
        public void Setup(TViewState viewState){
            m_viewState = viewState;
        }

        public override IEnumerator WillPushEnter()
        {
            Assert.IsNotNull(m_rootView);
            Assert.IsNotNull(m_viewState);

            if(!m_isInitialized){
                yield return m_rootView.Initialize(m_viewState);
                m_isInitialized = true;
            }
        }

        #region LifeCycle
        // reference: https://github.com/Haruma-K/UnityScreenNavigator?tab=readme-ov-file#lifecycle-events 

        // Called just after this page is loaded.
        // public override IEnumerator Initialize() { yield break; }

        // Called just before this page is released.
        // public override IEnumerator Cleanup() { yield break; }

        // Called just before this page is displayed by the Push transition.
        // public override IEnumerator WillPushEnter() { yield break; }

        // Called just after this page is displayed by the Push transition.
        // public override void DidPushEnter() { }

        // Called just before this page is hidden by the Push transition.
        // public override IEnumerator WillPushExit() { yield break; }

        // Called just after this page is hidden by the Push transition.
        // public override void DidPushExit() { }

        // Called just before this page is displayed by the Pop transition.
        // public override IEnumerator WillPopEnter() { yield break; }

        // Called just after this page is displayed by the Pop transition.
        // public override void DidPopEnter() { }

        // Called just before this page is hidden by the Pop transition.
        // public override IEnumerator WillPopExit() { yield break; }

        // Called just after this page is hidden by the Pop transition.
        // public override void DidPopExit() { }
        #endregion
    }
}
