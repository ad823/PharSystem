using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

public class GlobalKeyboardScanner : IDisposable
{
    #region Win32 Const

    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_SYSKEYDOWN = 0x0104;

    #endregion

    #region Fields

    private IntPtr _hookID = IntPtr.Zero;
    private LowLevelKeyboardProc _proc;

    private readonly StringBuilder _buffer = new StringBuilder();
    private readonly object _lock = new object();

    private readonly Timer _timer;

    #endregion

    #region Public Properties

    /// <summary>
    /// 是否啟用鍵盤掃碼模式
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 靜默時間（毫秒），超過即視為一筆完成
    /// </summary>
    public int TimeoutMs
    {
        get => _timer.Interval;
        set => _timer.Interval = value;
    }

    #endregion

    #region Events

    /// <summary>
    /// 一筆掃碼完成事件
    /// </summary>
    public event Action<string> OnScanCompleted;

    #endregion

    #region Constructor

    public GlobalKeyboardScanner(int timeoutMs = 200)
    {
        _proc = HookCallback;

        _timer = new Timer();
        _timer.Interval = timeoutMs;
        _timer.Tick += Timer_Tick;
    }

    #endregion

    #region Public Methods

    public void Start()
    {
        if (_hookID != IntPtr.Zero) return;

        using (var process = Process.GetCurrentProcess())
        using (var module = process.MainModule)
        {
            _hookID = SetWindowsHookEx(
                WH_KEYBOARD_LL,
                _proc,
                GetModuleHandle(module.ModuleName),
                0);
        }
    }

    public void Stop()
    {
        if (_hookID != IntPtr.Zero)
        {
            UnhookWindowsHookEx(_hookID);
            _hookID = IntPtr.Zero;
        }

        ClearBuffer();
    }

    #endregion

    #region Hook Core

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (!Enabled)
            return CallNextHookEx(_hookID, nCode, wParam, lParam);

        if (nCode >= 0 &&
            (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
        {
            int vkCode = Marshal.ReadInt32(lParam);

            if (IsIgnoredKey(vkCode))
                return CallNextHookEx(_hookID, nCode, wParam, lParam);

            char? ch = VkCodeToChar(vkCode);
            if (ch.HasValue)
            {
                lock (_lock)
                {
                    _buffer.Append(ch.Value);
                    _timer.Stop();
                    _timer.Start();
                }
            }
        }

        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    #endregion

    #region Timer

    private void Timer_Tick(object sender, EventArgs e)
    {
        _timer.Stop();

        string result;

        lock (_lock)
        {
            if (_buffer.Length <= 2)
            {
                _buffer.Clear();
                return;
            }

            result = _buffer.ToString();
            _buffer.Clear();
        }

        OnScanCompleted?.Invoke(result);
    }

    #endregion

    #region Helper

    private void ClearBuffer()
    {
        lock (_lock)
        {
            _buffer.Clear();
        }
    }

    private bool IsIgnoredKey(int vkCode)
    {
        switch (vkCode)
        {
            case 0x10: // Shift
            case 0x11: // Ctrl
            case 0x12: // Alt
            case 0x09: // Tab
            case 0x0D: // Enter
            case 0x1B: // Esc
                return true;
            default:
                return false;
        }
    }

    private char? VkCodeToChar(int vkCode)
    {
        byte[] keyboardState = new byte[256];
        if (!GetKeyboardState(keyboardState))
            return null;

        uint scanCode = MapVirtualKey((uint)vkCode, 0);
        StringBuilder buffer = new StringBuilder(2);

        int result = ToUnicode(
            (uint)vkCode,
            scanCode,
            keyboardState,
            buffer,
            buffer.Capacity,
            0);

        return result > 0 ? buffer[0] : (char?)null;
    }

    #endregion

    #region IDisposable

    public void Dispose()
    {
        Stop();
        _timer?.Dispose();
    }

    #endregion

    #region Win32 API

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll")]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll")]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll")]
    private static extern bool GetKeyboardState(byte[] lpKeyState);

    [DllImport("user32.dll")]
    private static extern uint MapVirtualKey(uint uCode, uint uMapType);

    [DllImport("user32.dll")]
    private static extern int ToUnicode(
        uint wVirtKey,
        uint wScanCode,
        byte[] lpKeyState,
        [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff,
        int cchBuff,
        uint wFlags);

    #endregion
}
