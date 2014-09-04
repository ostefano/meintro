using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace meintro {
    class Program {

        private const int MAX_CHAR_COUNT = 128;
        private const int MAX_THREAD_COUNT = 64;
        private const int MAX_DLL_COUNT = 256;
        private const int MAX_PROCESS_COUNT = 4;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct SHM_THREAD_ENV {
            public UInt32 thread_id;
            public UInt64 esp_max;
            public UInt64 esp_min;

            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U8, SizeConst = 2)]
            public UInt64[] code_range;
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U8, SizeConst = 2)]
            public UInt64[] data_range;
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U8, SizeConst = 2)]
            public UInt64[] stack_range;

            public UInt64 llstack_counter;
            public UInt64 stack_counter;
            public UInt64 heap_counter;
            public UInt64 data_counter;

            public UInt64 global_slstack_counter;
            public UInt64 global_stack_counter;
            public UInt64 global_heap_counter;
            public UInt64 global_data_counter;

            public UInt16 dll_count;
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = MAX_DLL_COUNT)]
            public UInt32[] dll_lookup;

            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = MAX_DLL_COUNT)]
            public SHM_DLL_ENV[] dll_envs;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct SHM_DLL_ENV {

            public UInt64 dll_id;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_CHAR_COUNT)]
            public string name;

            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U8, SizeConst = 2)]
            public UInt64[] data_range;

            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U8, SizeConst = 2)]
            public UInt64[] code_range;

            public UInt64 llstack_counter;
            public UInt64 stack_counter;
            public UInt64 heap_counter;
            public UInt64 data_counter;
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct SHM_CONFIG_ENV {
            public UInt16 max_char_count;
            public UInt16 max_thread_count;
            public UInt16 max_dll_count;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct SHM_PROCESS_ENV {

            public SHM_CONFIG_ENV configuration;

            public UInt32 process_id;
            public bool process_arch;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_CHAR_COUNT)]
            public string process_name;

            public UInt64 timestamp;
            public UInt64 global_counter;

            public UInt16 thread_count;
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = MAX_THREAD_COUNT)]
            public UInt32[] thread_lookup;
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = MAX_THREAD_COUNT)]
            public SHM_THREAD_ENV[] thread_envs;
        }



        static void Main(string[] args) {

            try {

                unsafe {

                    var files = Directory.EnumerateFiles("C:\\Users\\Stefano\\mepropin", "*.*", SearchOption.AllDirectories).Where(s => s.StartsWith("date_"));
                    foreach (var f in files)
                    {
                        Console.WriteLine("[*] " + f);
                    }


                    FileStream fileStream = new FileStream(@"c:\Users\Stefano\mepropin\date_140904_1643_s000.log", FileMode.Open);

                    Debug.Assert(fileStream.Length / Marshal.SizeOf(typeof(SHM_PROCESS_ENV)) == MAX_PROCESS_COUNT);

                    Console.WriteLine("[*] Snapshot len: " + fileStream.Length);
                    Console.WriteLine("[*] Estimated processes: " + fileStream.Length / Marshal.SizeOf(typeof(SHM_PROCESS_ENV)));
                    Console.WriteLine("[*]");
                    Console.WriteLine("[*] PROCESS_ENV: "   + Marshal.SizeOf(typeof(SHM_PROCESS_ENV)));
                    Console.WriteLine("[*] THREAD_ENV: "    + Marshal.SizeOf(typeof(SHM_THREAD_ENV)));
                    Console.WriteLine("[*] DLL_ENV: "       + Marshal.SizeOf(typeof(SHM_DLL_ENV)));

                    SHM_PROCESS_ENV[] snapshot = new SHM_PROCESS_ENV[MAX_PROCESS_COUNT];
                    for (int i = 0; i < MAX_PROCESS_COUNT; i++) {
                        byte[] readBuffer = new byte[Marshal.SizeOf(typeof(SHM_PROCESS_ENV))];
                        BinaryReader reader = new BinaryReader(fileStream);
                        readBuffer = reader.ReadBytes(Marshal.SizeOf(typeof(SHM_PROCESS_ENV)));
                        GCHandle handle = GCHandle.Alloc(readBuffer, GCHandleType.Pinned);
                        snapshot[i] = (SHM_PROCESS_ENV)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(SHM_PROCESS_ENV));
                        handle.Free();
                    }


                    

                    /*
                    Console.WriteLine("[*] Snapshot MAX_DLL_COUNT    " + aStruct.configuration.max_dll_count + " (reader set to " + MAX_DLL_COUNT + ")");
                    Console.WriteLine("[*] Snapshot MAX_THREAD_COUNT " + aStruct.configuration.max_thread_count + " (reader set to " + MAX_THREAD_COUNT + ")");
                    Console.WriteLine("[*] Snapshot MAX_CHAR_COUNT   " + aStruct.configuration.max_char_count + " (reader set to " + MAX_CHAR_COUNT + ")");

                    Console.Out.WriteLine("Process name: " + aStruct.process_name);
                    */
                    //Console.Out.WriteLine("Test " + Marshal.SizeOf(typeof(SHM_PROCESS_ENV)));

                    

                    Console.In.ReadLine();
                }

            } catch (Exception e) {
                Console.Out.WriteLine("[*] Exception " + e.Message);
                Console.In.ReadLine();
            }
        }
    }
}
