using Utils;

namespace Windows
{
    public interface IWindow
    {
        void SetState(EWindowState state);
        void SetAsLastSibling();
        void SetAsTheSecondLastSibling();
    }
}