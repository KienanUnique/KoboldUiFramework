using KoboldUi.Element.Controller;
using KoboldUi.Services.WindowsService;
using Samples.Simple_Sample.Scripts.Services.LevelProgression;
using Samples.Simple_Sample.Scripts.Services.Scenes;
using SampleUnirx;

namespace Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.LevelSelector.Selector
{
    public class LevelSelectorController : AUiController<LevelSelectorView>
    {
        private readonly ILevelProgressionService _levelProgressionService;
        private readonly ILocalWindowsService _localWindowsService;
        private readonly IScenesService _scenesService;

        private LevelItemView _selectedItem;

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
            
            var collection = View.levelItemsCollection;
            collection.Clear();

            var progression = _levelProgressionService.Progression;
            foreach (var levelData in progression)
            {
                var item = collection.Create();
                item.SetLevelData(levelData);
                item.SetSelectionState(false);
                item.OnClick.Subscribe(_ => OnItemClicked(item)).AddTo(View);
            }
        }

        protected override void OnOpen()
        {
            View.loadButton.interactable = false;

            if (_selectedItem == null) 
                return;
            
            _selectedItem.SetSelectionState(false);
            _selectedItem = null;
        }

        private void OnItemClicked(LevelItemView item)
        {
            var levelData = item.Data;
            
            if (!levelData.IsUnlocked)
                return;

            if(_selectedItem != null)
                _selectedItem.SetSelectionState(false);
            
            item.SetSelectionState(true);
            _selectedItem = item;

            View.loadButton.interactable = true;
        }

        private void OnLoadButtonPressed()
        {
            _scenesService.ReloadCurrentScene();
        }

        private void OnCancelButtonPressed()
        {
            _localWindowsService.CloseWindow();
        }
    }
}