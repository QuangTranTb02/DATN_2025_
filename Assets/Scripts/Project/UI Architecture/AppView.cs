using System;
using System.Collections;

namespace Project.UIArchitecture{

    /// <summary>
    /// This class presents as an UI view attached to game object
    /// </summary>
    /// <typeparam name="TViewState"></typeparam>
    public abstract class AppView<TViewState> : UnityEngine.MonoBehaviour
        where TViewState : AppViewState
    {
        private bool m_initialized = false;
        public IEnumerator Initialize(TViewState viewState){
            if(m_initialized == true) yield break;

            m_initialized = true;
            yield return InternalInitialize(viewState);
        }

        protected abstract IEnumerator InternalInitialize(TViewState viewState);
    }
}