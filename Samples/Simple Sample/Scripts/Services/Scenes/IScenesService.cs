using UniRx;

namespace Samples.Simple_Sample.Scripts.Services.Scenes
{
    public interface IScenesService
    {
        IReactiveProperty<float> LoadingProgress { get; }
        IReactiveProperty<bool> IsLoadingCompleted { get; }
        
        void ReloadCurrentScene();
    }
}