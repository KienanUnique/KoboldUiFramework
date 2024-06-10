﻿namespace KoboldUiFramework.Element.Animations
{
    public interface IUiAnimation
    {
        void Appear();
        void AnimateFocusReturn();
        void AnimateFocusRemoved();
        void Disappear();
    }
}