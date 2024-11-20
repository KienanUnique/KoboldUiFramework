using Cysharp.Threading.Tasks;

namespace KoboldUi.Element.Animations
{
    public interface IUiAnimation
    {
        UniTask Appear();
        UniTask AnimateFocusReturn();
        UniTask AnimateFocusRemoved();
        UniTask Disappear();
    }
}