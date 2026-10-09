namespace KoboldUi.Element.Controller
{
    public interface IUiViewReceiver<in TView>
    {
        void SetView(TView view);
    }
}
