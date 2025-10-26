using System.Collections;

namespace Project.UIArchitecture.Usage
{
    public class StartupPresenter : AppPagePresenter<StartupPage, StartupView, StartupViewState>
    {
        private readonly ITransitionService m_transitionService;
        public StartupPresenter(StartupPage view, ITransitionService transitionService) : base(view)
        {
            m_transitionService = transitionService;
        }

        protected override IEnumerator ViewDidLoad(StartupPage view, StartupViewState state)
        {
            yield return null;
            state.ClickEvent += m_transitionService.MainMenuShown;
        }

        protected override IEnumerator ViewWillPopExit(StartupPage view, StartupViewState state)
        {
            state.ClickEvent -= m_transitionService.MainMenuShown;
            yield return null;
        }
    }
}