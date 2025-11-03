using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KoboldUi.UiAction.Pool;

namespace KoboldUi.UiAction.Impl.Common
{
    /// <summary>
    /// Runs multiple UI actions simultaneously and waits for all to complete.
    /// </summary>
    public class ParallelAction : AUiAction
    {
        private IReadOnlyList<IUiAction> _actions;

        public ParallelAction(IUiActionsPool pool) : base(pool)
        {
        }

        /// <summary>
        /// Configures the actions that should run in parallel.
        /// </summary>
        /// <param name="actions">Actions to execute together.</param>
        public void Setup(IReadOnlyList<IUiAction> actions)
        {
            _actions = actions;
        }

        /// <inheritdoc />
        protected override void ReturnToPool()
        {
            _actions = null;
            Pool.ReturnAction(this);
        }

        /// <inheritdoc />
        protected override UniTask HandleStart()
        {
            if (_actions.Count == 0)
                return UniTask.CompletedTask;

            var tasks = new List<UniTask>();
            foreach (var uiAction in _actions)
                tasks.Add(uiAction.Start());

            return UniTask.WhenAll(tasks);
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            foreach (var uiAction in _actions)
                uiAction.Dispose();

            _actions = null;
        }
    }
}