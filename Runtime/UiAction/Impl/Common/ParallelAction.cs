using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KoboldUi.UiAction.Pool;
using UnityEngine.Pool;

namespace KoboldUi.UiAction.Impl.Common
{
    /// <summary>
    /// Runs multiple UI actions simultaneously and waits for all to complete.
    /// </summary>
    public class ParallelAction : AUiAction
    {
        private IReadOnlyList<IUiAction> _actions;
        private List<IUiAction> _ownedActions;

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

        internal void TakeOwnershipOfActions(List<IUiAction> actions)
        {
            if (!ReferenceEquals(_actions, actions))
                throw new System.InvalidOperationException("Parallel action can only own its configured action list.");
            _ownedActions = actions;
        }

        /// <inheritdoc />
        protected override void ReturnToPool()
        {
            ReleaseActions();
            Pool.ReturnAction(this);
        }

        /// <inheritdoc />
        protected override UniTask HandleStart()
        {
            if (_actions.Count == 0)
                return UniTask.CompletedTask;

            return RunActions();
        }

        private async UniTask RunActions()
        {
            var tasks = ListPool<UniTask>.Get();
            try
            {
                foreach (var uiAction in _actions)
                    tasks.Add(uiAction.Start());

                await UniTask.WhenAll(tasks);
            }
            finally
            {
                ListPool<UniTask>.Release(tasks);
                ReleaseActions();
            }
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            try
            {
                if (_actions == null) return;
                foreach (var uiAction in _actions)
                    uiAction.Dispose();
            }
            finally
            {
                ReleaseActions();
            }
        }

        private void ReleaseActions()
        {
            var ownedActions = _ownedActions;
            _ownedActions = null;
            _actions = null;
            if (ownedActions != null)
                ListPool<IUiAction>.Release(ownedActions);
        }
    }
}
