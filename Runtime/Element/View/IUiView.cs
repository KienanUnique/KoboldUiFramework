using Cysharp.Threading.Tasks;
using UnityEngine;

namespace KoboldUi.Element.View
{
    public interface IUiView
    {
        UniTask Open();
        UniTask ReturnFocus();
        UniTask RemoveFocus();
        UniTask Close();
        void CloseInstantly();
        void SetParent(Transform parent);
        void Destroy();
    }
}