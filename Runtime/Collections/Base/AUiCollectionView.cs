using UnityEngine;

namespace KoboldUi.Collections.Base
{
    public abstract class AUiCollectionView : MonoBehaviour, IUiCollectionView
    {
        public abstract void Appear();
        public abstract void Disappear();

        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}