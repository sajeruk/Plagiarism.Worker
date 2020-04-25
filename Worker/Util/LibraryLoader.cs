using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Plagiarism.Worker.Util
{
    public class LibraryLoader
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(String dllname);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, String procName);

        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr hModule);

        private IntPtr HModule;
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr CompareSource(byte[] source1, byte[] source2);
        private CompareSource Method;

        public LibraryLoader(string dllName)
        {
            HModule = LoadLibrary(dllName);
            if (HModule == IntPtr.Zero)
            {
                throw new Exception("failed to load dll: " + dllName);
            }
            IntPtr proc = GetProcAddress(HModule, "CompareSource");
            if (proc == IntPtr.Zero)
            {
                throw new Exception("failed to find func: CompareSource");
            }
            Method = Marshal.GetDelegateForFunctionPointer(proc, typeof(CompareSource)) as CompareSource;
        }
        
        ~LibraryLoader()
        {
            if (HModule != IntPtr.Zero)
            {
                FreeLibrary(HModule);
            }
        }

        public string Call(byte[] source1, byte[] source2)
        {
            IntPtr ptr = Method.Invoke(source1, source2);
            return Marshal.PtrToStringAnsi(ptr);
        }
    }
}
