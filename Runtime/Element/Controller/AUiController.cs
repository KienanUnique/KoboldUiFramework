using System;
using KoboldUi.Element.View;
using KoboldUi.UiAction;
using KoboldUi.Utils;
using Zenject;

namespace KoboldUi.Element.Controller
{
    public abstract class AUiController<TView> : IUIController, IInitializable where TView : IUiView
    {
        [Inject] protected readonly TView View;

        protected bool IsOpened { get; private set; }
        protected bool IsInFocus { get; private set; }

        public virtual void Initialize()
        {
        }

        public IUiAction SetState(EWindowState state)
        {
            IUiAction uiAction;
            switch (state)
            {
                case EWindowState.Active:
                    uiAction = IsOpened ? View.ReturnFocus() : View.Open();
                    IsOpened = true;
                    IsInFocus = true;
                    OnOpen();
                    break;
                case EWindowState.NonFocused:
                    uiAction = View.RemoveFocus();
                    IsOpened = true;
                    IsInFocus = false;
                    OnFocusRemove();
                    break;
                case EWindowState.Closed:
                    uiAction = View.Close();
                    IsOpened = false;
                    IsInFocus = false;
                    OnClose();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            return uiAction;
        }

        public void CloseInstantly()
        {
            View.CloseInstantly();
        }

        protected virtual void OnOpen()
        {
        }

        protected virtual void OnClose()
        {
        }

        protected virtual void OnFocusRemove()
        {
        }
    }
}