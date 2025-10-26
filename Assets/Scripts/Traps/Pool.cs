using System;

namespace TrapSystem{
    internal sealed class Pool<T> : IDisposable{
        private readonly System.Collections.Generic.List<T> m_Container;
        private readonly bool m_isFixedPool;
        private readonly Func<T> m_constructor;
        private readonly Action<T> m_destructor;

        private int m_availableCount;

        private Pool(int maxSize, Func<T> constructor, Action<T> destructor, bool isLimitPool = false){
            if(maxSize <= 0) throw new ArgumentException("maxSize must be greater than 0");
            if(constructor == null || destructor == null) throw new ArgumentNullException("constructor and destructor must not be null");

            m_isFixedPool = isLimitPool;
            m_Container = new System.Collections.Generic.List<T>(maxSize);

            m_constructor = constructor;
            m_destructor = destructor;
        }
        internal static Pool<T> CreateLimitSizedPool(int maxSize, Func<T> constructor, Action<T> destructor){
            return new Pool<T>(maxSize, constructor, destructor, isLimitPool: true);
        }

        internal static Pool<T> CreateDynamicPool(Func<T> constructor, Action<T> destructor, int initialCapacity = 4){
            return new Pool<T>(initialCapacity, constructor, destructor, isLimitPool: false);
        }

        public void Fill(){
            int maxSize = m_Container.Capacity;
            m_availableCount = m_Container.Count;
            for(int i = m_availableCount; i < maxSize; ++i){
                m_Container.Add(m_constructor.Invoke());
                ++m_availableCount;
            }
        }

        public T Rent(){
            T obj; 
            // pool is empty
            if(m_availableCount == 0){
                if(m_isFixedPool) return default;
                else{
                    obj = m_constructor.Invoke();
                    m_Container.Add(obj);
                    ++m_availableCount;
                }
            }

            obj = m_Container[^m_availableCount];
            --m_availableCount;
            return obj;
        }

        public void Return(T obj){
            if(obj == null || m_Container.Contains(obj)) return;
            // pool is full
            if(m_availableCount == m_Container.Count){
                if(m_isFixedPool){
                    m_destructor.Invoke(obj);
                    return;
                }
                else{
                    m_Container.Add(obj);
                }
            }

            m_Container[m_availableCount] = obj;
            ++m_availableCount;
        }

        public void Dispose()
        {
            foreach (T obj in m_Container)
            {
                m_destructor.Invoke(obj);
            }
            m_Container.Clear();
        }
    }
}