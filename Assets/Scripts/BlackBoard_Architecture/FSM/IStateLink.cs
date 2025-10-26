namespace BlackboardSystem.FSM{

    public interface IStateLink{
        bool TryGetNextState(Blackboard context, out IState nextState);
    }

    public sealed class EmptyStateLink : IStateLink
    {
        private EmptyStateLink() { }
        public static readonly IStateLink emptyStateLink = new EmptyStateLink();
        public bool TryGetNextState(Blackboard context, out IState nextState)
        {
            nextState = null;
            return false;
        }
    }
}