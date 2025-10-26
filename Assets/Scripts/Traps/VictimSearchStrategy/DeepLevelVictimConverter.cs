using System;
using UnityEngine;

namespace TrapSystem
{
    /// <summary>
    /// recursively find victim from children
    /// </summary>
    public sealed class DeepLevelVictimConverter : IVictimConverter
    {
        // this condition will filter out (ignore) invalid game objects for better search
        private readonly Func<GameObject, bool> m_ignoreCallback;

        public DeepLevelVictimConverter(Func<GameObject, bool> ignoreCallback = null){
            m_ignoreCallback = ignoreCallback;
        }

        public IVictim GetVictimFrom(GameObject victimObject)
        {
            return RecursivelyGetVictimFrom(victimObject.transform);
        }

        private IVictim RecursivelyGetVictimFrom(Transform currentTransform){
            bool shouldIgnoreThisObject = m_ignoreCallback != null && true == m_ignoreCallback.Invoke(currentTransform.gameObject);
            if(shouldIgnoreThisObject){
                return null;
            }
            int childCount = currentTransform.childCount;

            // base condition: this object is leaf child
            if(childCount == 0){
                return currentTransform.GetComponent<IVictim>();
            }

            // this object has victim component => no need to traverse the children
            if(currentTransform.TryGetComponent<IVictim>(out IVictim found)){
                return found;
            }
            for(int i = 0; i < childCount; ++i){
                found = RecursivelyGetVictimFrom(currentTransform.GetChild(i));
                if(found != null) return found;
            }
            return null;
        }
    }
}