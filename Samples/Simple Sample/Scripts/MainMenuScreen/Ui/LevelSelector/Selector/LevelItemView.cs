using KoboldUi.Element.View;
using Samples.Simple_Sample.Scripts.Utils;
using TMPro;
using UnityEngine;

namespace Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.LevelSelector.Selector
{
    public class LevelItemView : AUiAnimatedView
    {
        [SerializeField] private TextMeshProUGUI name;

        public void SetLevelData(LevelData levelData)
        {
            name.text = levelData.Name;
        }
    }
}