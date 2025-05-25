using KoboldUi.Windows;
using Samples.Simple_Sample.Scripts.Ui.LoadingWindow.LoadingIndicator;
using UnityEngine;

namespace Samples.Simple_Sample.Scripts.Ui.LoadingWindow
{
    public class LoadingWindow : AWindow
    {
        [SerializeField] private LoadingIndicatorView loadingIndicatorView;
        
        protected override void AddControllers()
        {
            AddController<LoadingIndicatorController, LoadingIndicatorView>(loadingIndicatorView);
        }
    }
}