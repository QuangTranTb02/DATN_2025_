using BlackboardSystem;
using BlackboardSystem.FSM;

namespace Project.PlayerControl.Movement
{
    internal sealed class CompositeStateLink : IStateLink
    {
        private readonly IStateLink[] m_links;
            
        private CompositeStateLink(IStateLink[] links){
            m_links = links;
        }
        public static IStateLink CreateACompositeStateLink(params IStateLink[] links){
            if(links == null || links.Length == 0){
                return EmptyStateLink.emptyStateLink;
            }

            return new CompositeStateLink(links);
        }

        public bool TryGetNextState(Blackboard context, out IState nextState)
        {
            for(int i = 0; i < m_links.Length; ++i){
                if(m_links[i].TryGetNextState(context, out nextState)){
                    return true;
                }
            }

            nextState = null;
            return false;
        }
    }
}