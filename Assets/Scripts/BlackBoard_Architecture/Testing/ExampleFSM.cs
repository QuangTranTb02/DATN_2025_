using System.Collections.Generic;

namespace BlackboardSystem.FSM
{
    public class ExampleState : IState
    {
        [BBKey("testInt")] private BlackboardKey m_testIntKey;
        [BBKey("testFloat")] private BlackboardKey m_testFloatKey;
        int intValue;
        float floatValue;
        private readonly Blackboard m_context;

        public ExampleState(Blackboard context){
            m_context = context;
        }

        public void FixedUpdate()
        {
            throw new System.NotImplementedException();
        }

        public void OnEnter()
        {
            bool hasKey = m_context.TryGetValue<int>(m_testIntKey, out int value);
            if(hasKey == false){
                UnityEngine.Debug.Log($"Can't find {m_testIntKey} value");
            }
            else{
                intValue = value;
                UnityEngine.Debug.Log($"Found {m_testIntKey} value: {intValue}");
            }

            hasKey = m_context.TryGetValue<float>(m_testFloatKey, out float value2);
            if(hasKey == false){
                UnityEngine.Debug.Log($"Can't find {m_testFloatKey} value");
            }
            else{
                floatValue = value2;
                UnityEngine.Debug.Log($"Found {m_testFloatKey} value: {floatValue}");
            }
        }

        public void OnExit()
        {
            throw new System.NotImplementedException();
        }

        public void Update()
        {
            throw new System.NotImplementedException();
        }
    }
}