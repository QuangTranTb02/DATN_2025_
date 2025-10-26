using System;
using UnityScreenNavigator.Runtime.Core.Page;

namespace Project.UIArchitecture.Usage{
    internal sealed class GeneralTransitionService : ITransitionService
    {
        private readonly PageContainer m_pageContainer;

        private readonly string m_startUpResourceKey, m_mainMenuResourceKey, m_settingsResourceKey;

        private readonly Action<(string pageId, Page page)> m_startUpPresenterFactory;
        private readonly Action<(string pageId, Page page)> m_mainMenuPresenterFactory;
        private readonly Action<(string pageId, Page page)> m_settingsPresenterFactory;

        public GeneralTransitionService(
            PageContainer pageContainer,
            string startUpResourceKey,
            string mainMenuResourceKey,
            string settingsResourceKey,
            Action<(string pageId, Page page)> startUpPresenterFactory,
            Action<(string pageId, Page page)> mainMenuPresenterFactory,
            Action<(string pageId, Page page)> settingsPresenterFactory
        )
        {

            m_pageContainer = pageContainer;
            m_startUpResourceKey = startUpResourceKey;
            m_mainMenuResourceKey = mainMenuResourceKey;
            m_settingsResourceKey = settingsResourceKey;

            m_startUpPresenterFactory = startUpPresenterFactory;
            m_mainMenuPresenterFactory = mainMenuPresenterFactory;
            m_settingsPresenterFactory = settingsPresenterFactory;
        }

        public void ExecutePopCommand()
        {
            m_pageContainer.Pop(playAnimation: true);
        }

        public void MainMenuShown()
        {
            m_pageContainer.Push(resourceKey: m_mainMenuResourceKey, true, onLoad: m_mainMenuPresenterFactory);
        }

        public void OnApplicationStarted()
        {
            m_pageContainer.Push(resourceKey: m_startUpResourceKey, true, onLoad: m_startUpPresenterFactory);
        }

        public void SettingsShown()
        {
            m_pageContainer.Push(resourceKey: m_settingsResourceKey, true, onLoad: m_settingsPresenterFactory);
        }
    }
}