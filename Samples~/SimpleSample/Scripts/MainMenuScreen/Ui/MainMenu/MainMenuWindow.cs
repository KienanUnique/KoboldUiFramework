using KoboldUi.Windows;
using Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.MainMenu.Menu;
using Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.MainMenu.Title;
using UnityEngine;

namespace Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.MainMenu
{
    public class MainMenuWindow : AWindow
    {
        [SerializeField] private MainMenuView mainMenuView;
        [SerializeField] private TitleView titleView;
        
        protected override void AddControllers()
        {
            AddController<MainMenuController, MainMenuView>(mainMenuView);
            AddController<TitleController, TitleView>(titleView);
        }
    }
}