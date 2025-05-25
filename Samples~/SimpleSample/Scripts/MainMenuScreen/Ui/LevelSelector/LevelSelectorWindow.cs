using KoboldUi.Windows;
using Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.LevelSelector.Selector;
using UnityEngine;

namespace Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.LevelSelector
{
    public class LevelSelectorWindow : AWindow
    {
        [SerializeField] private LevelSelectorView levelSelectorView;
        
        protected override void AddControllers()
        {
            AddController<LevelSelectorController, LevelSelectorView>(levelSelectorView);
        }
    }
}