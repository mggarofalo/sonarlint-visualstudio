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

using SonarLint.VisualStudio.Integration.Vsix.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;

namespace SonarLint.VisualStudio.Integration.Vsix
{
    /// <summary>
    /// Interaction logic for OtherOptionsDialogControl.xaml
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class OtherOptionsDialogControl : UserControl
    {
        public OtherOptionsDialogControl()
        {
            InitializeComponent();

            // Set the example json payload (see the xaml file for an explanation)
            jsonExampleTextBlock.Text = Strings.ToolsOptions_ExampleJson;
        }
    }
}
