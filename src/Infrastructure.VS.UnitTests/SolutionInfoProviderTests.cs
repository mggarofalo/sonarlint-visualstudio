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
using FluentAssertions;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SonarLint.VisualStudio.Core;
using SonarLint.VisualStudio.TestInfrastructure;
using Task = System.Threading.Tasks.Task;

namespace SonarLint.VisualStudio.Infrastructure.VS.UnitTests
{
    [TestClass]
    public class SolutionInfoProviderTests
    {
        [TestMethod]
        public void MefCtor_CheckIsExported()
        {
            MefTestHelpers.CheckTypeCanBeImported<SolutionInfoProvider, ISolutionInfoProvider>(
                MefTestHelpers.CreateExport<SVsServiceProvider>(),
                MefTestHelpers.CreateExport<IThreadHandling>());
        }

        [TestMethod]
        public void CheckIsSharedMefComponent()
        {
            MefTestHelpers.CheckIsSingletonMefComponent<SolutionInfoProvider>();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("a value")]
        public async Task Get_ReturnsExpectedValue(string solutionNameToReturn)
        {
            var solution = CreateIVsSolution(solutionNameToReturn);
            var serviceProvider = CreateServiceProviderWithSolution(solution.Object);

            var testSubject = CreateTestSubject(serviceProvider.Object);

            var actual = await testSubject.GetFullSolutionFilePathAsync();
            actual.Should().Be(solutionNameToReturn);
        }

        [TestMethod]
        public async Task Get_ServiceCalledOnBackgroundThread()
        {
            var calls = new List<string>();

            var solution = CreateIVsSolution("any", () => calls.Add("GetSolutionName"));
            var serviceProvider = CreateServiceProviderWithSolution(solution.Object, () => calls.Add("GetService"));
            var threadHandling = CreateThreadHandlingWithRunOnUICallback(() => calls.Add("RunOnUIThread"));

            var testSubject = CreateTestSubject(serviceProvider.Object, threadHandling.Object);

            var actual = await testSubject.GetFullSolutionFilePathAsync();

            calls.Should().ContainInOrder("RunOnUIThread", "GetService", "GetSolutionName");
        }

        private static SolutionInfoProvider CreateTestSubject(IServiceProvider serviceProvider,
            IThreadHandling threadHandling = null)
        {
            serviceProvider ??= Mock.Of<IServiceProvider>();
            threadHandling ??= new NoOpThreadHandler();

            return new SolutionInfoProvider(serviceProvider, threadHandling);
        }

        private static Mock<IServiceProvider> CreateServiceProviderWithSolution(IVsSolution solution, Action callback = null)
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x => x.GetService(typeof(SVsSolution)))
                .Callback<IInvocation>(x => callback?.Invoke())
                .Returns(solution);

            return serviceProvider;
        }

        private static Mock<IVsSolution> CreateIVsSolution(string solutionNameToReturn, Action callback = null)
        {
            var solution = new Mock<IVsSolution>();

            object solutionDirectory = solutionNameToReturn;
            solution
                .Setup(x => x.GetProperty((int)__VSPROPID.VSPROPID_SolutionFileName, out solutionDirectory))
                .Callback<IInvocation>(x => callback?.Invoke())
                .Returns(VSConstants.S_OK);

            return solution;
        }

        private static Mock<IThreadHandling> CreateThreadHandlingWithRunOnUICallback(Action testOperation)
        {
            var threadHandling = new Mock<IThreadHandling>();
            threadHandling.Setup(x => x.RunOnUIThreadAsync(It.IsAny<Action>()))
                .Callback<Action>(productOperation =>
                {
                    testOperation();
                    productOperation.Invoke();
                });
            return threadHandling;
        }
    }
}
