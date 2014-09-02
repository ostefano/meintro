using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace meintro {
    class Program {

        private const int ADDRINT_SIZE = 4;
        private const int MAX_CHAR_COUNT = 128;
        private const int MAX_THREAD_COUNT = 64;
        private const int MAX_DLL_COUNT = 256;

        private const int SHM_ADDRINT_SIZE = ADDRINT_SIZE;
        private const int SHM_INT64_SIZE = sizeof(UInt64);// 8;
        private const int SHM_INT32_SIZE = sizeof(UInt32);// 4;
        private const int SHM_INT16_SIZE = sizeof(UInt16);// 2;
        private const int SHM_CHAR_SIZE = sizeof(bool);//1;
        
        private const int SHM_DLL_ENV_SIZE =    (SHM_CHAR_SIZE * MAX_CHAR_COUNT) + 
                                                (SHM_INT64_SIZE * 6);

        private const int SHM_THREAD_ENV_SIZE = (SHM_INT32_SIZE * 3) + (4) + 
                                                (SHM_INT64_SIZE * 11) + 
                                                (SHM_INT16_SIZE * 1) + (6) + 
                                                (SHM_INT32_SIZE * MAX_DLL_COUNT) + 
                                                (SHM_DLL_ENV_SIZE * MAX_DLL_COUNT);

        private const int SHM_PROCESS_ENV_SIZE = (SHM_CHAR_SIZE * MAX_CHAR_COUNT) +
                                                 (SHM_INT32_SIZE * 1) + (4) +
                                                 (SHM_INT64_SIZE * 1) +
                                                 (SHM_INT16_SIZE * 1) + (6) +
                                                 (SHM_INT32_SIZE * MAX_THREAD_COUNT) +
                                                 (SHM_THREAD_ENV_SIZE * MAX_THREAD_COUNT);

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
        struct SHM_THREAD_ENV {
            [FieldOffset(0)]
            public UInt32 thread_id;
           
            [FieldOffset(SHM_INT32_SIZE)]
	        public UInt32  esp_max;

            [FieldOffset(SHM_INT32_SIZE + SHM_INT32_SIZE)]
	        public UInt32  esp_min;

            [FieldOffset(SHM_INT32_SIZE + SHM_INT32_SIZE + 4)]
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = 2)]
	        public UInt32[]  code_range;

            [FieldOffset(SHM_INT32_SIZE + SHM_INT32_SIZE + 4 + (SHM_INT32_SIZE * 2))]
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = 2)]
	        public UInt32[]  data_range;

            [FieldOffset(SHM_INT32_SIZE + SHM_INT32_SIZE + 4 + (SHM_INT32_SIZE * 2) + (SHM_INT32_SIZE * 2))]
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = 2)]
	        public UInt32[]  stack_range;

            [FieldOffset(SHM_INT32_SIZE + SHM_INT32_SIZE + 4 + (SHM_INT32_SIZE * 2) + (SHM_INT32_SIZE * 2) + (SHM_INT32_SIZE * 2) + (SHM_INT64_SIZE * 0))]
	        public UInt64 llstack_counter;			

            [FieldOffset(SHM_INT32_SIZE + SHM_INT32_SIZE + 4 + (SHM_INT32_SIZE * 2) + (SHM_INT32_SIZE * 2) + (SHM_INT32_SIZE * 2) + (SHM_INT64_SIZE * 1))]
	        public UInt64 stack_counter;

            [FieldOffset(SHM_INT32_SIZE + SHM_INT32_SIZE + 4 + (SHM_INT32_SIZE * 2) + (SHM_INT32_SIZE * 2) + (SHM_INT32_SIZE * 2) + (SHM_INT64_SIZE * 2))]
	        public UInt64 heap_counter;

            [FieldOffset(SHM_INT32_SIZE + SHM_INT32_SIZE + 4 + (SHM_INT32_SIZE * 2) + (SHM_INT32_SIZE * 2) + (SHM_INT32_SIZE * 2) + (SHM_INT64_SIZE * 3))]
	        public UInt64 data_counter;

            [FieldOffset(SHM_INT32_SIZE + SHM_INT32_SIZE + 4 + (SHM_INT32_SIZE * 2) + (SHM_INT32_SIZE * 2) + (SHM_INT32_SIZE * 2) + (SHM_INT64_SIZE * 4))]
	        public UInt64 global_slstack_counter;

            [FieldOffset(SHM_INT32_SIZE + SHM_INT32_SIZE + 4 + (SHM_INT32_SIZE * 2) + (SHM_INT32_SIZE * 2) + (SHM_INT32_SIZE * 2) + (SHM_INT64_SIZE * 5))]
	        public UInt64 global_stack_counter;	

            [FieldOffset(SHM_INT32_SIZE + SHM_INT32_SIZE + 4 + (SHM_INT32_SIZE * 2) + (SHM_INT32_SIZE * 2) + (SHM_INT32_SIZE * 2) + (SHM_INT64_SIZE * 6))]
	        public UInt64 global_heap_counter;

            [FieldOffset(SHM_INT32_SIZE + SHM_INT32_SIZE + 4 + (SHM_INT32_SIZE * 2) + (SHM_INT32_SIZE * 2) + (SHM_INT32_SIZE * 2) + (SHM_INT64_SIZE * 7))]
	        public UInt64 global_data_counter;

            [FieldOffset(SHM_INT32_SIZE + SHM_INT32_SIZE + 4 + (SHM_INT32_SIZE * 2) + (SHM_INT32_SIZE * 2) + (SHM_INT32_SIZE * 2) + (SHM_INT64_SIZE * 8))]
	        public UInt16 dll_count;
     
            [FieldOffset(SHM_INT32_SIZE + SHM_INT32_SIZE + 4 + (SHM_INT32_SIZE * 2) + (SHM_INT32_SIZE * 2) + (SHM_INT32_SIZE * 2) + (SHM_INT64_SIZE * 8) + SHM_INT16_SIZE + 6)]
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = MAX_DLL_COUNT)]
	        public UInt32[] dll_lookup;

            [FieldOffset(SHM_INT32_SIZE + SHM_INT32_SIZE + 4 + (SHM_INT32_SIZE * 2) + (SHM_INT32_SIZE * 2) + (SHM_INT32_SIZE * 2) + (SHM_INT64_SIZE * 8) + SHM_INT16_SIZE + 6 + (MAX_DLL_COUNT * SHM_INT32_SIZE))]
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = SHM_DLL_ENV_SIZE * MAX_DLL_COUNT)]
	        public Byte[]	dll_envs;

        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct SHM_THREAD_ENV_REV
        {
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 thread_id;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 esp_max;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 esp_min;

            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = 2)]
            public UInt32[] code_range;

            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = 2)]
            public UInt32[] data_range;

            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = 2)]
            public UInt32[] stack_range;

            [MarshalAs(UnmanagedType.U8)]
            public UInt64 llstack_counter;

            [MarshalAs(UnmanagedType.U8)]
            public UInt64 stack_counter;

            [MarshalAs(UnmanagedType.U8)]
            public UInt64 heap_counter;

            [MarshalAs(UnmanagedType.U8)]
            public UInt64 data_counter;

            [MarshalAs(UnmanagedType.U8)]
            public UInt64 global_slstack_counter;

            [MarshalAs(UnmanagedType.U8)]
            public UInt64 global_stack_counter;

            [MarshalAs(UnmanagedType.U8)]
            public UInt64 global_heap_counter;

            [MarshalAs(UnmanagedType.U8)]
            public UInt64 global_data_counter;

            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dll_count;

            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = MAX_DLL_COUNT)]
            public UInt32[] dll_lookup;

            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = SHM_DLL_ENV_SIZE * MAX_DLL_COUNT)]
            public Byte[] dll_envs;
        }

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
        struct SHM_DLL_ENV { 
            [FieldOffset(0)]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_CHAR_COUNT)]
            public string name;

            [FieldOffset((SHM_CHAR_SIZE * MAX_CHAR_COUNT))]
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = 2)]
            public UInt32[] data_range;

            [FieldOffset((SHM_CHAR_SIZE * MAX_CHAR_COUNT) + (SHM_INT32_SIZE * 2))]
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = 2)]
            public UInt32[] code_range;

            [FieldOffset((SHM_CHAR_SIZE * MAX_CHAR_COUNT) + (SHM_INT32_SIZE * 2) + (SHM_INT32_SIZE))]
            public UInt64 llstack_counter;

            [FieldOffset((SHM_CHAR_SIZE * MAX_CHAR_COUNT) + (SHM_INT32_SIZE * 2) + (SHM_INT32_SIZE) + SHM_INT64_SIZE)]
            public UInt64 stack_counter;

            [FieldOffset((SHM_CHAR_SIZE * MAX_CHAR_COUNT) + (SHM_INT32_SIZE * 2) + (SHM_INT32_SIZE) + SHM_INT64_SIZE + SHM_INT64_SIZE)]
            public UInt64 heap_counter;

            [FieldOffset((SHM_CHAR_SIZE * MAX_CHAR_COUNT) + (SHM_INT32_SIZE * 2) + (SHM_INT32_SIZE) + SHM_INT64_SIZE + SHM_INT64_SIZE + SHM_INT64_SIZE)]
            public UInt64 data_counter;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct SHM_DLL_ENV_REV {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_CHAR_COUNT)]
            public string name;

            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = 2)]
            public UInt32[] data_range;

            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = 2)]
            public UInt32[] code_range;

            [MarshalAs(UnmanagedType.U8)]
            public UInt64 llstack_counter;

            [MarshalAs(UnmanagedType.U8)]
            public UInt64 stack_counter;

            [MarshalAs(UnmanagedType.U8)]
            public UInt64 heap_counter;

            [MarshalAs(UnmanagedType.U8)]
            public UInt64 data_counter;
        }

        
        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
        struct SHM_PROCESS_ENV {
            [FieldOffset(0)]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_CHAR_COUNT)]
            public string name;

            [FieldOffset((SHM_CHAR_SIZE * MAX_CHAR_COUNT))]
            public UInt32 process_id;

            [FieldOffset((SHM_CHAR_SIZE * MAX_CHAR_COUNT) + SHM_INT32_SIZE + 4)]
            public UInt64 total_counter;

            [FieldOffset((SHM_CHAR_SIZE * MAX_CHAR_COUNT) + SHM_INT32_SIZE + 4 + SHM_INT64_SIZE)]
            public UInt16 thread_count;

            [FieldOffset((SHM_CHAR_SIZE * MAX_CHAR_COUNT) + SHM_INT32_SIZE + 4 + SHM_INT64_SIZE + SHM_INT16_SIZE + 6)]
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = MAX_THREAD_COUNT)]
            public UInt32[] thread_lookup;

            [FieldOffset((SHM_CHAR_SIZE * MAX_CHAR_COUNT) + SHM_INT32_SIZE + 4 + SHM_INT64_SIZE + SHM_INT16_SIZE + 6 + (SHM_INT32_SIZE * MAX_THREAD_COUNT))]
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = SHM_THREAD_ENV_SIZE * MAX_THREAD_COUNT)]
            public Byte[] thread_envs;    
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct SHM_PROCESS_ENV_REV {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_CHAR_COUNT)]
            public string name;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 process_id;

            [MarshalAs(UnmanagedType.U8)]
            public UInt64 total_counter;

            [MarshalAs(UnmanagedType.U2)]
            public UInt16 thread_count;

            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = MAX_THREAD_COUNT)]
            public UInt32[] thread_lookup;

            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = SHM_THREAD_ENV_SIZE * MAX_THREAD_COUNT)]
            public Byte[] thread_envs;
        }

        static void Main(string[] args) {

            try
            {

                unsafe
                {

                    FileStream fileStream = new FileStream(@"c:\Users\Stefano\mepropin\date_140902_1544_s001.log", FileMode.Open);

                    Console.WriteLine("["+SHM_PROCESS_ENV_SIZE+"] PROCESS_ENV_REV: " + Marshal.SizeOf(typeof(SHM_PROCESS_ENV_REV)) + " PROCESS_ENV: " + Marshal.SizeOf(typeof(SHM_PROCESS_ENV)));
                    Console.WriteLine("["+SHM_THREAD_ENV_SIZE+"] THREAD_ENV_REV: " + Marshal.SizeOf(typeof(SHM_THREAD_ENV_REV)) + " ENV: " + Marshal.SizeOf(typeof(SHM_THREAD_ENV)));
                    Console.WriteLine("["+SHM_DLL_ENV_SIZE+"] DLL_ENV_REV: " + Marshal.SizeOf(typeof(SHM_DLL_ENV_REV)) + " ENV: " + Marshal.SizeOf(typeof(SHM_DLL_ENV)));

                    SHM_PROCESS_ENV_REV aStruct;
                    int count = Marshal.SizeOf(typeof(SHM_PROCESS_ENV_REV));
                    byte[] readBuffer = new byte[count];
                    BinaryReader reader = new BinaryReader(fileStream);
                    readBuffer = reader.ReadBytes(count);
                    GCHandle handle = GCHandle.Alloc(readBuffer, GCHandleType.Pinned);
                    aStruct = (SHM_PROCESS_ENV_REV)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(SHM_PROCESS_ENV_REV));

                    Console.Out.WriteLine("Test " + aStruct.name);

                    //Console.Out.WriteLine("Test " + Marshal.SizeOf(typeof(SHM_PROCESS_ENV)));

                    handle.Free();

                    Console.In.ReadLine();
                }

            }
            catch (Exception e)
            {
                Console.Out.WriteLine("[*] Exception " + e.Message);

                Console.In.ReadLine();

            }

        }
    }
}
