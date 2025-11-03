using KoboldUi.UiAction;
using KoboldUi.UiAction.Pool;
using UnityEngine;

namespace KoboldUi.Element.Animations
{
    /// <summary>
    /// Base component for view animations that produce UI actions for lifecycle transitions.
    /// </summary>
    public abstract class AUiAnimationBase : MonoBehaviour, IUiAnimation
    {
        /// <inheritdoc />
        public abstract IUiAction Appear(in IUiActionsPool pool);
        /// <inheritdoc />
        public abstract IUiAction AnimateFocusReturn(in IUiActionsPool pool);
        /// <inheritdoc />
        public abstract IUiAction AnimateFocusRemoved(in IUiActionsPool pool);
        /// <inheritdoc />
        public abstract IUiAction Disappear(in IUiActionsPool pool);
        /// <inheritdoc />
        public abstract void DisappearInstantly();
    }
}