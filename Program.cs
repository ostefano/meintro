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


        static SHM_PROCESS_ENV[] GetSnapshot(String filename) {
            Console.WriteLine("[*] File '"+filename+"'");
            FileStream fileStream = new FileStream(filename, FileMode.Open);
            Debug.Assert(fileStream.Length / Marshal.SizeOf(typeof(SHM_PROCESS_ENV)) == MAX_PROCESS_COUNT);
            Console.WriteLine("[*] Snapshot len: " + fileStream.Length);
            Console.WriteLine("[*] Estimated processes: " + fileStream.Length / Marshal.SizeOf(typeof(SHM_PROCESS_ENV)));
            Console.WriteLine("[*]");
                  
            SHM_PROCESS_ENV[] snapshot = new SHM_PROCESS_ENV[MAX_PROCESS_COUNT];
            for (int i = 0; i < MAX_PROCESS_COUNT; i++) {
                byte[] readBuffer = new byte[Marshal.SizeOf(typeof(SHM_PROCESS_ENV))];
                BinaryReader reader = new BinaryReader(fileStream);
                readBuffer = reader.ReadBytes(Marshal.SizeOf(typeof(SHM_PROCESS_ENV)));
                GCHandle handle = GCHandle.Alloc(readBuffer, GCHandleType.Pinned);
                snapshot[i] = (SHM_PROCESS_ENV)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(SHM_PROCESS_ENV));
                handle.Free();
            }
            return snapshot;
        }

        static uint[] GetAllThreadIds(List<SHM_PROCESS_ENV[]> experiment, uint p_index) {
            ISet<uint> result_set = new HashSet<uint>();
            foreach (SHM_PROCESS_ENV[] spe in experiment) {
                foreach (SHM_THREAD_ENV ste in spe[p_index].thread_envs) {
                    if(ste.thread_id != 0) {
                        result_set.Add(ste.thread_id);
                    }
                }
            }
            return result_set.ToArray<uint>();
        }

        static UInt64[] GetAllDllIds(List<SHM_PROCESS_ENV[]> experiment, uint p_index) {
            ISet<UInt64> result_set = new HashSet<UInt64>();
            foreach (SHM_PROCESS_ENV[] spe in experiment) {
                foreach (SHM_THREAD_ENV ste in spe[p_index].thread_envs) {
                    foreach (SHM_DLL_ENV sde in ste.dll_envs) {
                        if (sde.dll_id != 0) {
                            result_set.Add(sde.dll_id);
                        }
                    }
                }
            }
            return result_set.ToArray<UInt64>();
        }

        static void Main(string[] args) {

            String directory_log = "C:\\Users\\Stefano\\mepropin";
            String experiment_log = "date_140904_2141";

            Console.WriteLine("[*] PROCESS_ENV: "   + Marshal.SizeOf(typeof(SHM_PROCESS_ENV)));
            Console.WriteLine("[*] THREAD_ENV:  "   + Marshal.SizeOf(typeof(SHM_THREAD_ENV)));
            Console.WriteLine("[*] DLL_ENV:     "   + Marshal.SizeOf(typeof(SHM_DLL_ENV)));

            try {

                IEnumerable<String> files = Directory.EnumerateFiles(directory_log, "*.*", SearchOption.AllDirectories).Where(s => s.Contains(experiment_log));     
                List<SHM_PROCESS_ENV[]> experiment = new List<SHM_PROCESS_ENV[]>();
                foreach (var f in files.OrderBy(s => s)) {
                    SHM_PROCESS_ENV[] snapshot = GetSnapshot(f);
                    experiment.Add(snapshot);
                }
                
            } catch (Exception e) {
                Console.Out.WriteLine("[*] Exception " + e.Message);
            }

            Console.ReadLine();

        }
    }
}
