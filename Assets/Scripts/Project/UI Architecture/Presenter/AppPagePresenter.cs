using System;
using System.Collections;

namespace Project.UIArchitecture
{
    public abstract class AppPagePresenter<TPage, TRootView, TRootViewState> 
        : PagePresenter<TPage>
        where TPage : AppViewPage<TRootView, TRootViewState>
        where TRootView : AppView<TRootViewState>
        where TRootViewState : AppViewState, new()
    {
        private TRootViewState m_state;
        protected AppPagePresenter(TPage view) : base(view)
        {
        }

        protected sealed override IEnumerator ViewDidLoad(TPage view)
        {
            yield return base.ViewDidLoad(view);
            m_state = new TRootViewState();
            view.Setup(m_state);
            yield return ViewDidLoad(view, m_state);
        }

        protected sealed override IEnumerator ViewWillPushEnter(TPage view){
            yield return base.ViewWillPushEnter(view);
            yield return ViewWillPushEnter(view, m_state);
        }

        protected sealed override IEnumerator ViewWillPushExit(TPage view){
            yield return base.ViewWillPushExit(view);
            yield return ViewWillPushExit(view, m_state);
        }

        protected sealed override IEnumerator ViewWillPopEnter(TPage view){
            yield return base.ViewWillPopEnter(view);
            yield return ViewWillPopEnter(view, m_state);
        }

        protected sealed override IEnumerator ViewWillPopExit(TPage view){
            yield return base.ViewWillPopExit(view);
            yield return ViewWillPopExit(view, m_state);
        }

        protected sealed override IEnumerator ViewWillDestroy(TPage view){
            yield return base.ViewWillDestroy(view);
            yield return ViewWillDestroy(view, m_state);
        }

        protected sealed override void ViewDidPopEnter(TPage view)
        {
            base.ViewDidPopEnter(view);
            ViewDidPopEnter(view, m_state);
        }

        protected sealed override void ViewDidPushEnter(TPage view)
        {
            base.ViewDidPushEnter(view);
            ViewDidPushEnter(view, m_state);
        }

        protected sealed override void ViewDidPopExit(TPage view){
            base.ViewDidPopExit(view);
            ViewDidPopExit(view, m_state);
        }

        protected sealed override void ViewDidPushExit(TPage view){
            base.ViewDidPushExit(view);
            ViewDidPushExit(view, m_state);
        }

        #region Virtual Lifecycle
        protected virtual IEnumerator ViewDidLoad(TPage view, TRootViewState state){ yield break; }
        protected virtual IEnumerator ViewWillPushEnter(TPage view, TRootViewState state){ yield break; }
        protected virtual IEnumerator ViewWillPushExit(TPage view, TRootViewState state){ yield break; }
        protected virtual IEnumerator ViewWillPopEnter(TPage view, TRootViewState state){ yield break; }
        protected virtual IEnumerator ViewWillPopExit(TPage view, TRootViewState state){ yield break; }
        protected virtual IEnumerator ViewWillDestroy(TPage view, TRootViewState state){ yield break; }
        protected virtual void ViewDidPushEnter(TPage view, TRootViewState state){}
        protected virtual void ViewDidPushExit(TPage view, TRootViewState state){}
        protected virtual void ViewDidPopEnter(TPage view, TRootViewState state){}
        protected virtual void ViewDidPopExit(TPage view, TRootViewState state){}
        #endregion
    }
}