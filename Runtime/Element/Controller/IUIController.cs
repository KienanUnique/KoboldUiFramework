using Cysharp.Threading.Tasks;
using KoboldUi.Utils;

namespace KoboldUi.Element.Controller
{
    public interface IUIController
    {
        UniTask SetState(EWindowState state);
    }
}