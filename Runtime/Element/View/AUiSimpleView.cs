using Cysharp.Threading.Tasks;

namespace KoboldUi.Element.View
{
    public class AUiSimpleView : AUiView
    {
        public sealed override UniTask Open()
        {
            gameObject.SetActive(true);
            return base.Open();
        }
        
        public sealed override UniTask ReturnFocus()
        {
            return base.ReturnFocus();
        }

        public sealed override UniTask RemoveFocus()
        {
            return base.RemoveFocus();
        }

        public sealed override UniTask Close()
        {
            gameObject.SetActive(false);
            return base.Close();
        }

        public sealed override void CloseInstantly()
        {
            gameObject.SetActive(false);
        }
    }
}