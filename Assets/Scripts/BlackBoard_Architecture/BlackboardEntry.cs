using System;

namespace BlackboardSystem
{
    public class BlackboardEntry<TValue> : IEquatable<BlackboardEntry<TValue>>
    {
        public event System.Action<TValue> ValueChangeEvent;
        public readonly BlackboardKey Key;

        private TValue m_value;
        public TValue Value => m_value;

        internal BlackboardEntry(in BlackboardKey key) : this(in key, default){}

        internal BlackboardEntry(in BlackboardKey key, TValue value){
            Key = key;
            m_value = value;
        }

        public bool Equals(BlackboardEntry<TValue> other)
        {
            if(other == null) return false;
            return other.Key.Equals(Key);
        }

        public void SetValue(TValue value){
            m_value = value;
            ValueChangeEvent?.Invoke(value);
        }
    }
}