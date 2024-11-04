using Cysharp.Threading.Tasks;

namespace KoboldUi.Element.View
{
    public interface IUiView
    {
        UniTask Open();
        UniTask ReturnFocus();
        UniTask RemoveFocus();
        UniTask Close();
        void CloseInstantly();
    }
}