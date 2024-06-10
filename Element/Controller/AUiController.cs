using KoboldUiFramework.Element.View;
using KoboldUiFramework.Utils;
using Zenject;

namespace KoboldUiFramework.Element.Controller
{
    public abstract class AUiController<TView> : IUIController, IInitializable where TView : IUiView
    {
        [Inject] protected readonly TView View;

        protected bool IsOpened { get; private set; }
        protected bool IsInFocus { get; private set; }
        
        public virtual void Initialize()
        {
        }

        public void SetState(EWindowState state)
        {
            switch (state)
            {
                case EWindowState.Active:
                    if(IsOpened)
                        View.ReturnFocus();
                    else
                        View.Open();
                    IsOpened = true;
                    IsInFocus = true;
                    OnOpen();
                    break;
                case EWindowState.NonFocused:
                    View.RemoveFocus();
                    IsOpened = true;
                    IsInFocus = false;
                    OnFocusRemove();
                    break;
                case EWindowState.Closed:
                    View.Close();
                    IsOpened = false;
                    IsInFocus = false;
                    OnClose();
                    break;
            }
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