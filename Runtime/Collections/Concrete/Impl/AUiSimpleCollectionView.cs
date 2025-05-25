using KoboldUi.Collections.Base;

namespace KoboldUi.Collections.Concrete.Impl
{
    public class AUiSimpleCollectionView : AUiCollectionView
    {
        public override void Appear()
        {
            gameObject.SetActive(true);
        }

        public override void Disappear()
        {
            gameObject.SetActive(false);
        }
    }
}