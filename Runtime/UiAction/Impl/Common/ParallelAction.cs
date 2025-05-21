using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace KoboldUi.UiAction.Impl.Common
{
    public class ParallelAction : IUiAction
    {
        private IReadOnlyList<IUiAction> _actions;

        public void Setup(IReadOnlyList<IUiAction> actions)
        {
            _actions = actions;
        }
        public UniTask Start()
        {
            if (_actions.Count == 0)
                return UniTask.CompletedTask;
            
            var tasks = new List<UniTask>();
            foreach (var uiAction in _actions) 
                tasks.Add(uiAction.Start());
            
            return UniTask.WhenAll(tasks);
        }

        public void Dispose()
        {
            foreach (var uiAction in _actions) 
                uiAction.Dispose();
            
            _actions = null;
        }
    }
}