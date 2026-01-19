using Cysharp.Threading.Tasks;
using KoboldUi.Services.WindowsService;
using KoboldUi.UiAction.Pool;
using KoboldUi.Utils;
using KoboldUi.Windows;
using KoboldUi.WindowsStack;
using UnityEngine;

namespace KoboldUi.UiAction.Impl.Service
{
    /// <summary>
    /// Opens a window through the windows stack service, closing or defocusing previous entries as needed.
    /// </summary>
    public class OpenWindowAction : AUiAction
    {
        private readonly IWindowsStackHolder _windowsStackHolder;

        private IWindow _windowToOpen;
        private IWindow _previousWindow;
        private EPreviousWindowPolicy _previousWindowPolicy = EPreviousWindowPolicy.Default;

        public OpenWindowAction(
            IUiActionsPool pool,
            IWindowsStackHolder windowsStackHolder
        ) : base(pool)
        {
            _windowsStackHolder = windowsStackHolder;
        }

        /// <summary>
        /// Specifies which window should be opened.
        /// </summary>
        /// <param name="windowToOpen">Window to make active.</param>
        public void Setup(IWindow windowToOpen, EPreviousWindowPolicy previousWindowPolicy)
        {
            _windowToOpen = windowToOpen;
            _previousWindowPolicy = previousWindowPolicy;
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            _windowToOpen = null;
            _previousWindow = null;
            _previousWindowPolicy = EPreviousWindowPolicy.Default;
        }

        /// <inheritdoc />
        protected override UniTask HandleStart()
        {
            return _windowsStackHolder.IsOpened(_windowToOpen) ? UniTask.CompletedTask : OpenWindow();
        }

        /// <inheritdoc />
        protected override void ReturnToPool()
        {
            _windowToOpen = null;
            _previousWindow = null;
            _previousWindowPolicy = EPreviousWindowPolicy.Default;
            Pool.ReturnAction(this);
        }

        private async UniTask OpenWindow()
        {
            _previousWindow = _windowsStackHolder.IsEmpty ? null : _windowsStackHolder.CurrentWindow;

            switch (_previousWindowPolicy)
            {
                case EPreviousWindowPolicy.Default:
                    await HandleDefaultPolicy();
                    break;
                case EPreviousWindowPolicy.CloseAndForget:
                    await HandleCloseAndForgetPolicy();
                    break;
                case EPreviousWindowPolicy.CloseAfterOpenAndForget:
                    await HandleCloseAfterOpenAndForgetPolicy();
                    break;
                default:
                    Debug.LogError($"[Kobold Ui {nameof(OpenWindowAction)}] | Invalid EPreviousWindowPolicy: {_previousWindowPolicy}");
                    break;
            }
        }

        private async UniTask HandleDefaultPolicy()
        {
            if (_previousWindow != null)
            {
                var newState = _windowToOpen.IsPopup ? EWindowState.NonFocused : EWindowState.Closed;
                await _previousWindow.SetState(newState, Pool).Start();
            }

            await OpenNextWindow();
        }

        private async UniTask HandleCloseAndForgetPolicy()
        {
            if (_previousWindow != null)
            {
                await _previousWindow.SetState(EWindowState.Closed, Pool).Start();
                _windowsStackHolder.Remove(_previousWindow);
            }

            await OpenNextWindow();
        }

        private async UniTask HandleCloseAfterOpenAndForgetPolicy()
        {
            if (_previousWindow != null)
                await _previousWindow.SetState(EWindowState.NonFocused, Pool).Start();

            await OpenNextWindow();

            if (_previousWindow != null)
            {
                await _previousWindow.SetState(EWindowState.Closed, Pool).Start();
                _windowsStackHolder.Remove(_previousWindow);
            }
        }

        private async UniTask OpenNextWindow()
        {
            if (!_windowToOpen.IsInitialized)
                await _windowToOpen.WaitInitialization(Pool).Start();

            WindowsOrdersManager.HandleWindowAppearOnTop(_windowToOpen);
            _windowsStackHolder.Push(_windowToOpen);

            await _windowToOpen.SetState(EWindowState.Active, Pool).Start();
        }
    }
}
