﻿using KoboldUi.Utils;
using UniRx;

namespace KoboldUi.Windows
{
    public interface IWindow
    {
        IReactiveProperty<bool> IsInitialized { get; }

        void SetState(EWindowState state);
        void SetAsLastSibling();
        void SetAsTheSecondLastSibling();
    }
}