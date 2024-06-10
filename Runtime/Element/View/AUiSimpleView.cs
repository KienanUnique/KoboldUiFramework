namespace KoboldUi.Element.View
{
    public class AUiSimpleView : AUiView
    {
        public sealed override void Open()
        {
            gameObject.SetActive(true);
            base.Open();
        }
        
        public sealed override void ReturnFocus()
        {
            base.ReturnFocus();
        }

        public sealed override void RemoveFocus()
        {
            base.RemoveFocus();
        }

        public sealed override void Close()
        {
            gameObject.SetActive(false);
            base.Close();
        }

        public sealed override void CloseInstantly()
        {
            base.CloseInstantly();
        }
    }
}