using System.Collections.Generic;
using UnityEngine;

namespace TrapSystem
{
    /// <summary>
    /// a helper class tries to find victim from collider game object
    /// </summary>
    public interface IVictimConverter{
        public IVictim GetVictimFrom(GameObject victimObject);
    }

    internal sealed class NullVictimConverter : IVictimConverter
    {
        private NullVictimConverter() { } // hide constructor
        internal readonly static IVictimConverter nullConverter = new NullVictimConverter();
        public IVictim GetVictimFrom(GameObject _) => null;
    }

    /// <summary>
    /// this would try to retrieve others to find victim in order from top to bottom
    /// </summary>
    public sealed class FallbackVictimConverter : IVictimConverter
    {
        private readonly IVictimConverter[] m_victimConverters;

        public FallbackVictimConverter(params IVictimConverter[] victimConverters){
            m_victimConverters = victimConverters;
        }

        public IVictim GetVictimFrom(GameObject victimObject)
        {
            foreach(var converter in m_victimConverters){
                IVictim victim = converter.GetVictimFrom(victimObject);

                if(victim != null) return victim; // found victim from one of the converters
            }
            return null;
        }
    }
}