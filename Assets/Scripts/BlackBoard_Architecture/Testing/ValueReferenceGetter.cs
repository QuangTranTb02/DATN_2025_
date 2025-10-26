using UnityEngine;

namespace BlackboardSystem.Testing
{
    public class ValueReferenceGetter : MonoBehaviour{
        Blackboard m_blackBoard;
        BlackboardEntry<int> testInt;
        [SerializeField] int benchmarkCount = 10000;

        void Start(){
            m_blackBoard = new Blackboard();
            var key = m_blackBoard.RegisterKey<int>("testInt", 0);
            m_blackBoard.RegisterKey<float>("testFloat", 0.0f);
            testInt = m_blackBoard.GetRefUnsafe<int>(key);
        }

        void Update(){
            for(int i = 1; i < benchmarkCount; ++i){
                testInt.SetValue(i);
            }
        }
    } 
}