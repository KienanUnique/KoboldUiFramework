using System;
using KoboldUi.Element.View;
using KoboldUi.UiAction;
using KoboldUi.UiAction.Pool;
using KoboldUi.Utils;
using Zenject;

namespace KoboldUi.Element.Controller
{
    /// <summary>
    /// Base controller that orchestrates view lifecycle transitions based on window state changes.
    /// </summary>
    /// <typeparam name="TView">View type controlled by the controller.</typeparam>
    public abstract class AUiController<TView> : IUIController, IInitializable where TView : IUiView
    {
        /// <summary>
        /// View instance managed by the controller.
        /// </summary>
        [Inject] protected readonly TView View;

        /// <summary>
        /// Indicates whether the view has been opened at least once.
        /// </summary>
        protected bool IsOpened { get; private set; }
        /// <summary>
        /// Indicates whether the view currently holds input focus.
        /// </summary>
        protected bool IsInFocus { get; private set; }

        /// <inheritdoc />
        public virtual void Initialize()
        {
        }

        /// <inheritdoc />
        public IUiAction SetState(EWindowState state, in IUiActionsPool pool)
        {
            IUiAction uiAction;
            switch (state)
            {
                case EWindowState.Active:
                    uiAction = IsOpened ? View.ReturnFocus(pool) : View.Open(pool);
                    IsOpened = true;
                    IsInFocus = true;
                    OnOpen();
                    break;
                case EWindowState.NonFocused:
                    uiAction = View.RemoveFocus(pool);
                    IsOpened = true;
                    IsInFocus = false;
                    OnFocusRemove();
                    break;
                case EWindowState.Closed:
                    uiAction = View.Close(pool);
                    IsOpened = false;
                    IsInFocus = false;
                    OnClose();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            return uiAction;
        }

        /// <inheritdoc />
        public void CloseInstantly()
        {
            View.CloseInstantly();
        }

        /// <summary>
        /// Called after the view has transitioned to the open state.
        /// </summary>
        protected virtual void OnOpen()
        {
        }

        /// <summary>
        /// Called after the view has transitioned to the closed state.
        /// </summary>
        protected virtual void OnClose()
        {
        }

        /// <summary>
        /// Called after the view has lost focus but remains open.
        /// </summary>
        protected virtual void OnFocusRemove()
        {
        }
    }
}