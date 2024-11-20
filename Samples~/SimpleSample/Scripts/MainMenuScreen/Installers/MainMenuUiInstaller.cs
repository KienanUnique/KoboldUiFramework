using KoboldUi.Utils;
using Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.LevelSelector;
using Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.MainMenu;
using Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.Settings;
using Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.SettingsChangeConfirmation;
using Samples.Simple_Sample.Scripts.Services.Bootstrap;
using UnityEngine;
using Zenject;

namespace Samples.Simple_Sample.Scripts.MainMenuScreen.Installers
{
    [CreateAssetMenu(fileName = nameof(MainMenuUiInstaller), menuName = "Simple Sample/" + nameof(MainMenuUiInstaller), order = 0)]
    public class MainMenuUiInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private Canvas canvas;
        
        [Header("Windows")]
        [SerializeField] private MainMenuWindow mainMenuWindow;
        [SerializeField] private SettingsWindow settingsWindow;
        [SerializeField] private SettingsChangeConfirmationWindow settingsChangeConfirmationWindow;
        [SerializeField] private LevelSelectorWindow levelSelectorWindow;

        public override void InstallBindings()
        {
            var canvasInstance = Instantiate(canvas);
            
            Container.BindWindowFromPrefab(canvasInstance, mainMenuWindow);
            Container.BindWindowFromPrefab(canvasInstance, settingsWindow);
            Container.BindWindowFromPrefab(canvasInstance, settingsChangeConfirmationWindow);
            Container.BindWindowFromPrefab(canvasInstance, levelSelectorWindow);

            Container.BindInterfacesTo<Bootstrap>().AsSingle().NonLazy(); // TODO: move this logic to other place
        }
    }
}