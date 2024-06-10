namespace KoboldUi.Element.View
{
    public interface IUiView
    {
        void Open();
        void ReturnFocus();
        void RemoveFocus();
        void Close();
        void CloseInstantly();
    }
}