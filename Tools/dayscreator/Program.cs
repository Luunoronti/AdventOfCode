﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace dayscreator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool force = true;
            bool localDir = Environment.CurrentDirectory.EndsWith("bin\\Debug");

            // first argument is the year
            int.TryParse(args[0], out var year);

            var directory = Environment.CurrentDirectory;
            var rootDir = $"{directory}\\AoC_DX12_V\\src\\{year}";
            if (localDir)
                rootDir = $"{directory}\\..\\..\\..\\..\\AoC_DX12_V\\src\\{year}";

            // if such year exists already. just recreate missing files
            // else create files AND cmake

            if (!Directory.Exists(rootDir))
                Directory.CreateDirectory(rootDir);

            var srcFiles = new Dictionary<string, (string, List<string>)>();

            // recreate missing files
            for (var i = 1; i <= 5; i++) RecreateSources(rootDir, year, 1, 5, i, srcFiles);
            for (var i = 6; i <= 10; i++) RecreateSources(rootDir, year, 6, 10, i, srcFiles);
            for (var i = 11; i <= 15; i++) RecreateSources(rootDir, year, 11, 15, i, srcFiles);
            for (var i = 16; i <= 20; i++) RecreateSources(rootDir, year, 16, 20, i, srcFiles);
            for (var i = 21; i <= 25; i++) RecreateSources(rootDir, year, 21, 25, i, srcFiles);

            // also, recreate stdafx files if do not exist
            if (force || !File.Exists($"{rootDir}\\stdafx.h"))
            {
                File.WriteAllText($"{rootDir}\\stdafx.cpp", stdAfxCppContent);
                File.WriteAllText($"{rootDir}\\stdafx.h", stdAfxHContent);
            }

            // if cmake does not exist, recreate
            if (force || !File.Exists($"{rootDir}\\CMakeLists.txt")) RecreateCMake(rootDir, year, srcFiles);

            // also recreate cmake install
            if (force || !File.Exists($"{rootDir}\\cmake_install.cmake")) File.WriteAllText($"{rootDir}\\cmake_install.cmake", cmakeInstallContent.Replace("%%YEAR%%", year.ToString()));


            // check if other cmakes have this year as dependency
            var otherCmakes = $"{Environment.CurrentDirectory}\\AoC_DX12_V\\src\\DX12\\CMakeLists.txt";
            if (localDir)
                otherCmakes = $"{Environment.CurrentDirectory}\\..\\..\\..\\..\\AoC_DX12_V\\src\\DX12\\CMakeLists.txt";

            AddYearToCmakeLinkLibs(year, otherCmakes);
            AddLibToMainCMake(year, localDir);

            // also add an include that will include all days
            // and make a macro to run each day
            WriteDaysHeader(rootDir, year, srcFiles);

            AddDaysToDatabase(localDir, year);

            if (!localDir)
                Process.Start($"{Environment.CurrentDirectory}\\GenerateSolutions.bat");
        }

        static void WriteDaysHeader(string rootDir, int year, Dictionary<string, (string, List<string>)> days)
        {

            // #define RUN_YEAR_2024(f, c) \
            // f<Year2024Day01>(c);

            // #pragma once
            // #include "..\\2024\\Days 1-5\\Day_01.h"
            string hFile = "#pragma once" + Environment.NewLine;
            foreach (var l in days.Select(d => d.Value.Item2).SelectMany(d => d).Where(s => s.EndsWith(".h")))
            {
                hFile += $"#include \"../{year}/{l}\"{Environment.NewLine}";
            }

            hFile += Environment.NewLine;
            hFile += $"#define RUN_YEAR_{year}(f, c) \\";
            hFile += Environment.NewLine;
            for (int i = 1; i <= 25; i++)
            {
                hFile += $"f<Year{year}Day{i:d2}>(c);\\{Environment.NewLine}";
            }
            hFile += Environment.NewLine;

            File.WriteAllText(rootDir + "\\Days.h", hFile);
        }

        static void AddLibToMainCMake(int year, bool localDir)
        {
            var strToLookOrAdd = $"add_subdirectory(AoC_DX12_V/src/{year})";
            var cmake = $"{Environment.CurrentDirectory}\\CMakeLists.txt";
            if (localDir)
                cmake = $"{Environment.CurrentDirectory}\\..\\..\\..\\..\\CMakeLists.txt";
            var list = File.ReadAllLines(cmake);
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].StartsWith(strToLookOrAdd))
                    return;
            }

            var nl = new List<string>(list);
            nl.Add(strToLookOrAdd);
            File.WriteAllLines(cmake, nl);
        }
        static void AddYearToCmakeLinkLibs(int year, string cmake)
        {
            var list = File.ReadAllLines(cmake);
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].StartsWith("target_link_libraries("))
                {
                    if (list[i].Split(' ').Any(s => s == year.ToString()))
                        return;


                    list[i] = list[i].Replace("AoC_Common", $"AoC_Common {year}");
                    File.WriteAllLines(cmake, list);
                    return;
                }
            }
        }
        static void RecreateSources(string root, int year, int dm, int dmax, int index, Dictionary<string, (string, List<string>)> srcFiles)
        {
            if (!srcFiles.TryGetValue($"src_{dm}-{dmax}", out var list))
                list = srcFiles[$"src_{dm}-{dmax}"] = ($"Days {dm}-{dmax}", new List<string>());

            list.Item2.Add($"Days {dm}-{dmax}//Day {index:d2}.cpp");
            list.Item2.Add($"Days {dm}-{dmax}//Day {index:d2}.h");

            var dir = $"{root}\\Days {dm}-{dmax}";
            Directory.CreateDirectory(dir);
            var cppFile = $"{dir}\\Day {index:d2}.cpp";
            var hFile = $"{dir}\\Day {index:d2}.h";

            File.WriteAllText(cppFile, daycppContent
                .Replace("%%DAY%%", $"{index:d2}")
                .Replace("%%YEAR%%", $"{year}")
                );
            File.WriteAllText(hFile, dayHeaderContent
                .Replace("%%DAY%%", $"{index:d2}")
                .Replace("%%YEAR%%", $"{year}")
                .Replace("%%DAY_NL%%", $"{index}")

                );
        }

        static void RecreateCMake(string root, int year, Dictionary<string, (string, List<string>)> srcFiles)
        {
            var srcs = "";
            var srcDays = "";
            var srcGroups = "";
            foreach (var p in srcFiles)
            {
                srcGroups += $"source_group(\"{p.Value.Item1}\" FILES ${{{p.Key}}}){Environment.NewLine}";
                srcs += $"${{{p.Key}}} ";
                srcDays += $@"set({p.Key}
";
                foreach (var file in p.Value.Item2)
                {
                    srcDays += $@"    ""{file}""
";
                }

                srcDays += $@"    )
";
            }



            // source_group("Days 1-5" FILES ${src15})



            File.WriteAllText($"{root}\\CMakeLists.txt",
                cmakeContent
                .Replace("%%YEAR%%", year.ToString())
                .Replace("%%SRC_DAYS%%", srcDays)
                .Replace("%%SRC_GROUPS%%", srcGroups)
                .Replace("%%SRCS%%", srcs.Trim())
                );
        }

        static void AddDaysToDatabase(bool localDir, int year)
        {
            var path = $"{Environment.CurrentDirectory}\\Database\\DaysData.json";
            if (localDir)
                path = $"{Environment.CurrentDirectory}\\..\\..\\..\\..\\Database\\DaysData.json";

            var data = JsonConvert.DeserializeObject<List<Day>>(File.ReadAllText(path));

            for (int i = 1; i <= 25; i++)
            {
                if (data.Any(d => d.year == year && d.day == i))
                    continue;

                data.Add(new Day
                {
                    day = i,
                    year = year,
                    debugRun = false,
                    enableVisualization = false,
                    run = false,
                    name = "___",
                    liveStep1 = new DayPart() { knownErrors = new KnownErrors() },
                    liveStep2 = new DayPart() { knownErrors = new KnownErrors() },
                    testStep1 = new DayPart() { knownErrors = new KnownErrors() },
                    testStep2 = new DayPart() { knownErrors = new KnownErrors() },
                });
            }

            File.WriteAllText(path, JsonConvert.SerializeObject(data, Formatting.Indented));
        }


        private const string stdAfxHContent = @"
// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//
#pragma once

// C RunTime Header Files
#include <malloc.h>
#include <map>
#include <mutex>
#include <vector>
#include <fstream>
#include <string>

#define HLSLPP_FEATURE_TRANSFORM
#include <hlsl++.h>
// normally, we should not declare namespace use in pch
// but this library is used all over and we just want
// same experience as in real HLSL.
//using namespace hlslpp;


#include <intrin.h>
#include <Tracy.hpp>
#include <aoc.h>
";

        private const string stdAfxCppContent = @"
// stdafx.cpp : source file that includes just the standard includes
// SampleD3D12.pch will be the pre-compiled header
// stdafx.obj will contain the pre-compiled type information
#include ""stdafx.h""
// TODO: reference any additional headers you need in STDAFX.H
// and not in this file
";

        private const string cmakeContent = @"include(${CMAKE_CURRENT_SOURCE_DIR}/../../../common.cmake)

set(src_g
    ""stdafx.cpp""
    ""stdafx.h""
    )

%%SRC_DAYS%%


add_library(%%YEAR%% STATIC ${src_g} %%SRCS%%)
set_property(TARGET %%YEAR%% PROPERTY CXX_STANDARD 23)

%%SRC_GROUPS%%

target_precompile_headers(%%YEAR%% PRIVATE stdafx.h)

