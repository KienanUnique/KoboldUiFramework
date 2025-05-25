using UnityEngine;

namespace KoboldUi.Collections.Base
{
    public interface IUiCollectionView
    {
        void SetParent(Transform parent);
        void Appear();
        void Disappear();
        void Destroy();
    }
}