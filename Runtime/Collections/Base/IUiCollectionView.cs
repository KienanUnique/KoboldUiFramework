using UnityEngine;

namespace KoboldUi.Collections.Base
{
    /// <summary>
    /// Defines behaviour for views that can be hosted inside a UI collection.
    /// </summary>
    public interface IUiCollectionView
    {
        /// <summary>
        /// Assigns the transform used as the parent for the view's hierarchy.
        /// </summary>
        /// <param name="parent">Target parent transform.</param>
        void SetParent(Transform parent);

        /// <summary>
        /// Makes the view visible or interactive when entering the collection.
        /// </summary>
        void Appear();

        /// <summary>
        /// Hides the view while keeping it available for pooling.
        /// </summary>
        void Disappear();

        /// <summary>
        /// Permanently disposes of the view when it leaves the collection lifecycle.
        /// </summary>
        void Destroy();
    }
}
