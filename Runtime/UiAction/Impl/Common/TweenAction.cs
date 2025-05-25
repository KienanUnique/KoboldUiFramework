using Cysharp.Threading.Tasks;
using DG.Tweening;
using KoboldUi.UiAction.Pool;
using UnityEngine;

namespace KoboldUi.UiAction.Impl.Common
{
    // TODO: make this Sequence action. Because unitask override oncomplete callback
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
            Debug.Log($"@@@@ Tween: HandleStart");
            _tween.Play();
            return _tween.ToUniTask();
        }
        
        protected override void ReturnToPool()
        {
            Debug.Log($"@@@@ Tween: ReturnToPool");
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