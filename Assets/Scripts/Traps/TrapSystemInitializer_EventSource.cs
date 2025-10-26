using System;
using System.Collections.Generic;

namespace TrapSystem{
    public sealed partial class TrapSystemInitializer{
        private int m_currentIndex;
        private readonly ITrapEventSource[] m_trapEventSources = new ITrapEventSource[4];
        public void AddTrapSourceEvent(ITrapEventSource trapEventSource){
            if(m_currentIndex >= m_trapEventSources.Length){
                throw new IndexOutOfRangeException(nameof(m_trapEventSources));
            }
            
            trapEventSource.TrapRegisteredEvent += m_internalSystem.OnTrapRegistered;
            trapEventSource.TrapUnregisteredEvent += m_internalSystem.OnTrapUnregistered;

            m_trapEventSources[m_currentIndex] = trapEventSource;
            ++m_currentIndex;
        }

        public void RemoveTrapSourceEvent(ITrapEventSource trapEventSource){
            if(m_currentIndex < 0){
                return;
            }

            trapEventSource.TrapRegisteredEvent -= m_internalSystem.OnTrapRegistered;
            trapEventSource.TrapUnregisteredEvent -= m_internalSystem.OnTrapUnregistered;
            
            for(int i = 0; i < m_currentIndex; i++){
                if(m_trapEventSources[i] == trapEventSource){
                    m_trapEventSources[i] = m_trapEventSources[m_currentIndex - 1];
                    m_trapEventSources[m_currentIndex - 1] = null;
                    --m_currentIndex;
                    break;
                }
            }
        }
    }
}