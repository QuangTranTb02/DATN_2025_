using System;
using System.Collections.Generic;
using System.Reflection;

namespace BlackboardSystem.FSM
{
    public class BlackboardFSM
    {
        private readonly Blackboard m_blackboard;
        private readonly Dictionary<IState, IStateLink[]> m_stateLinks;
        private static readonly BindingFlags s_flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        private IState m_currentState;
        private IStateLink[] m_links;

        public BlackboardFSM(Blackboard blackboard)
        {
            m_blackboard = blackboard;
            m_stateLinks = new Dictionary<IState, IStateLink[]>();
            m_currentState = EmptyState.emptyState;
            m_links = EmptyState.emptyLinks;
        }

        public void Start(IState initialState)
        {
            ChangeState(initialState);
        }

        public void AddState(IState state, params IStateLink[] links)
        {
            if(state == null){
                state = EmptyState.emptyState;
            }
            else{
                m_blackboard.InjectBlackboardTo(state, flags: s_flags);
                m_blackboard.InjectKeyTo(state, flags: s_flags);
            }

            if(links == null || links.Length == 0){
                links = EmptyState.emptyLinks;
            }
            else{
                for (int i = 0; i < links.Length; ++i)
                {
                    m_blackboard.InjectKeyTo(links[i], flags: s_flags);
                }
            }

            m_stateLinks.Add(state, links);
        }

        public void RemoveState(IState state)
        {
            m_stateLinks.Remove(state);
        }

        public void Update()
        {

            for (int i = 0; i < m_links.Length; ++i)
            {
                if (m_links[i].TryGetNextState(m_blackboard, out IState state))
                {
                    ChangeState(state);
                    break;
                }
            }

            m_currentState.Update();
        }

        public void FixedUpdate()
        {
            m_currentState.FixedUpdate();
        }

        private void ChangeState(IState state)
        {
            if (m_currentState == state) return;
            IStateLink[] links;
            if (m_stateLinks.ContainsKey(state) == false)
            {
                state = EmptyState.emptyState;
                links = EmptyState.emptyLinks;
            }
            else
            {
                links = m_stateLinks[state];
            }

            UnityEngine.Debug.Log($"Changing state from {m_currentState} to {state}");
            m_currentState.OnExit();
            m_currentState = state;
            m_links = links;
            m_currentState.OnEnter();
        }
    }
}