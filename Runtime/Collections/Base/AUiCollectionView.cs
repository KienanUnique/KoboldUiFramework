using UnityEngine;

namespace KoboldUi.Collections.Base
{
    /// <summary>
    /// Base class for collection views that handles parenting and teardown.
    /// </summary>
    public abstract class AUiCollectionView : MonoBehaviour, IUiCollectionView
    {
        /// <inheritdoc />
        public abstract void Appear();
        /// <inheritdoc />
        public abstract void Disappear();

        /// <inheritdoc />
        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }

        /// <inheritdoc />
        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}