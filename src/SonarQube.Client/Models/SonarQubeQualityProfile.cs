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

namespace SonarQube.Client.Models
{
    public class SonarQubeQualityProfile
    {
        // Ordinal comparer, similar to project key comparer
        public static readonly StringComparer KeyComparer = StringComparer.Ordinal;

        public string Key { get; }
        public string Name { get; }
        public string Language { get; }
        public bool IsDefault { get; }
        public DateTime TimeStamp { get; }

        public SonarQubeQualityProfile(string key, string name, string language, bool isDefault, DateTime timeStamp)
        {
            Key = key;
            Name = name;
            Language = language;
            IsDefault = isDefault;
            TimeStamp = timeStamp;
        }
    }
}
