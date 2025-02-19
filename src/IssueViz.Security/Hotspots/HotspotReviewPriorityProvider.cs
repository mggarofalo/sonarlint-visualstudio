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
using System.ComponentModel.Composition;
using SonarLint.VisualStudio.IssueVisualization.Security.Hotspots.Models;

namespace SonarLint.VisualStudio.IssueVisualization.Security.Hotspots
{
    /// <summary>
    /// Provides a mapping for hotspots to their review priority
    /// </summary>
    public interface IHotspotReviewPriorityProvider
    {
        /// <summary>
        /// Supported languages:
        /// <list type="bullet">
        /// <item><description>JavaScript</description></item>
        /// <item><description>TypeScript</description></item>
        /// <item><description>C++</description></item>
        /// <item><description>C</description></item>
        /// </list>
        /// </summary>
        /// <param name="hotspotKey">Hotspot Rule Key</param>
        /// <returns>Associated review priority or null if not mapped</returns>
        HotspotPriority? GetPriority(string hotspotKey);
    }

    [Export(typeof(IHotspotReviewPriorityProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class HotspotReviewPriorityProvider : IHotspotReviewPriorityProvider
    {
        // In order to save time and considering the fact that hotspots are no longer being updated, we decided to make a hardcoded list of priorities by rule key
        // See (https://github.com/SonarSource/sonarlint-visualstudio/issues/4532)
        private static readonly Dictionary<string, HotspotPriority> KeyToPriorityMap = new Dictionary<string, HotspotPriority>
        {
            { "cpp:S5443", HotspotPriority.Low },
            { "cpp:S2068", HotspotPriority.High },
            { "cpp:S5332", HotspotPriority.Low },
            { "cpp:S6069", HotspotPriority.High },
            { "cpp:S5042", HotspotPriority.Low },
            { "cpp:S5802", HotspotPriority.Low },
            { "cpp:S5801", HotspotPriority.High },
            { "cpp:S1313", HotspotPriority.Low },
            { "cpp:S5824", HotspotPriority.Low },
            { "cpp:S5815", HotspotPriority.High },
            { "cpp:S5816", HotspotPriority.High },
            { "cpp:S5813", HotspotPriority.High },
            { "cpp:S5814", HotspotPriority.High },
            { "cpp:S2612", HotspotPriority.Medium },
            { "cpp:S4790", HotspotPriority.Low },
            { "cpp:S2245", HotspotPriority.Medium },
            { "cpp:S5849", HotspotPriority.Medium },
            { "cpp:S5982", HotspotPriority.Low },

            { "c:S5802", HotspotPriority.Low },
            { "c:S5801", HotspotPriority.High },
            { "c:S1313", HotspotPriority.Low },
            { "c:S5824", HotspotPriority.Low },
            { "c:S5815", HotspotPriority.High },
            { "c:S5816", HotspotPriority.High },
            { "c:S5813", HotspotPriority.High },
            { "c:S5814", HotspotPriority.High },
            { "c:S2612", HotspotPriority.Medium },
            { "c:S5443", HotspotPriority.Low },
            { "c:S2068", HotspotPriority.High },
            { "c:S5332", HotspotPriority.Low },
            { "c:S4790", HotspotPriority.Low },
            { "c:S2245", HotspotPriority.Medium },
            { "c:S6069", HotspotPriority.High },
            { "c:S5042", HotspotPriority.Low },
            { "c:S5849", HotspotPriority.Medium },
            { "c:S5982", HotspotPriority.Low },

            { "javascript:S2092", HotspotPriority.Low },
            { "javascript:S6332", HotspotPriority.Low },
            { "javascript:S5122", HotspotPriority.Low },
            { "javascript:S6330", HotspotPriority.Low },
            { "javascript:S5247", HotspotPriority.High },
            { "javascript:S6333", HotspotPriority.Medium },
            { "javascript:S4036", HotspotPriority.Low },
            { "javascript:S4823", HotspotPriority.Low },
            { "javascript:S4829", HotspotPriority.Low },
            { "javascript:S6329", HotspotPriority.Medium },
            { "javascript:S6327", HotspotPriority.Low },
            { "javascript:S1313", HotspotPriority.Low },
            { "javascript:S5148", HotspotPriority.Low },
            { "javascript:S4721", HotspotPriority.High },
            { "javascript:S1523", HotspotPriority.Low },
            { "javascript:S2612", HotspotPriority.Medium },
            { "javascript:S5443", HotspotPriority.Low },
            { "javascript:S5689", HotspotPriority.Low },
            { "javascript:S6319", HotspotPriority.Low },
            { "javascript:S4818", HotspotPriority.Low },
            { "javascript:S4817", HotspotPriority.Low },
            { "javascript:S2077", HotspotPriority.High },
            { "javascript:S5693", HotspotPriority.Medium },
            { "javascript:S5691", HotspotPriority.Low },
            { "javascript:S6308", HotspotPriority.Low },
            { "javascript:S6302", HotspotPriority.Medium },
            { "javascript:S6303", HotspotPriority.Low },
            { "javascript:S2068", HotspotPriority.High },
            { "javascript:S5332", HotspotPriority.Low },
            { "javascript:S6304", HotspotPriority.Medium },
            { "javascript:S6299", HotspotPriority.High },
            { "javascript:S4790", HotspotPriority.Low },
            { "javascript:S2255", HotspotPriority.Low },
            { "javascript:S6281", HotspotPriority.Medium },
            { "javascript:S5759", HotspotPriority.Low },
            { "javascript:S4784", HotspotPriority.Low },
            { "javascript:S3330", HotspotPriority.Low },
            { "javascript:S5757", HotspotPriority.Low },
            { "javascript:S2245", HotspotPriority.Medium },
            { "javascript:S4787", HotspotPriority.Low },
            { "javascript:S6252", HotspotPriority.Low },
            { "javascript:S5042", HotspotPriority.Low },
            { "javascript:S5728", HotspotPriority.Low },
            { "javascript:S5725", HotspotPriority.Low },
            { "javascript:S5604", HotspotPriority.Low },
            { "javascript:S4507", HotspotPriority.Low },
            { "javascript:S6245", HotspotPriority.Low },
            { "javascript:S4502", HotspotPriority.High },
            { "javascript:S6249", HotspotPriority.Low },
            { "javascript:S6270", HotspotPriority.Medium },
            { "javascript:S6275", HotspotPriority.Low },
            { "javascript:S5742", HotspotPriority.Low },
            { "javascript:S5743", HotspotPriority.Low },
            { "javascript:S6265", HotspotPriority.Medium },
            { "javascript:S5739", HotspotPriority.Low },
            { "javascript:S5736", HotspotPriority.Low },
            { "javascript:S5730", HotspotPriority.Low },
            { "javascript:S5852", HotspotPriority.Medium },
            { "javascript:S6268", HotspotPriority.High },
            { "javascript:S5734", HotspotPriority.Low },
            { "javascript:S5732", HotspotPriority.Low },
            { "javascript:S6350", HotspotPriority.High },

            { "typescript:S2092", HotspotPriority.Low },
            { "typescript:S6332", HotspotPriority.Low },
            { "typescript:S5122", HotspotPriority.Low },
            { "typescript:S6330", HotspotPriority.Low },
            { "typescript:S5247", HotspotPriority.High },
            { "typescript:S6333", HotspotPriority.Medium },
            { "typescript:S4036", HotspotPriority.Low },
            { "typescript:S4823", HotspotPriority.Low },
            { "typescript:S4829", HotspotPriority.Low },
            { "typescript:S6329", HotspotPriority.Medium },
            { "typescript:S6327", HotspotPriority.Low },
            { "typescript:S1313", HotspotPriority.Low },
            { "typescript:S5148", HotspotPriority.Low },
            { "typescript:S4721", HotspotPriority.High },
            { "typescript:S1523", HotspotPriority.Medium },
            { "typescript:S2612", HotspotPriority.Medium },
            { "typescript:S5443", HotspotPriority.Low },
            { "typescript:S5689", HotspotPriority.Low },
            { "typescript:S6319", HotspotPriority.Low },
            { "typescript:S4818", HotspotPriority.Low },
            { "typescript:S4817", HotspotPriority.Low },
            { "typescript:S2077", HotspotPriority.High },
            { "typescript:S5693", HotspotPriority.Medium },
            { "typescript:S5691", HotspotPriority.Low },
            { "typescript:S6308", HotspotPriority.Low },
            { "typescript:S6302", HotspotPriority.Medium },
            { "typescript:S6303", HotspotPriority.Low },
            { "typescript:S2068", HotspotPriority.High },
            { "typescript:S5332", HotspotPriority.Low },
            { "typescript:S6304", HotspotPriority.Medium },
            { "typescript:S6299", HotspotPriority.High },
            { "typescript:S4790", HotspotPriority.Low },
            { "typescript:S2255", HotspotPriority.Low },
            { "typescript:S6281", HotspotPriority.Medium },
            { "typescript:S5759", HotspotPriority.Low },
            { "typescript:S4784", HotspotPriority.Low },
            { "typescript:S3330", HotspotPriority.Low },
            { "typescript:S5757", HotspotPriority.Low },
            { "typescript:S2245", HotspotPriority.Medium },
            { "typescript:S4787", HotspotPriority.Low },
            { "typescript:S6252", HotspotPriority.Low },
            { "typescript:S5042", HotspotPriority.Low },
            { "typescript:S5728", HotspotPriority.Low },
            { "typescript:S5725", HotspotPriority.Low },
            { "typescript:S5604", HotspotPriority.Low },
            { "typescript:S4507", HotspotPriority.Low },
            { "typescript:S6245", HotspotPriority.Low },
            { "typescript:S4502", HotspotPriority.High },
            { "typescript:S6249", HotspotPriority.Low },
            { "typescript:S6270", HotspotPriority.Medium },
            { "typescript:S6275", HotspotPriority.Low },
            { "typescript:S5742", HotspotPriority.Low },
            { "typescript:S5743", HotspotPriority.Low },
            { "typescript:S6265", HotspotPriority.Medium },
            { "typescript:S5739", HotspotPriority.Low },
            { "typescript:S5736", HotspotPriority.Low },
            { "typescript:S5730", HotspotPriority.Low },
            { "typescript:S5852", HotspotPriority.Medium },
            { "typescript:S6268", HotspotPriority.High },
            { "typescript:S5734", HotspotPriority.Low },
            { "typescript:S5732", HotspotPriority.Low },
            { "typescript:S6350", HotspotPriority.High },
        };

        public HotspotPriority? GetPriority(string hotspotKey) => 
            KeyToPriorityMap.TryGetValue(hotspotKey ?? throw new ArgumentNullException(nameof(hotspotKey)), out var priority) ? priority : (HotspotPriority?)null;
    }
}
