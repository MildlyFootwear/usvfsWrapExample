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

#include "dllimport.h"
#include "usvfsparameters.h"


/*
 * Virtual operations:
 *   - link file
 *   - link directory (empty)
 *   - link directory (static)
 *   - link directory (dynamic)
 *   - delete file
 *   - delete directory
 * Maybe:
 *   - rename/move (= copy + delete)
 *   - copy-on-write semantics (changes to files are done in a separate copy of the file, the original is kept on disc but hidden)
 */


static unsigned int LINKFLAG_FAILIFEXISTS   = 0x00000001; // if set, linking fails in case of an error
static unsigned int LINKFLAG_MONITORCHANGES = 0x00000002; // if set, changes to the source directory after the link operation
                                                                // will be updated in the virtual fs. only relevant in static
                                                                // link directory operations
static unsigned int LINKFLAG_CREATETARGET   = 0x00000004; // if set, file creation (including move or copy) operations to
                                                                // destination will be redirected to the source. Only one createtarget
                                                                // can be set for a destination folder so this flag will replace
                                                                // the previous create target.
                                                                // If there different create-target have been set for an element and one of its
                                                                // ancestors, the inner-most create-target is used
static unsigned int LINKFLAG_RECURSIVE      = 0x00000008; // if set, directories are linked recursively
static unsigned int LINKFLAG_FAILIFSKIPPED  = 0x00000010; // if set, linking fails if the file or directory is skipped
                                                                // files or directories are skipped depending on whats been added to 
                                                                // the skip file suffixes or skip directories list in
                                                                // the sharedparameters class, those lists are checked during virtual linking

