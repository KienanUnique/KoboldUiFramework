using Cysharp.Threading.Tasks;

namespace KoboldUi.UiAction.Impl
{
    public class EmptyAction : IUiAction
    {
        public UniTask Start()
        {
            return UniTask.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}