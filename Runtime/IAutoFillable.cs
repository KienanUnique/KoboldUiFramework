namespace KoboldUi
{
    public interface IAutoFillable
    {
#if KOBOLD_ALCHEMY_SUPPORT && UNITY_EDITOR
        void AutoFill();
#endif
    }
}