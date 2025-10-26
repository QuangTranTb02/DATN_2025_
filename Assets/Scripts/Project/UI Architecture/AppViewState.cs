namespace Project.UIArchitecture{
    /// <summary>
    /// Represents a state of current view
    /// This will be configured in higher layer (Presenter layer) 
    /// vn: Cái này khá đỉnh, class này sẽ chứa state data cho lớp UI và lớp Presenter 
    /// Ví dụ: UI (lớp View) không trực tiếp xử lý các nút nhấn 
    /// mà nhờ thằng state này gửi event nhấn về cho lớp xử lý cao hơn (lớp Presenter)
    /// Lúc này lớp UI (View) chỉ set UI dựa vô state data
    /// </summary>
    public abstract class AppViewState : System.IDisposable
    {
        private bool m_disposed;
        public void Dispose()
        {
            if(m_disposed) return;

            InternalDispose();
            m_disposed = true;
        }

        protected abstract void InternalDispose();
    }
}