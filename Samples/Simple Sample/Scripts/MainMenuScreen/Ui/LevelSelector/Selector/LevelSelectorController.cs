using KoboldUi.Element.Controller;
using KoboldUi.Services.WindowsService;
using Samples.Simple_Sample.Scripts.Services.LevelProgression;
using Samples.Simple_Sample.Scripts.Services.Scenes;
using UniRx;

namespace Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.LevelSelector.Selector
{
    public class LevelSelectorController : AUiController<LevelSelectorView>
    {
        private readonly ILevelProgressionService _levelProgressionService;
        private readonly ILocalWindowsService _localWindowsService;
        private readonly IScenesService _scenesService;

        public LevelSelectorController(
            ILevelProgressionService levelProgressionService, 
            ILocalWindowsService localWindowsService, 
            IScenesService scenesService
        )
        {
            _levelProgressionService = levelProgressionService;
            _localWindowsService = localWindowsService;
            _scenesService = scenesService;
        }

        public override void Initialize()
        {
            foreach (var cancelButton in View.cancelButtons)
                cancelButton.OnClickAsObservable().Subscribe(_ => OnCancelButtonPressed()).AddTo(View);

            View.loadButton.OnClickAsObservable().Subscribe(_ => OnLoadButtonPressed()).AddTo(View);
        }

        private void OnLoadButtonPressed()
        {
            _scenesService.ReloadCurrentScene(); // TODO: change this logic
        }

        private void OnCancelButtonPressed()
        {
            _localWindowsService.TryBackWindow();
        }

        protected override void OnOpen()
        {
            var collection = View.levelItemsCollection;
            collection.Clear();

            var progression = _levelProgressionService.Progression;
            foreach (var levelData in progression)
            {
                // TODO: add parameters logic
                var item = collection.Create();
                item.SetLevelData(levelData);
            }
        } 
    }
}