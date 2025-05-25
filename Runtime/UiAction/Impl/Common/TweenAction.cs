using Cysharp.Threading.Tasks;
using DG.Tweening;
using KoboldUi.UiAction.Pool;

namespace KoboldUi.UiAction.Impl.Common
{
    public class TweenAction : AUiAction
    {
        private Tween _tween;

        public TweenAction(IUiActionsPool pool) : base(pool)
        {
        }

        public void Setup(Tween tween)
        {
            tween.Pause();
            _tween = tween;
        }

        protected override UniTask HandleStart()
        {
            _tween.Play();
            return _tween.ToUniTask();
        }

        protected override void ReturnToPool()
        {
            _tween?.Kill();
            _tween = null;
            Pool.ReturnAction(this);
        }

        public override void Dispose()
        {
            _tween?.Kill();
            _tween = null;
        }
    }
}