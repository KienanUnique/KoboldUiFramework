using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KoboldUi.UiAction.Pool;

namespace KoboldUi.UiAction.Impl.Common
{
    public class ParallelAction : AUiAction
    {
        private IReadOnlyList<IUiAction> _actions;

        public ParallelAction(IUiActionsPool pool) : base(pool)
        {
        }

        public void Setup(IReadOnlyList<IUiAction> actions)
        {
            _actions = actions;
        }

        protected override void ReturnToPool()
        {
            _actions = null;
            Pool.ReturnAction(this);
        }

        protected override UniTask HandleStart()
        {
            if (_actions.Count == 0)
                return UniTask.CompletedTask;

            var tasks = new List<UniTask>();
            foreach (var uiAction in _actions)
                tasks.Add(uiAction.Start());

            return UniTask.WhenAll(tasks);
        }

        public override void Dispose()
        {
            foreach (var uiAction in _actions)
                uiAction.Dispose();

            _actions = null;
        }
    }
}