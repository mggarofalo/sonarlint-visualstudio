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
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SonarQube.Client.Requests;

namespace SonarQube.Client.Api.V5_50
{
    public class DownloadStaticFile : RequestBase<Stream>, IDownloadStaticFile
    {
        [JsonIgnore]
        public string PluginKey { get; set; }

        [JsonIgnore]
        public string FileName { get; set; }

        protected override string Path => $"static/{PluginKey}/{FileName}";

        protected override async Task<Result<Stream>> ReadResponseAsync(HttpResponseMessage httpResponse)
        {
            if (!httpResponse.IsSuccessStatusCode)
            {
                return new Result<Stream>(httpResponse, default(Stream));
            }

            var responseStream = await httpResponse.Content.ReadAsStreamAsync()
                .ConfigureAwait(false);

            return new Result<Stream>(httpResponse, responseStream);
        }

        protected override Stream ParseResponse(string response)
        {
            throw new NotSupportedException("This method will not be called because we override ReadResponse.");
        }
    }
}
