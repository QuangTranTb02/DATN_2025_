using UnityEngine;
using TrapSystem.Savable;
namespace TrapSystem.Testing{

    class SerializableGenerator : MonoBehaviour{
        [SerializeField] SerializableGUID guid;
        [SerializeField] uint part1, part2, part3, part4;
        [SerializeField] bool toggleGen = false;
        void Update(){
            if(!toggleGen) return;

            Regenerate();
        }

        [ContextMenu(nameof(Regenerate))]
        void Regenerate(){
            guid = SerializableGUID.NewGuid();

            part1 = guid.Part1;
            part2 = guid.Part2;
            part3 = guid.Part3;
            part4 = guid.Part4;
        }
    }
}