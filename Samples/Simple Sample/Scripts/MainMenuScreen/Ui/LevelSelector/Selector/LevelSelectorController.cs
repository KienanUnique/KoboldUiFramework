using KoboldUi.Element.Controller;
using Samples.Simple_Sample.Scripts.Services.LevelProgression;

namespace Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.LevelSelector.Selector
{
    public class LevelSelectorController : AUiController<LevelSelectorView>
    {
        private readonly ILevelProgressionService _levelProgressionService;

        public LevelSelectorController(ILevelProgressionService levelProgressionService)
        {
            _levelProgressionService = levelProgressionService;
        }

        protected override void OnOpen()
        {
            var collection = View.levelItemsCollection;
            collection.Clear();

            var progression = _levelProgressionService.Progression;
            foreach (var levelData in progression)
            {
                // TODO: add parameters logic
                collection.Create();
            }
        } 
    }
}