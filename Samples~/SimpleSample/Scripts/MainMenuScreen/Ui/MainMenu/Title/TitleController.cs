using DG.Tweening;
using KoboldUi.Element.Controller;

namespace Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.MainMenu.Title
{
    public class TitleController : AUiController<TitleView>
    {
        private Tween _animationTween;

        protected override void OnOpen()
        {
            _animationTween?.Kill();

            _animationTween = View.container.DOPunchScale(View.scalePunch, View.duration, View.vibrato, View.elasticity)
                .SetEase(View.ease)
                .SetLoops(-1, LoopType.Restart)
                .SetLink(View.gameObject);
        }

        protected override void OnClose()
        {
            _animationTween?.Kill();
        }
    }
}