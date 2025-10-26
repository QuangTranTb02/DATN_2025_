using System.Collections;

namespace Project.UIArchitecture.Usage{
    public sealed class PausePresenter : AppPagePresenter<PausePage, PauseMenuView, PauseMenuViewState>
    {
        private readonly ITransitionService m_transitionService;
        public PausePresenter(PausePage view, ITransitionService transitionService) : base(view)
        {
            m_transitionService = transitionService;
        }
        protected override IEnumerator ViewDidLoad(PausePage view, PauseMenuViewState state)
        {
            yield return null;
            // register continue button event
            //state.ContinueButtonState.ClickEvent += m_transitionService.GameContinued;
        }
        protected override void ViewDidPopExit(PausePage view, PauseMenuViewState state)
        {
            // TODO: send event that game will be continued
            //m_transitionService.GameContinued();
        }
    }
}