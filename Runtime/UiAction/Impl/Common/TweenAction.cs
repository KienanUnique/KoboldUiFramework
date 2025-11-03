using Cysharp.Threading.Tasks;
using DG.Tweening;
using KoboldUi.UiAction.Pool;

namespace KoboldUi.UiAction.Impl.Common
{
    /// <summary>
    /// Plays a DOTween tween as a UI action and waits for completion.
    /// </summary>
    public class TweenAction : AUiAction
    {
        private Tween _tween;

        public TweenAction(IUiActionsPool pool) : base(pool)
        {
        }

        /// <summary>
        /// Assigns the tween to execute when the action starts.
        /// </summary>
        /// <param name="tween">Tween to control.</param>
        public void Setup(Tween tween)
        {
            tween.Pause();
            _tween = tween;
        }

        /// <inheritdoc />
        protected override UniTask HandleStart()
        {
            _tween.Play();
            return _tween.ToUniTask();
        }

        /// <inheritdoc />
        protected override void ReturnToPool()
        {
            _tween?.Kill();
            _tween = null;
            Pool.ReturnAction(this);
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            _tween?.Kill();
            _tween = null;
        }
    }
}