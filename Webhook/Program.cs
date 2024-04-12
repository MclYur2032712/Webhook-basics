using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Webhook
{
    /*
    class MyApplicationContext : ApplicationContext
    {
        char letter = 'A';
        public MyApplicationContext()
        {
            do
            {
                letter = Console.ReadKey().KeyChar;
                Console.WriteLine("Test");
                //DO NUT PUT A LOOP OF SENDKEY HERE
            } while (letter != 'q');
            Application.Exit();
        }
    }
    */

    internal class Program
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        static void Main(string[] args)
        {
            /*
            char letter = 'A';
            var doWork = Task.Run(() =>
            {
                do
                {
                    letter = Console.ReadKey().KeyChar;
                    SendKeys.SendWait("{DOWN}");
                    SendKeys.SendWait("{DOWN}{RIGHT}");
                    SendKeys.SendWait("{RIGHT}");
                    SendKeys.SendWait("{A}");
                } while (letter != 'q');
                Application.Exit(); // Quick exit for demonstration only.  
            });
            */

            _hookID = SetHook(_proc);
            Application.Run();
            UnhookWindowsHookEx(_hookID);
            
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(
        int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(
        int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Console.WriteLine((Keys)vkCode);
                //This is where is supposedely write my code
                //If the key is equal to H
                if(vkCode == 0x48)
                {
                    UnhookWindowsHookEx(_hookID);
                    SendKeys.SendWait("{DOWN}");
                    SendKeys.SendWait("{DOWN}{RIGHT}");
                    SendKeys.SendWait("{RIGHT}");
                    SendKeys.SendWait("{A}");

                    Console.WriteLine("Hadouken!");
                    _hookID = SetHook(_proc);
                }
                //if the key is A
                if (vkCode == 0x41)
                {
                    UnhookWindowsHookEx(_hookID);
                    SendKeys.SendWait("{UP}");
                    SendKeys.SendWait("{UP}");
                    SendKeys.SendWait("{DOWN}");
                    SendKeys.SendWait("{DOWN}");
                    SendKeys.SendWait("{LEFT}");
                    SendKeys.SendWait("{RIGHT}");
                    SendKeys.SendWait("{LEFT}");
                    SendKeys.SendWait("{RIGHT}");
                    SendKeys.SendWait("{B}");
                    SendKeys.SendWait("{A}");
                    SendKeys.SendWait("{ENTER}");
                    Console.WriteLine("Konami Code");
                    _hookID = SetHook(_proc);
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
        LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}