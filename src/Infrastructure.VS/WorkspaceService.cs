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
using System.ComponentModel.Composition;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SonarLint.VisualStudio.Core;

namespace SonarLint.VisualStudio.Infrastructure.VS
{
    [Export(typeof(IWorkspaceService))]
    internal class WorkspaceService : IWorkspaceService
    {
        private readonly IVsSolution vsSolution;
        private readonly ILogger logger;

        [ImportingConstructor]
        public WorkspaceService([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider, ILogger logger)
        {
            vsSolution = serviceProvider.GetService(typeof(SVsSolution)) as IVsSolution;
            this.logger = logger;
        }

        public string FindRootDirectory()
        {
            var hr = vsSolution.GetProperty((int)__VSPROPID.VSPROPID_SolutionDirectory, out var solutionDirectory);

            if (hr != VSConstants.S_OK)
            {
                logger.WriteLine(Resources.NoOpenSolutionOrFolder);
            }

            return (string)solutionDirectory;
        }
    }
}
