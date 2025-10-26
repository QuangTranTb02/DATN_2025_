using UnityEngine;

namespace TrapSystem
{
    public sealed class ParentLevelVictimConverter : IVictimConverter
    {
        private readonly int m_levelDepth;
        private readonly IVictimConverter m_wrappedConverter;

        public ParentLevelVictimConverter(IVictimConverter wrappedConverter, int levelDepth = 1){
            m_wrappedConverter = wrappedConverter ?? throw new System.ArgumentNullException(nameof(wrappedConverter));
            m_levelDepth = levelDepth <= 0 ? 1 : levelDepth;
        }
        public IVictim GetVictimFrom(GameObject victimObject)
        {
            Transform parent = victimObject.transform.parent;
            int depth = m_levelDepth - 1;

            while(parent != null && depth > 0){
                parent = parent.parent;
                --depth;
            }

            if(parent == null) {
                parent = victimObject.transform;
            }
            return m_wrappedConverter.GetVictimFrom(parent.gameObject);
        }
    }
}