﻿using KoboldUiFramework.Utils;
using UnityEngine;
using Zenject;

namespace KoboldUiFramework.Windows
{
    public abstract class AWindowBase : MonoBehaviour, IWindow, IInitializable
    {
        public virtual void Initialize()
        {
        }
        
        public abstract void InstallBindings(DiContainer container);
        
        public abstract void SetState(EWindowState state);

        public void SetAsLastSibling()
        {
            transform.SetAsLastSibling();
        }

        public void SetAsTheSecondLastSibling()
        {
            var childCount = transform.childCount;
            transform.SetSiblingIndex(childCount - 1);
        }
    }
}