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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SonarQube.Client.Models;

namespace SonarQube.Client.Tests
{
    [TestClass]
    public class SonarQubeService_GetRulesAsync : SonarQubeService_TestBase
    {
        [TestMethod]
        public async Task GetRulesAsync_Old_Active_SonarQubeResponse()
        {
            await ConnectToSonarQube();

            SetupRequest("api/rules/search?activation=true&qprofile=quality-profile-1&f=repo%2CinternalKey%2Cparams%2Cactives%2ChtmlDesc%2Ctags%2Cname%2ChtmlNote&p=1&ps=500", @"
{
  ""total"": 4,
  ""p"": 1,
  ""ps"": 500,
  ""rules"": [
    {
      ""key"": ""csharpsquid:S2225"",
      ""repo"": ""csharpsquid"",
      ""params"": [],
      ""type"": ""BUG""
    },
    {
      ""key"": ""csharpsquid:S4524"",
      ""repo"": ""csharpsquid"",
      ""params"": [],
      ""type"": ""VULNERABILITY""
    },
    {
      ""key"": ""csharpsquid:S2342"",
      ""repo"": ""csharpsquid"",
      ""htmlDesc"": ""Html Description"",
      ""name"": ""RuleName"",
      ""tags"": [""tag1"",""tag2""],
      ""params"": [
        {
          ""key"": ""format"",
          ""htmlDesc"": ""Regular expression used to check the enumeration type names against."",
          ""defaultValue"": ""^([A-Z]{1,3}[a-z0-9]+)*([A-Z]{2})?$"",
          ""type"": ""STRING""
        },
        {
          ""key"": ""flagsAttributeFormat"",
          ""htmlDesc"": ""Regular expression used to check the flags enumeration type names against."",
          ""defaultValue"": ""^([A-Z]{1,3}[a-z0-9]+)*([A-Z]{2})?s$"",
          ""type"": ""STRING""
        }
      ],
      ""type"": ""CODE_SMELL""
    },
  ],
  ""actives"": {
    ""csharpsquid:S2225"": [
      {
        ""qProfile"": ""quality-profile-1"",
        ""inherit"": ""NONE"",
        ""severity"": ""MAJOR"",
        ""params"": [],
        ""createdAt"": ""2019-01-10T14:00:14+0100"",
        ""updatedAt"": ""2019-01-10T14:00:14+0100""
      }
    ],
    ""csharpsquid:S4524"": [
      {
        ""qProfile"": ""quality-profile-1"",
        ""inherit"": ""NONE"",
        ""severity"": ""CRITICAL"",
        ""params"": [],
        ""createdAt"": ""2019-01-10T14:00:14+0100"",
        ""updatedAt"": ""2019-01-10T14:00:14+0100""
      }
    ],
    ""csharpsquid:S2342"": [
      {
        ""qProfile"": ""quality-profile-1"",
        ""inherit"": ""NONE"",
        ""severity"": ""MINOR"",
        ""params"": [
          {
            ""key"": ""format"",
            ""value"": ""^([A-Z]{1,3}[a-z0-9]+)*([A-Z]{2})?$""
          },
          {
            ""key"": ""flagsAttributeFormat"",
            ""value"": ""^([A-Z]{1,3}[a-z0-9]+)*([A-Z]{2})?s$""
          }
        ],
        ""createdAt"": ""2019-01-10T14:00:14+0100"",
        ""updatedAt"": ""2019-01-10T14:00:14+0100""
      }
    ]
  },
  ""qProfiles"": {
    ""quality-profile-1"": {
      ""name"": ""Sonar way"",
      ""lang"": ""cs"",
      ""langName"": ""C#""
    }
  }
}
");

            var result = await service.GetRulesAsync(true, "quality-profile-1", CancellationToken.None);

            messageHandler.VerifyAll();

            result.Should().HaveCount(3);
            result.Select(r => r.Key).Should().ContainInOrder(new[] { "S2225", "S4524", "S2342" });
            result.Select(r => r.RepositoryKey).Should().Contain(new[] { "csharpsquid", "csharpsquid", "csharpsquid" });
            result.Select(r => r.IsActive).Should().Contain(new[] { true, true, true });
            result.Select(r => r.Severity).Should().ContainInOrder(new[] { SonarQubeIssueSeverity.Major, SonarQubeIssueSeverity.Critical, SonarQubeIssueSeverity.Minor });
            result.Select(r => r.IssueType).Should().ContainInOrder(new[] { SonarQubeIssueType.Bug, SonarQubeIssueType.Vulnerability, SonarQubeIssueType.CodeSmell });

            result.Select(r => r.Parameters.Count).Should().Contain(new[] { 0, 2, 0 });
            result.SelectMany(r => r.Parameters.Select(p => p.Key)).Should().ContainInOrder(new[] { "format", "flagsAttributeFormat" });
            result.SelectMany(r => r.Parameters.Select(p => p.Value)).Should().ContainInOrder(new[] { "^([A-Z]{1,3}[a-z0-9]+)*([A-Z]{2})?$", "^([A-Z]{1,3}[a-z0-9]+)*([A-Z]{2})?s$" });

            result[2].Description.Should().Be("Html Description");

            // All rules with empty parameters should return the same (read-only) object
            // 0 = S2225, no params; 1 = S4524, no params; 2 = S2342, has params
            result[0].Parameters.Should().NotBeNull();
            result[0].Parameters.Count().Should().Be(0);
            result[2].Parameters.Count().Should().Be(2);
            result[0].Parameters.Should().BeSameAs(result[1].Parameters);
        }

        [TestMethod]
        public async Task GetRulesAsync_9_6_Active_SonarQubeResponse()
        {
            await ConnectToSonarQube(version: "9.6.0.0");

            SetupRequest("api/rules/search?activation=true&qprofile=quality-profile-1&f=repo%2CinternalKey%2Cparams%2Cactives%2ChtmlDesc%2Ctags%2Cname%2ChtmlNote%2CdescriptionSections%2CeducationPrinciples&p=1&ps=500", @"
{
  ""total"": 4,
  ""p"": 1,
  ""ps"": 500,
  ""rules"": [
    {
      ""key"": ""csharpsquid:S2225"",
      ""repo"": ""csharpsquid"",
      ""params"": [],
      ""type"": ""BUG""
    },
    {
      ""key"": ""csharpsquid:S4524"",
      ""repo"": ""csharpsquid"",
      ""params"": [],
      ""type"": ""VULNERABILITY""
    },
    {
      ""key"": ""csharpsquid:S2342"",
      ""repo"": ""csharpsquid"",
      ""htmlDesc"": ""Html Description"",
      ""name"": ""RuleName"",
      ""tags"": [""tag1"",""tag2""],
      ""params"": [
        {
          ""key"": ""format"",
          ""htmlDesc"": ""Regular expression used to check the enumeration type names against."",
          ""defaultValue"": ""^([A-Z]{1,3}[a-z0-9]+)*([A-Z]{2})?$"",
          ""type"": ""STRING""
        },
        {
          ""key"": ""flagsAttributeFormat"",
          ""htmlDesc"": ""Regular expression used to check the flags enumeration type names against."",
          ""defaultValue"": ""^([A-Z]{1,3}[a-z0-9]+)*([A-Z]{2})?s$"",
          ""type"": ""STRING""
        }
      ],
      ""type"": ""CODE_SMELL"",
      ""descriptionSections"" : [
        {
          ""key"": ""key1"",
          ""content"": ""content1""
        },
        {
          ""key"": ""key2"",
          ""content"": ""content2"",
          ""context"":{
            ""displayName"":""displayName"",
            ""key"":""key""
          }
        }
        ],
         ""educationPrinciples"": [
            ""education principle 1"",
            ""education principle 2""
        ]
    },
  ],
  ""actives"": {
    ""csharpsquid:S2225"": [
      {
        ""qProfile"": ""quality-profile-1"",
        ""inherit"": ""NONE"",
        ""severity"": ""MAJOR"",
        ""params"": [],
        ""createdAt"": ""2019-01-10T14:00:14+0100"",
        ""updatedAt"": ""2019-01-10T14:00:14+0100""
      }
    ],
    ""csharpsquid:S4524"": [
      {
        ""qProfile"": ""quality-profile-1"",
        ""inherit"": ""NONE"",
        ""severity"": ""CRITICAL"",
        ""params"": [],
        ""createdAt"": ""2019-01-10T14:00:14+0100"",
        ""updatedAt"": ""2019-01-10T14:00:14+0100""
      }
    ],
    ""csharpsquid:S2342"": [
      {
        ""qProfile"": ""quality-profile-1"",
        ""inherit"": ""NONE"",
        ""severity"": ""MINOR"",
        ""params"": [
          {
            ""key"": ""format"",
            ""value"": ""^([A-Z]{1,3}[a-z0-9]+)*([A-Z]{2})?$""
          },
          {
            ""key"": ""flagsAttributeFormat"",
            ""value"": ""^([A-Z]{1,3}[a-z0-9]+)*([A-Z]{2})?s$""
          }
        ],
        ""createdAt"": ""2019-01-10T14:00:14+0100"",
        ""updatedAt"": ""2019-01-10T14:00:14+0100""
      }
    ]
  },
  ""qProfiles"": {
    ""quality-profile-1"": {
      ""name"": ""Sonar way"",
      ""lang"": ""cs"",
      ""langName"": ""C#""
    }
  }
}
");

            var result = await service.GetRulesAsync(true, "quality-profile-1", CancellationToken.None);

            messageHandler.VerifyAll();

            result.Should().HaveCount(3);
            result.Select(r => r.Key).Should().ContainInOrder(new[] { "S2225", "S4524", "S2342" });
            result.Select(r => r.RepositoryKey).Should().Contain(new[] { "csharpsquid", "csharpsquid", "csharpsquid" });
            result.Select(r => r.IsActive).Should().Contain(new[] { true, true, true });
            result.Select(r => r.Severity).Should().ContainInOrder(new[] { SonarQubeIssueSeverity.Major, SonarQubeIssueSeverity.Critical, SonarQubeIssueSeverity.Minor });
            result.Select(r => r.IssueType).Should().ContainInOrder(new[] { SonarQubeIssueType.Bug, SonarQubeIssueType.Vulnerability, SonarQubeIssueType.CodeSmell });

            result.Select(r => r.Parameters.Count).Should().Contain(new[] { 0, 2, 0 });
            result.SelectMany(r => r.Parameters.Select(p => p.Key)).Should().ContainInOrder(new[] { "format", "flagsAttributeFormat" });
            result.SelectMany(r => r.Parameters.Select(p => p.Value)).Should().ContainInOrder(new[] { "^([A-Z]{1,3}[a-z0-9]+)*([A-Z]{2})?$", "^([A-Z]{1,3}[a-z0-9]+)*([A-Z]{2})?s$" });

            result[2].Description.Should().Be("Html Description");

            result[2].DescriptionSections.Count.Should().Be(2);
            result[2].DescriptionSections[0].Key.Should().Be("key1");
            result[2].DescriptionSections[0].HtmlContent.Should().Be("content1");
            result[2].DescriptionSections[0].Context.Should().BeNull();

            result[2].DescriptionSections[1].Key.Should().Be("key2");
            result[2].DescriptionSections[1].HtmlContent.Should().Be("content2");
            result[2].DescriptionSections[1].Context.Should().NotBeNull();
            result[2].DescriptionSections[1].Context.Key.Should().Be("key");
            result[2].DescriptionSections[1].Context.DisplayName.Should().Be("displayName");

            result[2].EducationPrinciples.Count.Should().Be(2);
            result[2].EducationPrinciples[0].Should().Be("education principle 1");
            result[2].EducationPrinciples[1].Should().Be("education principle 2");

            // All rules with empty parameters should return the same (read-only) object
            // 0 = S2225, no params; 1 = S4524, no params; 2 = S2342, has params
            result[0].Parameters.Should().NotBeNull();
            result[0].Parameters.Count().Should().Be(0);
            result[2].Parameters.Count().Should().Be(2);
            result[0].Parameters.Should().BeSameAs(result[1].Parameters);
        }

        [TestMethod]
        public async Task GetRulesAsync_9_5_Active_SonarQubeResponse()
        {
            await ConnectToSonarQube(version: "9.5.0.0");

            SetupRequest("api/rules/search?activation=true&qprofile=quality-profile-1&f=repo%2CinternalKey%2Cparams%2Cactives%2ChtmlDesc%2Ctags%2Cname%2ChtmlNote%2CdescriptionSections&p=1&ps=500", @"
{
  ""total"": 4,
  ""p"": 1,
  ""ps"": 500,
  ""rules"": [
    {
      ""key"": ""csharpsquid:S2225"",
      ""repo"": ""csharpsquid"",
      ""params"": [],
      ""type"": ""BUG""
    },
    {
      ""key"": ""csharpsquid:S4524"",
      ""repo"": ""csharpsquid"",
      ""params"": [],
      ""type"": ""VULNERABILITY""
    },
    {
      ""key"": ""csharpsquid:S2342"",
      ""repo"": ""csharpsquid"",
      ""htmlDesc"": ""Html Description"",
      ""name"": ""RuleName"",
      ""tags"": [""tag1"",""tag2""],
      ""params"": [
        {
          ""key"": ""format"",
          ""htmlDesc"": ""Regular expression used to check the enumeration type names against."",
          ""defaultValue"": ""^([A-Z]{1,3}[a-z0-9]+)*([A-Z]{2})?$"",
          ""type"": ""STRING""
        },
        {
          ""key"": ""flagsAttributeFormat"",
          ""htmlDesc"": ""Regular expression used to check the flags enumeration type names against."",
          ""defaultValue"": ""^([A-Z]{1,3}[a-z0-9]+)*([A-Z]{2})?s$"",
          ""type"": ""STRING""
        }
      ],
      ""type"": ""CODE_SMELL"",
      ""descriptionSections"" : [
        {
          ""key"": ""key1"",
          ""content"": ""content1""
        },
        {
          ""key"": ""key2"",
          ""content"": ""content2"",
          ""context"":{
            ""displayName"":""displayName"",
            ""key"":""key""
          }
        }
        ]
    },
  ],
  ""actives"": {
    ""csharpsquid:S2225"": [
      {
        ""qProfile"": ""quality-profile-1"",
        ""inherit"": ""NONE"",
        ""severity"": ""MAJOR"",
        ""params"": [],
        ""createdAt"": ""2019-01-10T14:00:14+0100"",
        ""updatedAt"": ""2019-01-10T14:00:14+0100""
      }
    ],
    ""csharpsquid:S4524"": [
      {
        ""qProfile"": ""quality-profile-1"",
        ""inherit"": ""NONE"",
        ""severity"": ""CRITICAL"",
        ""params"": [],
        ""createdAt"": ""2019-01-10T14:00:14+0100"",
        ""updatedAt"": ""2019-01-10T14:00:14+0100""
      }
    ],
    ""csharpsquid:S2342"": [
      {
        ""qProfile"": ""quality-profile-1"",
        ""inherit"": ""NONE"",
        ""severity"": ""MINOR"",
        ""params"": [
          {
            ""key"": ""format"",
            ""value"": ""^([A-Z]{1,3}[a-z0-9]+)*([A-Z]{2})?$""
          },
          {
            ""key"": ""flagsAttributeFormat"",
            ""value"": ""^([A-Z]{1,3}[a-z0-9]+)*([A-Z]{2})?s$""
          }
        ],
        ""createdAt"": ""2019-01-10T14:00:14+0100"",
        ""updatedAt"": ""2019-01-10T14:00:14+0100""
      }
    ]
  },
  ""qProfiles"": {
    ""quality-profile-1"": {
      ""name"": ""Sonar way"",
      ""lang"": ""cs"",
      ""langName"": ""C#""
    }
  }
}
");

            var result = await service.GetRulesAsync(true, "quality-profile-1", CancellationToken.None);

            messageHandler.VerifyAll();

            result.Should().HaveCount(3);
            result.Select(r => r.Key).Should().ContainInOrder(new[] { "S2225", "S4524", "S2342" });
            result.Select(r => r.RepositoryKey).Should().Contain(new[] { "csharpsquid", "csharpsquid", "csharpsquid" });
            result.Select(r => r.IsActive).Should().Contain(new[] { true, true, true });
            result.Select(r => r.Severity).Should().ContainInOrder(new[] { SonarQubeIssueSeverity.Major, SonarQubeIssueSeverity.Critical, SonarQubeIssueSeverity.Minor });
            result.Select(r => r.IssueType).Should().ContainInOrder(new[] { SonarQubeIssueType.Bug, SonarQubeIssueType.Vulnerability, SonarQubeIssueType.CodeSmell });

            result.Select(r => r.Parameters.Count).Should().Contain(new[] { 0, 2, 0 });
            result.SelectMany(r => r.Parameters.Select(p => p.Key)).Should().ContainInOrder(new[] { "format", "flagsAttributeFormat" });
            result.SelectMany(r => r.Parameters.Select(p => p.Value)).Should().ContainInOrder(new[] { "^([A-Z]{1,3}[a-z0-9]+)*([A-Z]{2})?$", "^([A-Z]{1,3}[a-z0-9]+)*([A-Z]{2})?s$" });

            result[2].Description.Should().Be("Html Description");

            result[2].DescriptionSections.Count.Should().Be(2);
            result[2].DescriptionSections[0].Key.Should().Be("key1");
            result[2].DescriptionSections[0].HtmlContent.Should().Be("content1");
            result[2].DescriptionSections[0].Context.Should().BeNull();

            result[2].DescriptionSections[1].Key.Should().Be("key2");
            result[2].DescriptionSections[1].HtmlContent.Should().Be("content2");
            result[2].DescriptionSections[1].Context.Should().NotBeNull();
            result[2].DescriptionSections[1].Context.Key.Should().Be("key");
            result[2].DescriptionSections[1].Context.DisplayName.Should().Be("displayName");

            // All rules with empty parameters should return the same (read-only) object
            // 0 = S2225, no params; 1 = S4524, no params; 2 = S2342, has params
            result[0].Parameters.Should().NotBeNull();
            result[0].Parameters.Count().Should().Be(0);
            result[2].Parameters.Count().Should().Be(2);
            result[0].Parameters.Should().BeSameAs(result[1].Parameters);
        }

        [TestMethod]
        public async Task GetRulesAsync_NotActive_SonarQubeResponse()
        {
            await ConnectToSonarQube();

            SetupRequest("api/rules/search?activation=false&qprofile=quality-profile-1&f=repo%2CinternalKey%2Cparams%2Cactives%2ChtmlDesc%2Ctags%2Cname%2ChtmlNote&p=1&ps=500", @"
{
  ""total"": 4,
  ""p"": 1,
  ""ps"": 500,
  ""rules"": [
    {
      ""key"": ""csharpsquid:S2225"",
      ""repo"": ""csharpsquid"",
      ""params"": [],
      ""type"": ""BUG""
    },
    {
      ""key"": ""csharpsquid:S4524"",
      ""repo"": ""csharpsquid"",
      ""params"": [],
      ""type"": ""VULNERABILITY""
    },
    {
      ""key"": ""csharpsquid:S2342"",
      ""repo"": ""csharpsquid"",
      ""params"": [
        {
          ""key"": ""format"",
          ""htmlDesc"": ""Regular expression used to check the enumeration type names against."",
          ""defaultValue"": ""^([A-Z]{1,3}[a-z0-9]+)*([A-Z]{2})?$"",
          ""type"": ""STRING""
        },
        {
          ""key"": ""flagsAttributeFormat"",
          ""htmlDesc"": ""Regular expression used to check the flags enumeration type names against."",
          ""defaultValue"": ""^([A-Z]{1,3}[a-z0-9]+)*([A-Z]{2})?s$"",
          ""type"": ""STRING""
        }
      ],
      ""type"": ""CODE_SMELL""
    },
  ],
  ""actives"": { },
  ""qProfiles"": { }
}
");

            var result = await service.GetRulesAsync(false, "quality-profile-1", CancellationToken.None);

            messageHandler.VerifyAll();

            result.Should().HaveCount(3);
            result.Select(r => r.Key).Should().ContainInOrder(new[] { "S2225", "S4524", "S2342" });
            result.Select(r => r.RepositoryKey).Should().ContainInOrder(new[] { "csharpsquid", "csharpsquid", "csharpsquid" });
            result.Select(r => r.IsActive).Should().ContainInOrder(new[] { false, false, false });
            result.Select(r => r.Severity).Should().ContainInOrder(new[] { SonarQubeIssueSeverity.Unknown, SonarQubeIssueSeverity.Unknown, SonarQubeIssueSeverity.Unknown });
            result.Select(r => r.IssueType).Should().ContainInOrder(new[] { SonarQubeIssueType.Bug, SonarQubeIssueType.Vulnerability, SonarQubeIssueType.CodeSmell });

            // The response contains parameter "definitions", the Parameters property contains parameter values
            result.Select(r => r.Parameters.Count).Should().ContainInOrder(new[] { 0, 0, 0 });

            // All empty parameter objects should be the same instance
            result[0].Parameters.Should().BeSameAs(result[1].Parameters);
            result[0].Parameters.Should().BeSameAs(result[2].Parameters);
        }

        [TestMethod]
        public void GetRulesAsync_NotConnected()
        {
            // No calls to Connect
            // No need to setup request, the operation should fail

            Func<Task<IList<SonarQubeRule>>> func = async () =>
                await service.GetRulesAsync(true, "whatever", CancellationToken.None);

            func.Should().ThrowExactly<InvalidOperationException>().And
                .Message.Should().Be("This operation expects the service to be connected.");

            logger.ErrorMessages.Should().Contain("The service is expected to be connected.");
        }
    }
}
