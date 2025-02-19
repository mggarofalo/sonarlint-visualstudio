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
using System.Security;
using SonarQube.Client.Helpers;

namespace SonarQube.Client.Models
{
    /// <summary>
    /// Represents the connection information needed to connect to SonarQube service
    /// </summary>
    public sealed class ConnectionInformation : ICloneable, IDisposable
    {
#pragma warning disable S1075
        internal static readonly Uri FixedSonarCloudUri = new Uri("https://sonarcloud.io/");
#pragma warning restore S1075

        private bool isDisposed;

        public ConnectionInformation(Uri serverUri, string userName, SecureString password)
        {
            if (serverUri == null)
            {
                throw new ArgumentNullException(nameof(serverUri));
            }

            ServerUri = FixSonarCloudUri(serverUri).EnsureTrailingSlash();
            UserName = userName;
            Password = password?.CopyAsReadOnly();
            Authentication = AuthenticationType.Basic; // Only one supported at this point
            IsSonarCloud = ServerUri == FixedSonarCloudUri;
        }

        public ConnectionInformation(Uri serverUri)
            : this(serverUri, null, null)
        {
        }

        public Uri ServerUri { get; }

        public bool IsSonarCloud { get; }

        public string UserName { get; }

        public SecureString Password { get; }

        public AuthenticationType Authentication { get; }

        public bool IsDisposed => isDisposed;

        public SonarQubeOrganization Organization { get; set; }

        public ConnectionInformation Clone()
        {
            return new ConnectionInformation(ServerUri, UserName, Password?.CopyAsReadOnly()) { Organization = Organization };
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// When the provided serverUri contains 'sonarcloud.io' returns 'https://sonarcloud.io', otherwise
        /// returns serverUri. This method tries to prevent slightly incorrect SonarCloud URIs (e.g. with
        /// http instead of https, or www.sonarcloud.io instead of sonarcloud.io) from redirecting to the
        /// correct scheme and url. See https://github.com/SonarSource/sonarlint-visualstudio/issues/796
        /// </summary>
        private static Uri FixSonarCloudUri(Uri serverUri) =>
            (serverUri.Host.Equals("sonarcloud.io", StringComparison.OrdinalIgnoreCase) ||
             serverUri.Host.Equals("www.sonarcloud.io", StringComparison.OrdinalIgnoreCase))
                ? FixedSonarCloudUri
                : serverUri;

        #region IDisposable Support

        public void Dispose()
        {
            if (!isDisposed)
            {
                Password?.Dispose();
                isDisposed = true;
            }
        }

        #endregion IDisposable Support
    }
}
