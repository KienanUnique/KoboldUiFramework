using System;
using Cysharp.Threading.Tasks;
using KoboldUi.Element.View;
using KoboldUi.Utils;

#if KOBOLD_ZENJECT_SUPPORT
using Zenject;
#elif KOBOLD_VCONTAINER_SUPPORT
using VContainer;
using VContainer.Unity;
#endif

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

        public UniTask SetState(EWindowState state)
        {
            UniTask animationTask;
            switch (state)
            {
                case EWindowState.Active:
                    animationTask = IsOpened ? View.ReturnFocus() : View.Open();
                    IsOpened = true;
                    IsInFocus = true;
                    OnOpen();
                    break;
                case EWindowState.NonFocused:
                    animationTask = View.RemoveFocus();
                    IsOpened = true;
                    IsInFocus = false;
                    OnFocusRemove();
                    break;
                case EWindowState.Closed:
                    animationTask = View.Close();
                    IsOpened = false;
                    IsInFocus = false;
                    OnClose();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            return animationTask;
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