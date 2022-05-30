﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using UnityEngine.SceneManagement;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace ConfirmPlaylistDifficulty.Configuration
{
    [IPA.Config.Stores.Attributes.NotifyPropertyChanges]
    internal class PluginConfig : INotifyPropertyChanged,IDisposable
    {
        public static PluginConfig Instance { get; set; }

        public PluginConfig()
        {
            OnReloaded += RefreshColor;
            OnReloaded += RefreshText;
            OnReloaded += RefreshCantClick;
            PropertyChanged += InvokedMethod;
        }

        private void InvokedMethod(object sender, PropertyChangedEventArgs e)
        {
            RefreshColor();
            RefreshText();
            RefreshCantClick();
        }

        public virtual bool ChangeColor { get; set; } = true;

        public virtual bool ChangeText { get; set; } = true;

        public virtual string WarnPlayButtonText { get; set; } = "(>_<)";
        public virtual string NormalPlayButtonText { get; set; } = "(⁎ᵕᴗᵕ⁎)";


        public virtual bool CantClick { get; set; } = true; 

        public event Action<PluginConfig> OnReloaded;
        public event PropertyChangedEventHandler PropertyChanged;

        // IPAによって自動的に呼び出される
        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (SceneManager.GetActiveScene().name == "PCInit") return;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// This is called whenever BSIPA reads the config from disk (including when file changes are detected).
        /// </summary>
        public virtual void OnReload()
        {
            // Do stuff after config is read from disk.

            if (SceneManager.GetActiveScene().name == "PCInit") return;

            this.OnReloaded?.Invoke(this);
        }

        /// <summary>
        /// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
        /// </summary>
        public virtual void Changed()
        {
            // Do stuff when the config is changed.
            if (SceneManager.GetActiveScene().name == "PCInit") return ;

            RefreshColor();
            RefreshText();
            RefreshCantClick();
        }

        /// <summary>
        /// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
        /// </summary>
        public virtual void CopyFrom(PluginConfig other)
        {
            // This instance's members populated from other
            this.ChangeColor = other.ChangeColor;
            this.ChangeText = other.ChangeText;
            this.WarnPlayButtonText = other.WarnPlayButtonText;
            this.NormalPlayButtonText = other.NormalPlayButtonText;
            this.CantClick = other.CantClick;
        }

        internal void RefreshColor()
        {
            if (ChangeColor)
            {
                DataModel.RefreshPlayButton();
                return;
            }

            DataModel.ChangePlayButtonColor(toWarning: false);
        }

        internal void RefreshText()
        {
            if (ChangeText)
            {
                DataModel.RefreshPlayButton();
                return;
            }

            DataModel.ChangePlayButtonText(toWarning: false);
        }

        internal void RefreshCantClick()
        {
            if (CantClick)
            {
                // プレイボタンの色をデフォルトに戻しておく
                DataModel.ChangePlayButtonColor(toWarning: false);

                DataModel.RefreshPlayButton();

                return;
            }

            DataModel.ChangePlayButtonInteractable(toWarning: false);

            // プレイボタンの色を変えたり変えなかったりするため
            DataModel.RefreshPlayButton();
        }

        internal void RefreshColor(PluginConfig pluginConfig)
        {
            RefreshColor();
        }

        internal void RefreshText(PluginConfig pluginConfig)
        {
            RefreshText();
        }

        internal void RefreshCantClick(PluginConfig pluginConfig)
        {
            RefreshCantClick();
        }

        public void Dispose()
        {
            OnReloaded -= RefreshColor;
            OnReloaded -= RefreshText;
            OnReloaded -= RefreshCantClick;
            PropertyChanged -= InvokedMethod;
        }
    }
}