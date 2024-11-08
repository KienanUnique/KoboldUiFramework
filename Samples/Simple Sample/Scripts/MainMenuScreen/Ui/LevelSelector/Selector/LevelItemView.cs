using KoboldUi.Element.View;
using Samples.Simple_Sample.Scripts.Utils;
using TMPro;
using UnityEngine;

namespace Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.LevelSelector.Selector
{
    public class LevelItemView : AUiAnimatedView
    {
        [SerializeField] private TextMeshProUGUI name;
        [SerializeField] private GameObject lockedContainer;
        [SerializeField] private GameObject unlockedContainer;
        [SerializeField] private StarGroup[] stars;

        public void SetLevelData(LevelData levelData)
        {
            name.text = levelData.Name;
            
            lockedContainer.SetActive(!levelData.IsUnlocked);
            unlockedContainer.SetActive(levelData.IsUnlocked);

            for (var i = 0; i < stars.Length; i++)
            {
                var isAchieved = i < levelData.StarsCount;  
                stars[i].SetState(isAchieved);
            }
        }
    }
}