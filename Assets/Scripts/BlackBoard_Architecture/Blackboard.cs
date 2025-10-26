using System;
using System.Collections.Generic;

namespace BlackboardSystem{
    public readonly struct BlackboardKey : IEquatable<BlackboardKey>{
        readonly string m_key;
        readonly int m_hashedKey;

        internal BlackboardKey(string key) : this (key, GetHashKey(key)){}

        private BlackboardKey(string key, int hashedKey){
            m_key = key;
            m_hashedKey = hashedKey;
        }

        internal readonly static BlackboardKey EmptyKey = new(string.Empty, -1);

        private static int GetHashKey(string key){
            uint hash = 2166136261;
            foreach(char c in key){
                hash = (hash ^ c) * 16777619;
            }
            return unchecked((int)hash);
        }

        public bool Equals(BlackboardKey other) => m_hashedKey == other.m_hashedKey;
        public override bool Equals(object obj) => obj is BlackboardKey other && Equals(other);
        public override int GetHashCode() => m_hashedKey;
        public override string ToString() => m_key;

        public static bool operator ==(BlackboardKey left, BlackboardKey right) => left.m_hashedKey == right.m_hashedKey;
        public static bool operator !=(BlackboardKey left, BlackboardKey right) => left.m_hashedKey != right.m_hashedKey;
        public static bool operator == (BlackboardKey left, int right) => left.m_hashedKey == right;
        public static bool operator !=(BlackboardKey left, int right) => left.m_hashedKey != right;
    }

    public sealed class Blackboard
    {
        private readonly Dictionary<string, BlackboardKey> m_keyRegistration;
        private readonly Dictionary<BlackboardKey, object> m_blackboard;

        public Blackboard(){
            m_keyRegistration = new Dictionary<string, BlackboardKey>();
            m_blackboard = new Dictionary<BlackboardKey, object>();
        }

        public BlackboardKey RegisterKey<TValue>(string key, TValue initValue = default)
        {
            if(m_keyRegistration.ContainsKey(key)) return m_keyRegistration[key];
            BlackboardKey bbKey = new(key);
            m_keyRegistration[key] = bbKey;
            m_blackboard[bbKey] = new BlackboardEntry<TValue>(bbKey, initValue);
            return bbKey;
        }

        public bool TryGetValue<TValue>(in BlackboardKey key, out TValue value)
        {
            bool hasRef = TryGetReference(key, out BlackboardEntry<TValue> entry);
            if(hasRef){
                value = entry.Value;
                return true;
            }
            value = default;
            return false;
        }

        public bool TryGetValue<TValue>(string key, out TValue value){
            if(m_keyRegistration.TryGetValue(key, out BlackboardKey bKey)){
                return TryGetValue<TValue>(bKey, out value);
            }
            value = default;
            return false;
        }

        public bool TryGetReference<TValue>(string key, out BlackboardEntry<TValue> value){
            if(m_keyRegistration.TryGetValue(key, out BlackboardKey bKey)){
                return TryGetReference<TValue>(bKey, out value);
            }
            value = default;
            return false;
        }

        public bool TryGetReference<TValue>(in BlackboardKey key, out BlackboardEntry<TValue> value)
        {
            if(m_blackboard.TryGetValue(key, out object obj)){
                if(obj is BlackboardEntry<TValue> entry){
                    value = entry;
                    return true;
                }
            }
            value = default;
            return false;
        }

        public bool TryGetKey(string key, out BlackboardKey bKey)
        {
            return m_keyRegistration.TryGetValue(key, out bKey);
        }

        internal BlackboardEntry<TValue> GetReference<TValue>(in BlackboardKey key)
        {
            return (BlackboardEntry<TValue>)m_blackboard[key];
        }

        internal BlackboardKey GetKey(string key) => m_keyRegistration[key];
    }
}