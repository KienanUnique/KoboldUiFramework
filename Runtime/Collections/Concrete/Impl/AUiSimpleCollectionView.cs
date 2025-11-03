using KoboldUi.Collections.Base;

namespace KoboldUi.Collections.Concrete.Impl
{
    /// <summary>
    /// Simple collection view that toggles the GameObject on appear and disappear.
    /// </summary>
    public class AUiSimpleCollectionView : AUiCollectionView
    {
        /// <inheritdoc />
        public override void Appear()
        {
            gameObject.SetActive(true);
        }

        /// <inheritdoc />
        public override void Disappear()
        {
            gameObject.SetActive(false);
        }
    }
}