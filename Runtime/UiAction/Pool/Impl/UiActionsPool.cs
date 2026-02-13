using System;
using System.Collections.Generic;
using DG.Tweening;
using KoboldUi.UiAction.Impl.Common;
using KoboldUi.UiAction.Impl.Service;
using KoboldUi.Services.WindowsService;
using KoboldUi.Windows;
using KoboldUi.WindowsStack;
using UnityEngine.Pool;
using Zenject;

namespace KoboldUi.UiAction.Pool.Impl
{
    /// <summary>
    /// Maintains pools for the various UI action types used by the framework.
    /// </summary>
    public class UiActionsPool : IUiActionsPool, IDisposable, IInitializable
    {
        private const int DEFAULT_POOL_SIZE = 2;

        private readonly IWindowsStackHolder _windowsStackHolder;

        private ObjectPool<EmptyAction> _emptyActionPool;
        private ObjectPool<OpenPreviousWindowAction> _openPreviousWindowActionPool;
        private ObjectPool<OpenWindowAction> _openWindowActionPool;
        private ObjectPool<ParallelAction> _parallelActionPool;
        private ObjectPool<SimpleCallbackAction> _simpleCallbackActionPool;
        private ObjectPool<CloseWindowAction> _tryBackWindowActionPool;
        private ObjectPool<TweenAction> _tweenActionPool;
        private ObjectPool<WaitInitializationAction> _waitInitializationActionPool;
        private ObjectPool<CloseToWindowAction> _backToWindowActionPool;
        private ObjectPool<CloseAllWindowsAction> _closeAllWindowsActionPool;

        public UiActionsPool(IWindowsStackHolder windowsStackHolder)
        {
            _windowsStackHolder = windowsStackHolder;
        }


        /// <summary>
        /// Releases pooled action instances and their resources.
        /// </summary>
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
            _backToWindowActionPool?.Dispose();
            _closeAllWindowsActionPool?.Dispose();
        }

        /// <summary>
        /// Initializes object pools for the known action types.
        /// </summary>
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
            _tryBackWindowActionPool = new ObjectPool<CloseWindowAction>(
                () => new CloseWindowAction(this, _windowsStackHolder), defaultCapacity: DEFAULT_POOL_SIZE);
            
            _backToWindowActionPool = new ObjectPool<CloseToWindowAction>(
                () => new CloseToWindowAction(this, _windowsStackHolder), defaultCapacity: DEFAULT_POOL_SIZE);
            _closeAllWindowsActionPool = new ObjectPool<CloseAllWindowsAction>(
                () => new CloseAllWindowsAction(this, _windowsStackHolder), defaultCapacity: DEFAULT_POOL_SIZE);
        }

        #region GetActions

        /// <inheritdoc />
        public void GetAction(out EmptyAction action)
        {
            action = _emptyActionPool.Get();
        }

        /// <inheritdoc />
        public void GetAction(out ParallelAction action, IReadOnlyList<IUiAction> actions)
        {
            action = _parallelActionPool.Get();
            action.Setup(actions);
        }

        /// <inheritdoc />
        public void GetAction(out SimpleCallbackAction action, Action callback)
        {
            action = _simpleCallbackActionPool.Get();
            action.Setup(callback);
        }

        /// <inheritdoc />
        public void GetAction(out TweenAction action, Tween tween)
        {
            action = _tweenActionPool.Get();
            action.Setup(tween);
        }

        /// <inheritdoc />
        public void GetAction(out WaitInitializationAction action, AWindowBase window)
        {
            action = _waitInitializationActionPool.Get();
            action.Setup(window);
        }

        /// <inheritdoc />
        public void GetAction(out OpenPreviousWindowAction action)
        {
            action = _openPreviousWindowActionPool.Get();
            action.Setup();
        }

        /// <inheritdoc />
        public void GetAction(out OpenWindowAction action, IWindow windowToOpen, EPreviousWindowPolicy previousWindowPolicy)
        {
            action = _openWindowActionPool.Get();
            action.Setup(windowToOpen, previousWindowPolicy);
        }

        /// <inheritdoc />
        public void GetAction(out CloseWindowAction action, bool useBackLogicIgnorableChecks)
        {
            action = _tryBackWindowActionPool.Get();
            action.Setup(useBackLogicIgnorableChecks);
        }

        /// <inheritdoc />
        public void GetAction(out CloseToWindowAction action, IWindow targetWindow, bool useBackLogicIgnorableChecks)
        {
            action = _backToWindowActionPool.Get();
            action.Setup(targetWindow, useBackLogicIgnorableChecks);
        }

        /// <inheritdoc />
        public void GetAction(out CloseAllWindowsAction action, bool useBackLogicIgnorableChecks)
        {
            action = _closeAllWindowsActionPool.Get();
            action.Setup(useBackLogicIgnorableChecks);
        }

        #endregion

        #region ReturnAction

        /// <inheritdoc />
        public void ReturnAction(EmptyAction action)
        {
            _emptyActionPool.Release(action);
        }

        /// <inheritdoc />
        public void ReturnAction(ParallelAction action)
        {
            _parallelActionPool.Release(action);
        }

        /// <inheritdoc />
        public void ReturnAction(SimpleCallbackAction action)
        {
            _simpleCallbackActionPool.Release(action);
        }

        /// <inheritdoc />
        public void ReturnAction(TweenAction action)
        {
            _tweenActionPool.Release(action);
        }

        /// <inheritdoc />
        public void ReturnAction(WaitInitializationAction action)
        {
            _waitInitializationActionPool.Release(action);
        }

        /// <inheritdoc />
        public void ReturnAction(OpenPreviousWindowAction action)
        {
            _openPreviousWindowActionPool.Release(action);
        }

        /// <inheritdoc />
        public void ReturnAction(OpenWindowAction action)
        {
            _openWindowActionPool.Release(action);
        }

        /// <inheritdoc />
        public void ReturnAction(CloseWindowAction action)
        {
            _tryBackWindowActionPool.Release(action);
        }

        /// <inheritdoc />
        public void ReturnAction(CloseToWindowAction action)
        {
            _backToWindowActionPool.Release(action);
        }

        /// <inheritdoc />
        public void ReturnAction(CloseAllWindowsAction action)
        {
            _closeAllWindowsActionPool.Release(action);
        }

        #endregion
    }
}
