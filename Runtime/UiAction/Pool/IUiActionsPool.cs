using System;
using System.Collections.Generic;
using DG.Tweening;
using KoboldUi.UiAction.Impl.Common;
using KoboldUi.UiAction.Impl.Service;
using KoboldUi.Services.WindowsService;
using KoboldUi.Windows;

namespace KoboldUi.UiAction.Pool
{
    /// <summary>
    /// Provides pooled UI actions to minimize allocations during window transitions.
    /// </summary>
    public interface IUiActionsPool
    {
        /// <summary>
        /// Retrieves an empty action from the pool.
        /// </summary>
        /// <param name="action">Action instance to use.</param>
        void GetAction(out EmptyAction action);

        /// <summary>
        /// Retrieves a parallel action configured with the supplied actions.
        /// </summary>
        /// <param name="action">Action instance to use.</param>
        /// <param name="actions">Actions to run concurrently.</param>
        void GetAction(out ParallelAction action, IReadOnlyList<IUiAction> actions);

        /// <summary>
        /// Retrieves a callback action configured with the supplied delegate.
        /// </summary>
        /// <param name="action">Action instance to use.</param>
        /// <param name="callback">Callback to invoke.</param>
        void GetAction(out SimpleCallbackAction action, Action callback);

        /// <summary>
        /// Retrieves a tween action configured with the supplied tween.
        /// </summary>
        /// <param name="action">Action instance to use.</param>
        /// <param name="tween">Tween to play.</param>
        void GetAction(out TweenAction action, Tween tween);

        /// <summary>
        /// Retrieves a wait-initialization action for the provided window.
        /// </summary>
        /// <param name="action">Action instance to use.</param>
        /// <param name="window">Window whose initialization should be awaited.</param>
        void GetAction(out WaitInitializationAction action, AWindowBase window);

        /// <summary>
        /// Retrieves an action that reopens the previous window.
        /// </summary>
        /// <param name="action">Action instance to use.</param>
        void GetAction(out OpenPreviousWindowAction action);

        /// <summary>
        /// Retrieves an action that opens the specified window.
        /// </summary>
        /// <param name="action">Action instance to use.</param>
        /// <param name="windowToOpen">Window to make active.</param>
        void GetAction(out OpenWindowAction action, IWindow windowToOpen, EPreviousWindowPolicy previousWindowPolicy);

        /// <summary>
        /// Retrieves an action that closes the current window.
        /// </summary>
        /// <param name="action">Action instance to use.</param>
        /// <param name="useBackLogicIgnorableChecks">When true, respects back-logic ignore flags.</param>
        void GetAction(out CloseWindowAction action, bool useBackLogicIgnorableChecks);

        /// <summary>
        /// Retrieves an action that closes windows until the target window is on top of the stack.
        /// </summary>
        /// <param name="action">Action instance to use.</param>
        /// <param name="targetWindow">Window to stop at.</param>
        /// <param name="useBackLogicIgnorableChecks">When true, respects back-logic ignore flags.</param>
        void GetAction(out CloseToWindowAction action, IWindow targetWindow, bool useBackLogicIgnorableChecks);

        /// <summary>
        /// Returns an empty action to the pool.
        /// </summary>
        void ReturnAction(EmptyAction action);

        /// <summary>
        /// Returns a parallel action to the pool.
        /// </summary>
        void ReturnAction(ParallelAction action);

        /// <summary>
        /// Returns a callback action to the pool.
        /// </summary>
        void ReturnAction(SimpleCallbackAction action);

        /// <summary>
        /// Returns a tween action to the pool.
        /// </summary>
        void ReturnAction(TweenAction action);

        /// <summary>
        /// Returns a wait-initialization action to the pool.
        /// </summary>
        void ReturnAction(WaitInitializationAction action);

        /// <summary>
        /// Returns an open-previous-window action to the pool.
        /// </summary>
        void ReturnAction(OpenPreviousWindowAction action);

        /// <summary>
        /// Returns an open-window action to the pool.
        /// </summary>
        void ReturnAction(OpenWindowAction action);

        /// <summary>
        /// Returns a close-window action to the pool.
        /// </summary>
        void ReturnAction(CloseWindowAction action);

        /// <summary>
        /// Returns a close-to-window action to the pool.
        /// </summary>
        void ReturnAction(CloseToWindowAction action);
    }
}
