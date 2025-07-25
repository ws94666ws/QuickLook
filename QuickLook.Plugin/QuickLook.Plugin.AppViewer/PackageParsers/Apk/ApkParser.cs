﻿// Copyright © 2017-2025 QL-Win Contributors
//
// This file is part of QuickLook program.
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace QuickLook.Plugin.AppViewer.PackageParsers.Apk;

public static class ApkParser
{
    public static ApkInfo Parse(string path)
    {
        try
        {
            using var zip = new ZipFile(path);

            var apkReader = new ApkReader.ApkReader();
            ApkReader.ApkInfo baseInfo = apkReader.Read(path);
            ApkInfo info = new()
            {
                VersionName = baseInfo.VersionName,
                VersionCode = baseInfo.VersionCode,
                TargetSdkVersion = ApiLevel.Create(baseInfo.TargetSdkVersion).ToString(),
                Permissions = baseInfo.Permissions,
                PackageName = baseInfo.PackageName,
                MinSdkVersion = ApiLevel.Create(baseInfo.MinSdkVersion).ToString(),
                Icon = baseInfo.Icon,
                Icons = baseInfo.Icons,
                Label = baseInfo.Label,
                Labels = baseInfo.Labels,
                Locales = baseInfo.Locales,
                Densities = baseInfo.Densities,
                ABIs = [.. baseInfo.Abis],
                LaunchableActivity = baseInfo.LaunchableActivity,
            };

            if (baseInfo.HasIcon)
            {
                ZipEntry entry = zip.GetEntry(baseInfo.Icons.Values
                    .Where(icon => icon.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                    .LastOrDefault());
                using var s = new BinaryReader(zip.GetInputStream(entry));
                info.Logo = s.ReadBytes((int)entry.Size);
            }

            return info;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }

        return new ApkInfo();
    }
}
