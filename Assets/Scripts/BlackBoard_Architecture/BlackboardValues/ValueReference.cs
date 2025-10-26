namespace BlackboardSystem{
    public sealed class ValueReference<TValue>{
        public event System.Action<TValue> ValueChangeEvent;
        private TValue m_value;
        public TValue Value => m_value;

        internal ValueReference(TValue value = default){
            m_value = value;
        }

        public void SetValueWithoutNotify(TValue value){
            m_value = value;
        }

        public void SetValue(TValue value){
            m_value = value;
            ValueChangeEvent?.Invoke(value);
        }
    }
}