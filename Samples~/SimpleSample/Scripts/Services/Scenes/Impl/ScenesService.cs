using System;
using Cysharp.Threading.Tasks;
using KoboldUi.Services.WindowsService;
using Samples.Simple_Sample.Scripts.Ui.LoadingWindow;
using SampleUnirx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Samples.Simple_Sample.Scripts.Services.Scenes.Impl
{
    public class ScenesService : IScenesService, IDisposable
    {
        private readonly IProjectWindowsService _projectWindowsService;
        
        private readonly ReactiveProperty<float> _loadingProgress = new();
        private readonly ReactiveProperty<bool> _isLoadingCompleted = new(true);
        
        private AsyncOperation _loadingOperation;
        private IDisposable _updateLoadingDisposable;

        public IReactiveProperty<float> LoadingProgress => _loadingProgress;
        public IReactiveProperty<bool> IsLoadingCompleted => _isLoadingCompleted;

        public ScenesService(IProjectWindowsService projectWindowsService)
        {
            _projectWindowsService = projectWindowsService;
        }

        public void ReloadCurrentScene()
        {
            var currentScene = SceneManager.GetActiveScene().name;
            LoadScene(currentScene);
        }
        
        public void Dispose()
        {
            _loadingProgress?.Dispose();
            _updateLoadingDisposable?.Dispose();
        }
        
        private void LoadScene(string sceneName)
        {
            if(!_isLoadingCompleted.Value)
                return;

            _isLoadingCompleted.Value = false;
            
            _loadingProgress.Value = 0f;
            
            _projectWindowsService.OpenWindow<LoadingWindow>();
            
            _loadingOperation = SceneManager.LoadSceneAsync(sceneName);
            
            _loadingOperation.completed += OnLoadingCompleted;
            
            _updateLoadingDisposable = Observable.EveryUpdate().Subscribe(_ => OnUpdateDuringLoading());
        }
        
        private void OnUpdateDuringLoading()
        {
            _loadingProgress.Value = _loadingOperation.progress;
        }

        private void OnLoadingCompleted(AsyncOperation obj)
        {
            _loadingOperation.completed -= OnLoadingCompleted;
            _loadingOperation = null;
            
            _updateLoadingDisposable.Dispose();
            
            FakeLoading().Forget();

            return;

            async UniTask FakeLoading()
            {
                for (var i = 0f; i < 1; i += 0.01f)
                {
                    _loadingProgress.Value = i;
                    await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
                }

                _projectWindowsService.CloseWindow();
                _isLoadingCompleted.Value = true;
            }
        }
    }
}