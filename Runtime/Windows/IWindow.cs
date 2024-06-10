using KoboldUi.Utils;

namespace KoboldUi.Windows
{
    public interface IWindow
    {
        void SetState(EWindowState state);
        void SetAsLastSibling();
        void SetAsTheSecondLastSibling();
    }
}