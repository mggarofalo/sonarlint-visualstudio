﻿/*
 * SonarLint for Visual Studio
 * Copyright (C) 2016-2023 SonarSource SA
 * mailto:info AT sonarsource DOT com
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Windows.Input;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using SonarLint.VisualStudio.ConnectedMode.Binding;
using SonarLint.VisualStudio.Integration.State;
using SonarLint.VisualStudio.Integration.WPF;

namespace SonarLint.VisualStudio.Integration.TeamExplorer
{
    internal class ConnectSectionViewModel : TeamExplorerSectionViewModelBase,
                                        IConnectSectionViewModel,
                                        IUserNotification /* Most of it implemented by TeamExplorerSectionViewModelBase */
    {
        private TransferableVisualState state;
        private ICommand connectCommand;
        private ICommand<BindCommandArgs> bindCommand;
        private ICommand<string> browseToUrl;

        public ConnectSectionViewModel()
        {
            this.Title = Resources.Strings.ConnectSectionTitle;
            this.IsExpanded = true;
            this.IsVisible = true;
        }

        #region IUserNotification

        public void ShowNotificationError(string message, Guid notificationId, ICommand associatedCommand)
        {
            this.ShowNotification(message, NotificationType.Error, NotificationFlags.NoTooltips/*No need for them since we don't use hyperlinks*/, associatedCommand, notificationId);
        }

        public void ShowNotificationWarning(string message, Guid notificationId, ICommand associatedCommand)
        {
            this.ShowNotification(message, NotificationType.Warning, NotificationFlags.NoTooltips/*No need for them since we don't use hyperlinks*/, associatedCommand, notificationId);
        }

        #endregion

        #region Properties

        public TransferableVisualState State
        {
            get { return this.state; }
            set { this.SetAndRaisePropertyChanged(ref this.state, value); }
        }

        #endregion

        #region Commands

        public ICommand ConnectCommand
        {
            get { return this.connectCommand; }
            set { SetAndRaisePropertyChanged(ref this.connectCommand, value); }
        }

        public ICommand<BindCommandArgs> BindCommand
        {
            get { return this.bindCommand; }
            set { SetAndRaisePropertyChanged(ref this.bindCommand, value); }
        }

        public ICommand<string> BrowseToUrlCommand
        {
            get { return this.browseToUrl; }
            set { SetAndRaisePropertyChanged(ref this.browseToUrl, value); }
        }

        #endregion
    }
}
