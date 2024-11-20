using KoboldUi.Interfaces;
using KoboldUi.Windows;
using Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.Settings.Settings;
using UnityEngine;

namespace Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.Settings
{
    public class SettingsWindow : AWindow, IPopUp
    {
        [SerializeField] private SettingsView settingsView;

        protected override void AddControllers()
        {
            AddController<SettingsController, SettingsView>(settingsView);
        }
    }
}