namespace KoboldUi.Collections.Base
{
    public interface IUiFactory<out TView>
    {
        TView Create();
    }
}