using KoboldUi.UiAction;
using KoboldUi.UiAction.Pool;

namespace KoboldUi.Element.View
{
    public class AUiSimpleView : AUiView
    {
        public sealed override IUiAction Open(in IUiActionsPool pool)
        {
            gameObject.SetActive(true);
            return base.Open(pool);
        }
        
        public sealed override IUiAction ReturnFocus(in IUiActionsPool pool)
        {
            return base.ReturnFocus(pool);
        }

        public sealed override IUiAction RemoveFocus(in IUiActionsPool pool)
        {
            return base.RemoveFocus(pool);
        }

        public sealed override IUiAction Close(in IUiActionsPool pool)
        {
            gameObject.SetActive(false);
            return base.Close(pool);
        }

        public sealed override void CloseInstantly()
        {
            gameObject.SetActive(false);
        }
    }
}