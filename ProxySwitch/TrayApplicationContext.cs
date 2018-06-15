﻿//
// Copyright (c) 2018 Matthias Reiseder. All rights reserved.  
// Licensed under the MIT License. 
// See LICENSE file in the repository root for full license information.
//

using ProxySwitch.Controls;
using ProxySwitch.Properties;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace ProxySwitch
{
    internal sealed partial class TrayApplicationContext : ApplicationContext
    {
        #region Private fields

        private AboutDialog aboutDialog;
        private SettingsDialog settingsDialog;

        private Timer refreshIconTimer;

        #endregion

        #region Constructors

        public TrayApplicationContext()
        {
            InitializeComponent();

            aboutDialog = new AboutDialog();
            settingsDialog = new SettingsDialog();

            if (Settings.Instance.DisableProxyOnStart)
                RegistryService.Instance.DisableProxyServer();

            UpdateIcon();
            CreateRefreshIconTimer();
        }

        #endregion

        #region Event handler

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            RegistryService.Instance.ToggleProxyServer();
            UpdateIcon();
        }

        private void NotifyIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(notifyIcon, null);
            }
        }

        private void SettingsItem_Click(object sender, EventArgs e)
        {
            if (settingsDialog.ShowDialog() == DialogResult.OK)
            {

            }
        }

        private void AboutItem_Click(object sender, EventArgs e)
        {
            aboutDialog.ShowDialog();
        }

        private void ExitItem_Click(object sender, EventArgs e)
        {
            ExitThread();
        }

        private void RefreshIconTimer_Tick(object sender, EventArgs e)
        {
            UpdateIcon();
        }

        #endregion

        #region Methods

        protected override void ExitThreadCore()
        {
            refreshIconTimer.Stop();
            notifyIcon.Visible = false;

            base.ExitThreadCore();
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Creates and starts the refreshIconTimer.
        /// </summary>
        private void CreateRefreshIconTimer()
        {
            refreshIconTimer = new Timer
            {
                Enabled = false,
                Interval = Settings.Instance.RefreshInterval * 1000
            };

            refreshIconTimer.Tick += RefreshIconTimer_Tick;
            refreshIconTimer.Start();
        }

        private void UpdateIcon()
        {
            if (RegistryService.Instance.ProxyEnabled)
                notifyIcon.Icon = Resources.networking_green;
            else
                notifyIcon.Icon = Resources.networking;
        }

        #endregion
    }
}
