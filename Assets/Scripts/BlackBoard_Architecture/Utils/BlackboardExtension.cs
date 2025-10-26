namespace BlackboardSystem
{
    public static class BlackboardExtension
    {
        public static BlackboardEntry<TValue> GetRefUnsafe<TValue>(this Blackboard blackboard, BlackboardKey key){
            return blackboard.GetReference<TValue>(key);
        }
        public static BlackboardEntry<TValue> GetRefUnsafe<TValue>(this Blackboard blackboard, string key){
            BlackboardKey bKey = blackboard.GetKey(key);
            return blackboard.GetReference<TValue>(bKey);
        }

        public static TValue GetValue<TValue>(this Blackboard blackboard, BlackboardKey key){
            blackboard.TryGetReference(key, out BlackboardEntry<TValue> refVal);
            if( refVal == null ) return default;
            return refVal.Value;
        }

        public static TValue GetValue<TValue>(this Blackboard blackboard, string key){
            blackboard.TryGetReference(key, out BlackboardEntry<TValue> refVal);
            if( refVal == null ) return default;
            return refVal.Value;
        }

        public static void SetValue<TValue>(this Blackboard blackboard, BlackboardKey key, TValue value){
            bool hasRef = blackboard.TryGetReference(key, out BlackboardEntry<TValue> refVal);
            if(hasRef) refVal.SetValue(value);
        }

        public static BlackboardKey RegisterOrGetKey<TValue>(this Blackboard blackboard, string key, TValue initValue = default){
            bool hasRef = blackboard.TryGetReference(key, out BlackboardEntry<TValue> refVal);
            if(hasRef) return refVal.Key;
            return blackboard.RegisterKey<TValue>(key, initValue);
        }

        public static BlackboardEntry<TValue> RegisterOrGetReference<TValue>(this Blackboard blackboard, string key, TValue initValue = default){
            bool hasRef = blackboard.TryGetReference(key, out BlackboardEntry<TValue> refVal);
            if(hasRef) return refVal;

            BlackboardKey bKey = blackboard.RegisterKey<TValue>(key, initValue); 
            return blackboard.GetReference<TValue>(bKey);
        }
    }
}