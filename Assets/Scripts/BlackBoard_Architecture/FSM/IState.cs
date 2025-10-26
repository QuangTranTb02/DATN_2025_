namespace BlackboardSystem.FSM
{
    public interface IState{
        void OnEnter();
        void OnExit();
        void Update();
        void FixedUpdate();
    }

    public sealed class EmptyState : IState
    {
        public readonly static IState emptyState = new EmptyState();
        public readonly static IStateLink[] emptyLinks = System.Array.Empty<IStateLink>();
        public void FixedUpdate(){}
        public void Update(){}
        public void OnEnter(){}
        public void OnExit(){}
    }
}