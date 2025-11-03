namespace KoboldUi.Collections.Base
{
    /// <summary>
    /// Creates view instances used by UI collections.
    /// </summary>
    /// <typeparam name="TView">Type of view produced by the factory.</typeparam>
    public interface IUiFactory<out TView>
    {
        /// <summary>
        /// Builds a new view instance ready to be attached to a collection.
        /// </summary>
        /// <returns>The created view.</returns>
        TView Create();
    }
}
