using System;
using UnityEngine;

namespace TrapSystem.Test
{
    public class TrapInitializer : MonoBehaviour
    {
        [SerializeField] ScriptableTrapEventSource _scriptableTrapEventSource;
        private TrapSystemInitializer m_initializer;
        void Awake(){
            m_initializer = TrapSystemInitializer.CreateSystem();
            m_initializer.WithVictimConverter(CreateConverter());
            m_initializer.AddTrapSourceEvent(_scriptableTrapEventSource);
        }

        private IVictimConverter CreateConverter()
        {
            IVictimConverter currentLevelConverter = new CurrentLevelVictimConverter();
            IVictimConverter parentDepthConverter = new ParentLevelVictimConverter(new DeepLevelVictimConverter(ignoreCallback: CheckIfShouldIgnore), levelDepth: 1);
            IVictimConverter fallbackConverter = new FallbackVictimConverter(currentLevelConverter, parentDepthConverter);
            return fallbackConverter;
        }

        private bool CheckIfShouldIgnore(GameObject obj)
        {
            return false;   
        }

        void OnDestroy(){
            m_initializer.Dispose();
        }
    }
}
