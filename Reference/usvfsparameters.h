/*
Userspace Virtual Filesystem

Copyright (C) 2015 Sebastian Herbord. All rights reserved.

This file is part of usvfs.

usvfs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

usvfs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with usvfs. If not, see <http://www.gnu.org/licenses/>.
*/
#pragma once

#include "logging.h"
#include "dllimport.h"
#include <chrono>

enum class CrashDumpsType : uint8_t {
  None,
  Mini,
  Data,
  Full
};

extern "C"
{

// deprecated, use usvfsParameters and usvfsCreateParameters()
//
struct USVFSParameters {
  char instanceName[65];
  char currentSHMName[65];
  char currentInverseSHMName[65];
  bool debugMode{false};
  LogLevel logLevel{LogLevel::Debug};
  CrashDumpsType crashDumpsType{CrashDumpsType::None};
  char crashDumpsPath[260];
};


struct usvfsParameters;

[DllImport("usvfs_x64.dll")] public static unsafe extern usvfsParameters* usvfsCreateParameters();
[DllImport("usvfs_x64.dll")] public static unsafe extern usvfsParameters* usvfsDupeParameters(usvfsParameters* p);
[DllImport("usvfs_x64.dll")] public static unsafe extern void usvfsCopyParameters(usvfsParameters* source, usvfsParameters* dest);
[DllImport("usvfs_x64.dll")] public static unsafe extern void usvfsFreeParameters(usvfsParameters* p);

[DllImport("usvfs_x64.dll")] public static unsafe extern void usvfsSetInstanceName(usvfsParameters* p, char* name);
[DllImport("usvfs_x64.dll")] public static unsafe extern void usvfsSetDebugMode(usvfsParameters* p, bool debugMode);
[DllImport("usvfs_x64.dll")] public static unsafe extern void usvfsSetLogLevel(usvfsParameters* p, LogLevel level);
[DllImport("usvfs_x64.dll")] public static unsafe extern void usvfsSetCrashDumpType(usvfsParameters* p, CrashDumpsType type);
[DllImport("usvfs_x64.dll")] public static unsafe extern void usvfsSetCrashDumpPath(usvfsParameters* p, char* path);
[DllImport("usvfs_x64.dll")] public static unsafe extern void usvfsSetProcessDelay(usvfsParameters* p, int milliseconds);

[DllImport("usvfs_x64.dll")] public static unsafe extern char* usvfsLogLevelToString(LogLevel lv);
[DllImport("usvfs_x64.dll")] public static unsafe extern char* usvfsCrashDumpTypeToString(CrashDumpsType t);

}