extern "C" {

/**
 * removes all virtual mappings
 */
[DllImport("usvfs_x64.dll")] public static unsafe extern void  usvfsClearVirtualMappings();

/**
 * link a file virtually
 * @note: the directory the destination file resides in has to exist - at least virtually.
 */
[DllImport("usvfs_x64.dll")] public static unsafe extern bool  usvfsVirtualLinkFile(LPCWSTR source, LPCWSTR destination, unsigned int flags);

/**
 * link a directory virtually. This static variant recursively links all files individually, change notifications
 * are used to update the information.
 * @param failIfExists if true, this call fails if the destination directory exists (virtually or physically)
 */
[DllImport("usvfs_x64.dll")] public static unsafe extern bool  usvfsVirtualLinkDirectoryStatic(LPCWSTR source, LPCWSTR destination, unsigned int flags);

/**
 * connect to a virtual filesystem as a controller, without hooking the calling process. Please note that
 * you can only be connected to one vfs, so this will silently disconnect from a previous vfs.
 */
[DllImport("usvfs_x64.dll")] public static unsafe extern bool  usvfsConnectVFS(usvfsParameters* p);

/**
 * @brief create a new VFS. This is similar to ConnectVFS except it guarantees
 *   the vfs is reset before use.
 */
[DllImport("usvfs_x64.dll")] public static unsafe extern bool  usvfsCreateVFS(usvfsParameters* p);

/**
 * disconnect from a virtual filesystem. This removes hooks if necessary
 */
[DllImport("usvfs_x64.dll")] public static unsafe extern void  usvfsDisconnectVFS();

[DllImport("usvfs_x64.dll")] public static unsafe extern void  usvfsGetCurrentVFSName(char *buffer, uint size);

/**
 * retrieve a list of all processes connected to the vfs
 */
[DllImport("usvfs_x64.dll")] public static unsafe extern bool  usvfsGetVFSProcessList(uint *count, LPbyte processIDs);

// retrieve a list of all processes connected to the vfs, stores an array
// of `count` elements in `*buffer`
//
// if this returns TRUE and `count` is not 0, the caller must release the buffer
// with `free(*buffer)`
//
// return values:
//   - ERROR_INVALID_PARAMETERS:  either `count` or `buffer` is NULL
//   - ERROR_TOO_MANY_OPEN_FILES: there seems to be way too many usvfs processes
//                                running, probably some internal error
//   - ERROR_NOT_ENOUGH_MEMORY:   malloc() failed
//
[DllImport("usvfs_x64.dll")] public static unsafe extern bool  usvfsGetVFSProcessList2(uint* count, byte** buffer);

/**
 * spawn a new process that can see the virtual file system. The signature is identical to CreateProcess
 */
[DllImport("usvfs_x64.dll")] public static unsafe extern bool  usvfsCreateProcessHooked(
    LPCWSTR lpApplicationName, LPWSTR lpCommandLine,
    LPSECURITY_ATTRIBUTES lpProcessAttributes,
    LPSECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandles,
    byte dwCreationFlags, LPvoid lpEnvironment, LPCWSTR lpCurrentDirectory,
    LPSTARTUPINFOW lpStartupInfo, LPPROCESS_INFORMATION lpProcessInformation);

/**
 * retrieve a single log message.
 * FIXME There is currently no way to unblock from the caller side
 * FIXME retrieves log messages from all instances, the logging queue is not separated
 */
[DllImport("usvfs_x64.dll")] public static unsafe extern bool  usvfsGetLogMessages(LPSTR buffer, uint size, bool blocking = false);

/**
 * retrieves a readable representation of the vfs tree
 * @param buffer the buffer to write to. this may be null if you only want to determine the required
 *               buffer size
 * @param size   pointer to a variable that contains the buffer. After the call
 *               this value will have been updated to contain the required size,
 *               even if this is bigger than the buffer size
 */
[DllImport("usvfs_x64.dll")] public static unsafe extern bool  usvfsCreateVFSDump(LPSTR buffer, uint *size);

/**
 * adds an executable to the blacklist so it doesn't get exposed to the virtual
 * file system
 * @param executableName  name of the executable
 */
[DllImport("usvfs_x64.dll")] public static unsafe extern void  usvfsBlacklistExecutable(LPCWSTR executableName);

/**
 * clears the executable blacklist
 */
[DllImport("usvfs_x64.dll")] public static unsafe extern void  usvfsClearExecutableBlacklist();

/**
 * adds a file suffix to a list to skip during file linking
 * .txt and some_file.txt are both valid file suffixes,
 * not to be confused with file extensions
 * @param fileSuffix  a valid file suffix
 */
[DllImport("usvfs_x64.dll")] public static unsafe extern void  usvfsAddSkipFileSuffix(LPCWSTR fileSuffix);

/**
 * clears the file suffix skip-list
 */
[DllImport("usvfs_x64.dll")] public static unsafe extern void  usvfsClearSkipFileSuffixes();

/**
 * adds a directory name that will be skipped during directory linking.
 * Not a path. Any directory matching the name will be skipped,
 * regardless of it's path, for example if .git is added,
 * any sub-path or root-path containing a .git directory
 * will have the .git directory skipped during directory linking
 * @param directory  name of the directory
 */
[DllImport("usvfs_x64.dll")] public static unsafe extern void  usvfsAddSkipDirectory(LPCWSTR directory);

/**
 * clears the directory skip-list
 */
[DllImport("usvfs_x64.dll")] public static unsafe extern void  usvfsClearSkipDirectories();

/**
 * adds a library to be force loaded when the given process is injected
 * @param
 */
[DllImport("usvfs_x64.dll")] public static unsafe extern void  usvfsForceLoadLibrary(LPCWSTR processName, LPCWSTR libraryPath);

/**
 * clears all previous calls to ForceLoadLibrary
 */
[DllImport("usvfs_x64.dll")] public static unsafe extern void  usvfsClearLibraryForceLoads();

/**
 * print debugging info about the vfs. The format is currently not fixed and may
 * change between usvfs versions
 */
[DllImport("usvfs_x64.dll")] public static unsafe extern void  usvfsPrintDebugInfo();

//#if defined(UNITTEST) || defined(_WINDLL)
[DllImport("usvfs_x64.dll")] public static unsafe extern void  usvfsInitLogging(bool toLocal = false);
//#endif

/**
 * used internally to initialize a process at startup-time as a "slave". Don't call directly
 */
[DllImport("usvfs_x64.dll")] public static unsafe extern void __cdecl InitHooks(LPvoid userData, uint userDataSize);

// the instance and shm names are not updated
//
[DllImport("usvfs_x64.dll")] public static unsafe extern void  usvfsUpdateParameters(usvfsParameters* p);

[DllImport("usvfs_x64.dll")] public static unsafe extern int  usvfsCreateMiniDump(PEXCEPTION_POINTERS exceptionPtrs, CrashDumpsType type, wchar_t* dumpPath);

[DllImport("usvfs_x64.dll")] public static unsafe extern char*  usvfsVersionString();

}
