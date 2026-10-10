namespace KoboldUi.Composition
{
    /// <summary>Optional bridge to an application's object composition system.</summary>
    public interface IUiCompositionAdapter
    {
        void Inject(object instance);
        T Create<T>(params object[] arguments) where T : class;
    }
}
