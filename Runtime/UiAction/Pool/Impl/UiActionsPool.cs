using System;
using System.Collections.Generic;
using DG.Tweening;
using KoboldUi.UiAction.Impl.Common;
using KoboldUi.UiAction.Impl.Service;
using KoboldUi.Windows;
using KoboldUi.WindowsStack;
using UnityEngine.Pool;
using Zenject;

namespace KoboldUi.UiAction.Pool.Impl
{
    public class UiActionsPool : IUiActionsPool, IDisposable, IInitializable
    {
        private const int DEFAULT_POOL_SIZE = 2;

        private readonly IWindowsStackHolder _windowsStackHolder;

        private ObjectPool<EmptyAction> _emptyActionPool;
        private ObjectPool<OpenPreviousWindowAction> _openPreviousWindowActionPool;
        private ObjectPool<OpenWindowAction> _openWindowActionPool;
        private ObjectPool<ParallelAction> _parallelActionPool;
        private ObjectPool<SimpleCallbackAction> _simpleCallbackActionPool;
        private ObjectPool<BackWindowAction> _tryBackWindowActionPool;
        private ObjectPool<TweenAction> _tweenActionPool;
        private ObjectPool<WaitInitializationAction> _waitInitializationActionPool;
        private ObjectPool<BackToWindowAction> _backToWindowActionPool;

        public UiActionsPool(IWindowsStackHolder windowsStackHolder)
        {
            _windowsStackHolder = windowsStackHolder;
        }


        public void Dispose()
        {
            _emptyActionPool?.Dispose();
            _parallelActionPool?.Dispose();
            _simpleCallbackActionPool?.Dispose();
            _tweenActionPool?.Dispose();
            _waitInitializationActionPool?.Dispose();
            _openPreviousWindowActionPool?.Dispose();
            _openWindowActionPool?.Dispose();
            _tryBackWindowActionPool?.Dispose();
        }

        public void Initialize()
        {
            _emptyActionPool =
                new ObjectPool<EmptyAction>(() => new EmptyAction(this), defaultCapacity: DEFAULT_POOL_SIZE);
            _parallelActionPool =
                new ObjectPool<ParallelAction>(() => new ParallelAction(this), defaultCapacity: DEFAULT_POOL_SIZE);
            _simpleCallbackActionPool = new ObjectPool<SimpleCallbackAction>(() => new SimpleCallbackAction(this),
                defaultCapacity: DEFAULT_POOL_SIZE);
            _tweenActionPool =
                new ObjectPool<TweenAction>(() => new TweenAction(this), defaultCapacity: DEFAULT_POOL_SIZE);
            _waitInitializationActionPool =
                new ObjectPool<WaitInitializationAction>(() => new WaitInitializationAction(this),
                    defaultCapacity: DEFAULT_POOL_SIZE);
            _openPreviousWindowActionPool = new ObjectPool<OpenPreviousWindowAction>(
                () => new OpenPreviousWindowAction(this, _windowsStackHolder), defaultCapacity: DEFAULT_POOL_SIZE);
            _openWindowActionPool =
                new ObjectPool<OpenWindowAction>(() => new OpenWindowAction(this, _windowsStackHolder),
                    defaultCapacity: DEFAULT_POOL_SIZE);
            _tryBackWindowActionPool = new ObjectPool<BackWindowAction>(
                () => new BackWindowAction(this, _windowsStackHolder), defaultCapacity: DEFAULT_POOL_SIZE);
            
            _backToWindowActionPool = new ObjectPool<BackToWindowAction>(
                () => new BackToWindowAction(this, _windowsStackHolder), defaultCapacity: DEFAULT_POOL_SIZE);
        }

        #region GetActions

        public void GetAction(out EmptyAction action)
        {
            action = _emptyActionPool.Get();
        }

        public void GetAction(out ParallelAction action, IReadOnlyList<IUiAction> actions)
        {
            action = _parallelActionPool.Get();
            action.Setup(actions);
        }

        public void GetAction(out SimpleCallbackAction action, Action callback)
        {
            action = _simpleCallbackActionPool.Get();
            action.Setup(callback);
        }

        public void GetAction(out TweenAction action, Tween tween)
        {
            action = _tweenActionPool.Get();
            action.Setup(tween);
        }

        public void GetAction(out WaitInitializationAction action, AWindowBase window)
        {
            action = _waitInitializationActionPool.Get();
            action.Setup(window);
        }

        public void GetAction(out OpenPreviousWindowAction action)
        {
            action = _openPreviousWindowActionPool.Get();
            action.Setup();
        }

        public void GetAction(out OpenWindowAction action, IWindow windowToOpen)
        {
            action = _openWindowActionPool.Get();
            action.Setup(windowToOpen);
        }

        public void GetAction(out BackWindowAction action)
        {
            action = _tryBackWindowActionPool.Get();
            action.Setup();
        }

        public void GetAction(out BackToWindowAction action, IWindow targetWindow)
        {
            action = _backToWindowActionPool.Get();
            action.Setup(targetWindow);
        }

        #endregion

        #region ReturnAction

        public void ReturnAction(EmptyAction action)
        {
            _emptyActionPool.Release(action);
        }

        public void ReturnAction(ParallelAction action)
        {
            _parallelActionPool.Release(action);
        }

        public void ReturnAction(SimpleCallbackAction action)
        {
            _simpleCallbackActionPool.Release(action);
        }

        public void ReturnAction(TweenAction action)
        {
            _tweenActionPool.Release(action);
        }

        public void ReturnAction(WaitInitializationAction action)
        {
            _waitInitializationActionPool.Release(action);
        }

        public void ReturnAction(OpenPreviousWindowAction action)
        {
            _openPreviousWindowActionPool.Release(action);
        }

        public void ReturnAction(OpenWindowAction action)
        {
            _openWindowActionPool.Release(action);
        }

        public void ReturnAction(BackWindowAction action)
        {
            _tryBackWindowActionPool.Release(action);
        }

        public void ReturnAction(BackToWindowAction action)
        {
            _backToWindowActionPool.Release(action);
        }

        #endregion
    }
}