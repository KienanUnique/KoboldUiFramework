using System.Collections.Generic;

namespace KoboldUi.Collections.Base
{
    public interface IUiCollection<out TView> : IEnumerable<TView>
        where TView : IUiCollectionView
    {
        int Count { get; }

        void Clear();
    }
}