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

using SonarLint.VisualStudio.ConnectedMode.Migration;
using SonarLint.VisualStudio.ConnectedMode.Migration.Wizard;
using SonarLint.VisualStudio.Core;
using SonarLint.VisualStudio.TestInfrastructure;

namespace SonarLint.VisualStudio.ConnectedMode.UnitTests.Migration.Wizard
{
    [TestClass]
    public class MigrationPromptTests
    {
        [TestMethod]
        public void MefCtor_CheckIsExported()
        {
            MefTestHelpers.CheckTypeCanBeImported<MigrationWizardController, IMigrationWizardController>(
                MefTestHelpers.CreateExport<IConnectedModeMigration>(),
                MefTestHelpers.CreateExport<IBrowserService>(),
                MefTestHelpers.CreateExport<IOutputWindowService>(),
                MefTestHelpers.CreateExport<IGitWorkspaceService>(),
                MefTestHelpers.CreateExport<ILogger>());
        }

        [TestMethod]
        public void CheckIsNonSharedMefComponent()
        {
            MefTestHelpers.CheckIsNonSharedMefComponent<MigrationWizardController>();
        }
    }
}
