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

using System.Text;
using System.Xml;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SonarLint.VisualStudio.Education.Layout.Visual;
using SonarLint.VisualStudio.Education.XamlGenerator;

namespace SonarLint.VisualStudio.Education.UnitTests.Layout.Visual;

[TestClass]
public class BorderedSectionTests
{
    [TestMethod]
    public void ProduceXaml_ReturnsSectionWithStyle()
    {
        var sb = new StringBuilder();
        var contentMock = new Mock<IAbstractVisualizationTreeNode>();
        contentMock.Setup(x => x.ProduceXaml(It.IsAny<XmlWriter>())).Callback((XmlWriter w) => w.WriteString("Hello"));
        var testSubject = new BorderedSection(contentMock.Object);
        var xmlWriter = new XamlWriterFactory().Create(sb);

        testSubject.ProduceXaml(xmlWriter);
        xmlWriter.Close();

        sb.ToString().Should().BeEquivalentTo("<Section Style=\"{DynamicResource Bordered_Section}\">Hello</Section>");
    }
}
