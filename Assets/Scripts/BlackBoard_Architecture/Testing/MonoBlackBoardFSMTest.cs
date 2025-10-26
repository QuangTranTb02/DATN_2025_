using System.Collections;
using BlackboardSystem.FSM;
using UnityEngine;
namespace BlackboardSystem.Testing
{
    public class MonoBlackBoardFSMTest : MonoBehaviour{
        [SerializeField] Mapper mapper;
        BlackboardFSM fsm;
        Blackboard blackboard;
        IEnumerator Start(){
            yield return null;
            yield return null;
            blackboard = new Blackboard();
            mapper.MapDataTo(blackboard);
            fsm = new BlackboardFSM(blackboard);
            Initialize();
        }

        void Initialize(){
            IState state = new ExampleState(blackboard);
            fsm.AddState(state);
            fsm.Start(state);
        }
    }
}