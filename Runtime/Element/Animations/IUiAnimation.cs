using KoboldUi.UiAction;
using KoboldUi.UiAction.Pool;

namespace KoboldUi.Element.Animations
{
    /// <summary>
    /// Supplies UI actions for animated transitions between view states.
    /// </summary>
    public interface IUiAnimation
    {
        /// <summary>
        /// Builds the animation used when a view appears.
        /// </summary>
        /// <param name="pool">Pool used to obtain reusable actions.</param>
        /// <returns>The action representing the appearance animation.</returns>
        IUiAction Appear(in IUiActionsPool pool);

        /// <summary>
        /// Builds the animation used when a view regains focus.
        /// </summary>
        /// <param name="pool">Pool used to obtain reusable actions.</param>
        /// <returns>The action representing the focus return animation.</returns>
        IUiAction AnimateFocusReturn(in IUiActionsPool pool);

        /// <summary>
        /// Builds the animation used when a view loses focus.
        /// </summary>
        /// <param name="pool">Pool used to obtain reusable actions.</param>
        /// <returns>The action representing the focus removal animation.</returns>
        IUiAction AnimateFocusRemoved(in IUiActionsPool pool);

        /// <summary>
        /// Builds the animation used when a view disappears.
        /// </summary>
        /// <param name="pool">Pool used to obtain reusable actions.</param>
        /// <returns>The action representing the disappearance animation.</returns>
        IUiAction Disappear(in IUiActionsPool pool);
    }
}
