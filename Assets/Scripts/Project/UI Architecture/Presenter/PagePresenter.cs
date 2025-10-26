using System.Collections;
using UnityScreenNavigator.Runtime.Core.Page;

namespace Project.UIArchitecture
{
    public abstract class PagePresenter<TPage> 
        : Presenter<TPage>, IPageLifecycleEvent
        where TPage : Page
    {
        protected PagePresenter(TPage view) : base(view){}

        IEnumerator IPageLifecycleEvent.Initialize()
        {
            yield return ViewDidLoad(m_view);
        }

        protected sealed override void Initialize(TPage view)
        {
            // The lifecycle event of the view will be added with priority 0.
            // Presenters should be processed after the view so set the priority to 1.
            view.AddLifecycleEvent(this, priority: 1);
        }

        public IEnumerator Cleanup() => ViewWillDestroy(m_view);

        public void DidPopEnter() => ViewDidPopEnter(m_view);

        public void DidPopExit() => ViewDidPopExit(m_view);

        public void DidPushEnter() => ViewDidPushEnter(m_view);

        public void DidPushExit() => ViewDidPushExit(m_view);

        public IEnumerator WillPopEnter() => ViewWillPopEnter(m_view);

        public IEnumerator WillPopExit() => ViewWillPopExit(m_view);

        public IEnumerator WillPushEnter() => ViewWillPushEnter(m_view);

        public IEnumerator WillPushExit() => ViewWillPushExit(m_view);
        

        protected override void Dispose(TPage view)
        {
            view.RemoveLifecycleEvent(this);
        }

        #region Virtual Lifecycle Methods
        protected virtual IEnumerator ViewDidLoad(TPage view) { yield break; }
        protected virtual IEnumerator ViewWillPushEnter(TPage view) { yield break; }
        protected virtual IEnumerator ViewWillPushExit(TPage view) { yield break; }
        protected virtual IEnumerator ViewWillPopEnter(TPage view) { yield break; }
        protected virtual IEnumerator ViewWillPopExit(TPage view) { yield break; }
        protected virtual IEnumerator ViewWillDestroy(TPage view) { yield break; }
        protected virtual void ViewDidPushEnter(TPage view) { }
        protected virtual void ViewDidPushExit(TPage view) { }
        protected virtual void ViewDidPopEnter(TPage view) { }
        protected virtual void ViewDidPopExit(TPage view) { }
        #endregion
    }
}