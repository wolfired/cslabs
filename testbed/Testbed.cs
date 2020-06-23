using System;
using System.Runtime.InteropServices;

using utils;

namespace testbed
{
    class Testbed
    {
        [DllImport("tools.dll")] extern public static int tools_max(int x, int y);
        [DllImport("tools.dll")] extern public static IntPtr tools_toupper(string src);

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World! {0}", new Utils
            ().inc(1));

            Console.WriteLine("tools_max() -> {0}", tools_max(99, 9));
            Console.WriteLine("tools_toupper() -> {0}", Marshal.PtrToStringAnsi(tools_toupper("tools_toupper")));
        }
    }
}
