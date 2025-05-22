using System;
using System.Collections.Generic;
using DG.Tweening;
using KoboldUi.UiAction.Impl.Common;
using KoboldUi.UiAction.Impl.Service;
using KoboldUi.Windows;

namespace KoboldUi.UiAction.Pool
{
    public interface IUiActionsPool
    {
        void GetAction(out EmptyAction action);
        void GetAction(out ParallelAction action, IReadOnlyList<IUiAction> actions);
        void GetAction(out SimpleCallbackAction action, Action callback);
        void GetAction(out TweenAction action, Tween tween);
        void GetAction(out WaitInitializationAction action, AWindowBase window);
        void GetAction(out OpenPreviousWindowAction action);
        void GetAction(out OpenWindowAction action, IWindow windowToOpen);
        void GetAction(out TryBackWindowAction action);
        
        void ReturnAction(EmptyAction action);
        void ReturnAction(ParallelAction action);
        void ReturnAction(SimpleCallbackAction action);
        void ReturnAction(TweenAction action);
        void ReturnAction(WaitInitializationAction action);
        void ReturnAction(OpenPreviousWindowAction action);
        void ReturnAction(OpenWindowAction action);
        void ReturnAction(TryBackWindowAction action);
    }
}