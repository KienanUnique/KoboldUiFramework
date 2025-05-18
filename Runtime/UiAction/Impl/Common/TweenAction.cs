using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace KoboldUi.UiAction.Impl.Common
{
    public class TweenAction : IUiAction
    {
        private Tween _tween;

        public UniTask Start()
        {
            _tween.Play();
            return _tween.ToUniTask();
        }

        public void Setup(Tween tween)
        {
            tween.Pause();
        }

        public void Dispose()
        {
            _tween?.Kill();
            _tween = null;
        }
    }
}