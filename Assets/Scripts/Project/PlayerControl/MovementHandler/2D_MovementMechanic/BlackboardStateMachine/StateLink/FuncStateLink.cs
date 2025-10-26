using System;
using BlackboardSystem;
using BlackboardSystem.FSM;

namespace Project.PlayerControl.Movement
{
    internal sealed class FuncWrapStateLink : IStateLink
    {
        private Func<Blackboard, IState> m_func;
        public Func<Blackboard, IState> Condition {
            get => m_func;
            set{
                m_func = value ?? EmptyCondition;
            }
        }

        public FuncWrapStateLink(Func<Blackboard, IState> func)
        {
            Condition = func;
        }

        public bool TryGetNextState(Blackboard context, out IState nextState)
        {
            nextState = m_func.Invoke(context);
            return nextState != null;
        }

        private static IState EmptyCondition(Blackboard context){ return null; }
    }
}