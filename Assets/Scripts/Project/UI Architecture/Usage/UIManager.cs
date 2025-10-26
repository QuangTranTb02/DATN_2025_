namespace Project.UIArchitecture.Usage
{
    /// <summary>
    /// 
    /// </summary>//  
    internal sealed class UIManager{
        private ITransitionService m_TransitionService;
        // hide constructor
        private UIManager(){}

        public static bool IsInitialized {get;private set;}

        public class Builder{
            private readonly UIManager m_Instance;
            private Builder(){
                if(IsInitialized)
                {
                    throw new System.InvalidOperationException("UIManager is already initialized");
                }
                m_Instance = new UIManager();
            }
            public static Builder CreateBuilder(){
                return new Builder();
            }
            public UIManager Build(){
                IsInitialized = true;
                return m_Instance;
            } 

            public Builder BuildWith(ITransitionService transitionService){
                m_Instance.m_TransitionService = transitionService;
                return this;
            }
        }
    } 
}