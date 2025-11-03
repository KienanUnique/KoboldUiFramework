namespace KoboldUi
{
    /// <summary>
    /// Provides an editor-only hook for components that can populate their references automatically.
    /// </summary>
    public interface IAutoFillable
    {
#if KOBOLD_ALCHEMY_SUPPORT && UNITY_EDITOR
        /// <summary>
        /// Populates serialized references using heuristics while running inside the editor.
        /// </summary>
        void AutoFill();
#endif
    }
}