add_dependencies(%%YEAR%% copied_common_config)
target_link_libraries(%%YEAR%% TracyClient AoC_Common)
target_include_directories (%%YEAR%% PUBLIC ${CMAKE_HOME_DIRECTORY}/Tools/tracy/public/tracy)
target_include_directories (%%YEAR%% PUBLIC ${CMAKE_HOME_DIRECTORY}/AoC_DX12_V/src/Common)
target_include_directories (%%YEAR%% PUBLIC ${CMAKE_HOME_DIRECTORY}/Tools/hlsl++)
";

        private const string cmakeInstallContent = @"# Install script for directory: C:/Work/AdventOfCode/AoC_DX12_V/src/%%YEAR%%

# Set the install prefix
if(NOT DEFINED CMAKE_INSTALL_PREFIX)
  set(CMAKE_INSTALL_PREFIX ""C:/Program Files/AoC"")
endif()
string(REGEX REPLACE ""/$"" """" CMAKE_INSTALL_PREFIX ""${CMAKE_INSTALL_PREFIX}"")

# Set the install configuration name.
if(NOT DEFINED CMAKE_INSTALL_CONFIG_NAME)
  if(BUILD_TYPE)
    string(REGEX REPLACE ""^[^A-Za-z0-9_]+"" """"
           CMAKE_INSTALL_CONFIG_NAME ""${BUILD_TYPE}"")
  else()
    set(CMAKE_INSTALL_CONFIG_NAME ""Release"")
  endif()
  message(STATUS ""Install configuration: \""${CMAKE_INSTALL_CONFIG_NAME}\"""")
endif()

# Set the component getting installed.
if(NOT CMAKE_INSTALL_COMPONENT)
  if(COMPONENT)
    message(STATUS ""Install component: \""${COMPONENT}\"""")
    set(CMAKE_INSTALL_COMPONENT ""${COMPONENT}"")
  else()
    set(CMAKE_INSTALL_COMPONENT)
  endif()
endif()

# Is this installation the result of a crosscompile?
if(NOT DEFINED CMAKE_CROSSCOMPILING)
  set(CMAKE_CROSSCOMPILING ""FALSE"")
endif()

string(REPLACE "";"" ""\n"" CMAKE_INSTALL_MANIFEST_CONTENT
       ""${CMAKE_INSTALL_MANIFEST_FILES}"")
if(CMAKE_INSTALL_LOCAL_ONLY)
  file(WRITE ""C:/Work/AdventOfCode/AoC_DX12_V/src/%%YEAR%%/install_local_manifest.txt""
     ""${CMAKE_INSTALL_MANIFEST_CONTENT}"")
endif()

";

        private const string daycppContent = @"
#include ""stdafx.h""
#include ""Day %%DAY%%.h""

const std::string Year%%YEAR%%Day%%DAY%%::Part1(const AoCExecutionContext* Context)
{
    return """";
}

const std::string Year%%YEAR%%Day%%DAY%%::Part2(const AoCExecutionContext* Context)
{
    return """";
}
";
        private const string dayHeaderContent = @"#pragma once
#include <AoCBase.h>

class Year%%YEAR%%Day%%DAY%% : public AoCBase
{
public:
    const std::string Part1(const AoCExecutionContext* Context) override;
    const std::string Part2(const AoCExecutionContext* Context) override;
    const __forceinline int GetYear() const override { return %%YEAR%%; };
    const __forceinline int GetDay() const override { return %%DAY_NL%%; }

};";
    }







    public class RootObject
    {
        public List<Day> Days
        {
            get; set;
        }
    }

    public class Day
    {
        public bool run
        {
            get; set;
        }
        public bool debugRun
        {
            get; set;
        }
        public string name
        {
            get; set;
        }
        public int day
        {
            get; set;
        }
        public DayPart liveStep1
        {
            get; set;
        }
        public DayPart liveStep2
        {
            get; set;
        }
        public DayPart testStep1
        {
            get; set;
        }
        public DayPart testStep2
        {
            get; set;
        }
        public int year
        {
            get; set;
        }
        public bool enableVisualization
        {
            get; set;
        }
    }

    public class DayPart
    {
        public string expectedResultStr
        {
            get;
            set;
        } = "";
        public long expectedResult
        {
            get; set;
        }
        public KnownErrors knownErrors
        {
            get; set;
        }
        public bool enableVisualization
        {
            get; set;
        }
    }

    public class KnownErrors
    {
        public string[] general
        {
            get; set;
        } = new string[0];
        public object[] tooSmall
        {
            get; set;
        } = new string[0];
        public object[] tooBig
        {
            get; set;
        } = new string[0];
    }



}




