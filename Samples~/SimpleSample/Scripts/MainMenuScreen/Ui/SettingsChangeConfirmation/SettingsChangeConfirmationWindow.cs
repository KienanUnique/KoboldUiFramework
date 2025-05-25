﻿using KoboldUi.Windows;
using Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.SettingsChangeConfirmation.SettingsChangeConfirmation;
using UnityEngine;

namespace Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.SettingsChangeConfirmation
{
    public class SettingsChangeConfirmationWindow : AWindow
    {
        [SerializeField] private SettingsChangeConfirmationView settingsChangeConfirmationView;
        
        protected override void AddControllers()
        {
            AddController<SettingsChangeConfirmationController, SettingsChangeConfirmationView>(settingsChangeConfirmationView);
        }
    }
}