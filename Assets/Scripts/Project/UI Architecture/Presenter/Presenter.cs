namespace Project.UIArchitecture
{
    /// <summary>
    /// A class acts as controller of a view
    /// </summary>
    /// <typeparam name="TPage"> The type of the view.</typeparam>
    public abstract class Presenter<TView> : System.IDisposable{
        
        public bool IsDisposed {get; private set;}
        public bool IsInitialized {get; private set;}
        protected readonly TView m_view;

        protected Presenter(TView view){
            m_view = view;
        }
        public void Initialize(){
            if(IsInitialized == true) return;
            if(IsDisposed == true) return;

            Initialize(m_view);
            IsInitialized = true;
        }
        public void Dispose(){
            if(IsInitialized == false) return;
            if(IsDisposed == true) return;

            Dispose(m_view);
        }

        protected abstract void Initialize(TView view);
        protected abstract void Dispose(TView view);
    }
}