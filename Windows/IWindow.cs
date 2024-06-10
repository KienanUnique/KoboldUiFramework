using KoboldUiFramework.Utils;

namespace KoboldUiFramework.Windows
{
    public interface IWindow
    {
        void SetState(EWindowState state);
        void SetAsLastSibling();
        void SetAsTheSecondLastSibling();
    }
}