using KoboldUi.UiAction;
using UnityEngine;

namespace KoboldUi.Element.View
{
    public interface IUiView
    {
        IUiAction Open();
        IUiAction ReturnFocus();
        IUiAction RemoveFocus();
        IUiAction Close();
        void CloseInstantly();
        void SetParent(Transform parent);
        void Destroy();
    }
}