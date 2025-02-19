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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SonarLint.VisualStudio.TypeScript.EslintBridgeClient.Contract;

namespace SonarLint.VisualStudio.TypeScript.EslintBridgeClient
{
    internal interface IEslintBridgeClient : IDisposable
    {
        /// <summary>
        /// Configures the linter with the set of rules to execute
        /// </summary>
        /// <remarks>This method should be called whenever the set of active rules or
        /// their configuration changes.</remarks>
        Task InitLinter(IEnumerable<Rule> rules, CancellationToken cancellationToken);

        /// <summary>
        /// Analyzes the specified file and returns the detected issues.
        /// </summary>
        Task<AnalysisResponse> Analyze(string filePath, string tsConfigFilePath, CancellationToken cancellationToken);

        /// <summary>
        /// Closes running eslint-bridge server.
        /// </summary>
        Task Close();
    }
}
