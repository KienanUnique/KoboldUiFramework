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

    public interface IUiCollection<out TView> : IUiCollectionBase<TView>, IUiFactory<TView>
        where TView : IUiView
    {
    }

    public interface IUiCollection<in TParam1, out TView> : IUiCollectionBase<TView>,
        IUiFactory<TParam1, TView>
        where TView : IUiView, IParametrizedView<TParam1>
    {
    }

    public interface IUiCollection<in TParam1, in TParam2, out TView> : IUiCollectionBase<TView>,
        IUiFactory<TParam1, TParam2, TView>
        where TView : IUiView, IParametrizedView<TParam1, TParam2>
    {
    }

    public interface IUiCollection<in TParam1, in TParam2, in TParam3, out TView> : IUiCollectionBase<TView>,
        IUiFactory<TParam1, TParam2, TParam3, TView>
        where TView : IUiView, IParametrizedView<TParam1, TParam2, TParam3>
    {
    }

    public interface IUiCollection<in TParam1, in TParam2, in TParam3, in TParam4, out TView> :
        IUiCollectionBase<TView>,
        IUiFactory<TParam1, TParam2, TParam3, TParam4, TView>
        where TView : IUiView, IParametrizedView<TParam1, TParam2, TParam3, TParam4>
    {
    }

    public interface IUiCollection<in TParam1, in TParam2, in TParam3, in TParam4, in TParam5, out TView> :
        IUiCollectionBase<TView>,
        IUiFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TView>
        where TView : IUiView, IParametrizedView<TParam1, TParam2, TParam3, TParam4, TParam5>
    {
    }

    public interface IUiCollection<in TParam1, in TParam2, in TParam3, in TParam4, in TParam5, in TParam6,
        out TView> : IUiCollectionBase<TView>,
        IUiFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TView>
        where TView : IUiView, IParametrizedView<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
    {
    }

    public interface IUiCollection<in TParam1, in TParam2, in TParam3, in TParam4, in TParam5, in TParam6, in TParam7,
        out TView> : IUiCollectionBase<TView>,
        IUiFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TView>
        where TView : IUiView, IParametrizedView<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>
    {
    }
}