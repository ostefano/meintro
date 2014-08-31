using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace meintro {
    class Program {

        private static int MAX_CHAR_COUNT = 128;

        

        [StructLayout(LayoutKind.Explicit)]
        struct SHM_PROCESS_ENV {
            [FieldOffset(0)]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string name;

            [FieldOffset(128)]
            public UInt32 process_id;


            [FieldOffset(136)]
            public UInt64 total_counter;

            [FieldOffset(144)]
            public UInt16 thread_count;
            
            [FieldOffset(152)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public UInt32[] thread_lookup;

            [FieldOffset(152+(64 * 4))]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2956288)]
            public byte[] thread_envs;
        }

   

        /*
        [StructLayout(LayoutKind.Explicit)]
        struct SHM_PROCESS_ENV
        {
            [FieldOffset(0)]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string FileDate;

            [FieldOffset(8)]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string FileTime;

            [FieldOffset(16)]
            public int Id1;

            [FieldOffset(20)]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 66)] //Or however long Id2 is.
            public string Id2;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct SHM_PROCESS_ENV
        {
            [FieldOffset(0)]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string FileDate;

            [FieldOffset(8)]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string FileTime;

            [FieldOffset(16)]
            public int Id1;

            [FieldOffset(20)]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 66)] //Or however long Id2 is.
            public string Id2;
        }

        */

        private int SHM_PROCESS_SIZE = 2956696;
        private int SHM_THREAD_SIZE = 46192;
        private int SHM_DLL = 176;

        private int PROCESS_COUNT = 5;
        private int THREAD_COUNT = 64;
        private int DLL_COUNT = 256;

        static void Main(string[] args) {

            try
            {

                unsafe
                {

                    FileStream fileStream = new FileStream(@"c:\Users\Stefano\mepropin\date_140818_2134_s001.log", FileMode.Open);

                    SHM_PROCESS_ENV aStruct;
                    int count = Marshal.SizeOf(typeof(SHM_PROCESS_ENV));
                    byte[] readBuffer = new byte[count];
                    BinaryReader reader = new BinaryReader(fileStream);
                    readBuffer = reader.ReadBytes(count);
                    GCHandle handle = GCHandle.Alloc(readBuffer, GCHandleType.Pinned);
                    aStruct = (SHM_PROCESS_ENV)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(SHM_PROCESS_ENV));

                    Console.Out.WriteLine("Test " + aStruct.name);

                    //Console.Out.WriteLine("Test " + Marshal.SizeOf(typeof(SHM_PROCESS_ENV)));

                    handle.Free();

                    Console.In.ReadLine();
                }

            }
            catch (Exception e)
            {

            }

        }
    }
}
