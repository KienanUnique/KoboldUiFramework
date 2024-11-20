using KoboldUi.Element.Controller;
using Samples.Simple_Sample.Scripts.Services.Scenes;
using SampleUnirx;
using UnityEngine;

namespace Samples.Simple_Sample.Scripts.Ui.LoadingWindow.LoadingIndicator
{
    public class LoadingIndicatorController : AUiController<LoadingIndicatorView>
    {
        private readonly IScenesService _levelsService;

        public LoadingIndicatorController(IScenesService levelsService)
        {
            _levelsService = levelsService;
        }

        public override void Initialize()
        {
            _levelsService.LoadingProgress.Subscribe(OnLoadingProgress).AddTo(View);
        }

        private void OnLoadingProgress(float progress)
        {
            var progressPercentage = (int) (progress * 100f);
            progressPercentage = Mathf.Clamp(progressPercentage, 0, 100);

            View.loadingProgressText.text = $"{progressPercentage}%";
        }
    }
}