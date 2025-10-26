using UnityEngine;

namespace TrapSystem
{
    /// <summary>
    /// find the victim at current gameobject (not child or parent)
    /// </summary>
    public sealed class CurrentLevelVictimConverter : IVictimConverter
    {
        public IVictim GetVictimFrom(GameObject victimObject)
        {
            return victimObject.GetComponent<IVictim>();
        }
    }
}