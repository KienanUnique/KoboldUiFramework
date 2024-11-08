using System.Collections.Generic;
using KoboldUi.Element.View;

namespace KoboldUi.Collections.Base
{
    public interface IUiCollectionBase<out TView> : IEnumerable<TView>
        where TView : IUiView
    {
        int Count { get; }

        void Clear();
    }
}