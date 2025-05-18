using KoboldUi.UiAction;

namespace KoboldUi.Element.View
{
    public class AUiSimpleView : AUiView
    {
        public sealed override IUiAction Open()
        {
            gameObject.SetActive(true);
            return base.Open();
        }
        
        public sealed override IUiAction ReturnFocus()
        {
            return base.ReturnFocus();
        }

        public sealed override IUiAction RemoveFocus()
        {
            return base.RemoveFocus();
        }

        public sealed override IUiAction Close()
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