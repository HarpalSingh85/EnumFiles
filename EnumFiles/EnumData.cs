﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EnumFiles
{
    class WinAPIEnum
    {
        private static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        private static int FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
        private static int FILE_ATTRIBUTE_REPARSE = 0x00000400;
        private const int MAX_PATH = 260;

        [StructLayout(LayoutKind.Sequential)]

        private struct FILETIME
        {
            internal uint dwLowDateTime;
            internal uint dwHighDateTime;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]

        private struct WIN32_FIND_DATA
        {
            internal FileAttributes dwFileAttributes;
            internal FILETIME ftCreationTime;
            internal FILETIME ftLastAccessTime;
            internal FILETIME ftLastWriteTime;
            internal int nFileSizeHigh;
            internal int nFileSizeLow;
            internal int dwReserved0;
            internal int dwReserved1;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            internal string cFileName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            internal string cAlternate;

        }

        [Flags]
        private enum EFileAccess : uint
        {
            GenericRead = 0x80000000,
            GenericWrite = 0x40000000,
            GenericExecute = 0x20000000,
            GenericAll = 0x10000000,
        }

        [Flags]
        private enum EFileShare : uint
        {
            None = 0x00000000,
            Read = 0x00000001,
            Write = 0x00000002,
            Delete = 0x00000004,
        }

        private enum ECreationDisposition : uint
        {
            New = 1,
            CreateAlways = 2,
            OpenExisting = 3,
            OpenAlways = 4,
            TruncateExisting = 5,
        }

        [Flags]
        private enum EFileAttributes : uint
        {
            Readonly = 0x00000001,
            Hidden = 0x00000002,
            System = 0x00000004,
            Directory = 0x00000010,
            Archive = 0x00000020,
            Device = 0x00000040,
            Normal = 0x00000080,
            Temporary = 0x00000100,
            SparseFile = 0x00000200,
            ReparsePoint = 0x00000400,
            Compressed = 0x00000800,
            Offline = 0x00001000,
            NotContentIndexed = 0x00002000,
            Encrypted = 0x00004000,
            Write_Through = 0x80000000,
            Overlapped = 0x40000000,
            NoBuffering = 0x20000000,
            RandomAccess = 0x10000000,
            SequentialScan = 0x08000000,
            DeleteOnClose = 0x04000000,
            BackupSemantics = 0x02000000,
            PosixSemantics = 0x01000000,
            OpenReparsePoint = 0x00200000,
            OpenNoRecall = 0x00100000,
            FirstPipeInstance = 0x00080000
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FindClose(IntPtr hFindFile);

        // Assume dirName passed in is already prefixed with \\?\

        public static List<string> FindFilesAndDirs(string dirName)
        {
            List<string> results = new List<string>();

            WIN32_FIND_DATA findData;

            IntPtr findHandle = FindFirstFile(dirName + @"\*", out findData);

            if (findHandle != INVALID_HANDLE_VALUE)
            {
                bool found;
                do
                {
                    string currentFileName = findData.cFileName;

                    // if this is a directory, find its contents
                    //if ((((int)findData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) != 0) && findData.dwFileAttributes != FILE_ATTRIBUTE_REPARSE)
                    if ((((int)findData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) != 0))
                    {

                        if (currentFileName != "." && currentFileName != "..")
                        {
                            List<string> childResults = FindFilesAndDirs(Path.Combine(dirName, currentFileName));

                            // add children and self to results
                            results.AddRange(childResults);
                            results.Add(Path.Combine(dirName, currentFileName));
                        }

                    }

                    // it's a file; add it to the results
                    else
                    {
                        results.Add(Path.Combine(dirName, currentFileName));
                    }

                    // find next
                    found = FindNextFile(findHandle, out findData);
                }

                while (found);
            }



            // close the find handle

            FindClose(findHandle);
            return results;
        }

    }
}
