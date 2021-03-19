
using System;
using System.Runtime.InteropServices;
using HarmonyLib;
using UnityEngine;
using XRL;
using XRL.UI;
using Mono;

namespace Fyrefly.EasyBorderlessWindow
{
    [HasModSensitiveStaticCache]
    [HarmonyPatch(typeof(Options), "UpdateFlags")]
    static class EasyBorderlessWindow
    {
        [DllImport("user32.dll")] private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")] private static extern IntPtr MonitorFromWindow(IntPtr hWnd, uint dwFlags);
        [DllImport("user32.dll")] private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        // This static method is required because Win32 does not support
        // GetWindowLongPtr directly
        public static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 8)
                return GetWindowLongPtr64(hWnd, nIndex);
            else
                return GetWindowLongPtr32(hWnd, nIndex);

        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, long newValue);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, long newValue);

        // This static method is required because Win32 does not support
        // GetWindowLongPtr directly
        public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, long newValue)
        {

            if (IntPtr.Size == 8)
            {
                Debug.Log($"Test64bitset");
                return SetWindowLongPtr64(hWnd, nIndex, newValue);
            }
            else
            {
                Debug.Log($"Test32bitset");
                return SetWindowLongPtr32(hWnd, nIndex, newValue);
            }
        }

        private enum WindowStyle : long
        {
            BORDER = 0x00800000L,
            POPUP = 0x80000000L,
            THICKFRAME = 0x00040000L,
            CAPTION = 0x00C00000L,
            SYSMENU = 0x00080000L,
            MINIMIZE = 0x20000000L,
            MAXIMIZE = 0x01000000L,


        }

        private static WindowStyle Style
        {
            get => (WindowStyle)GetWindowLongPtr(MainWindow, -16);
            set => SetWindowLongPtr(MainWindow, -16, (long)value);
        }
        private const int OFFSET_WINDOWSTYLE = -16;

        public static bool IsFramelessFullscreen
        {
            get => (Style & WindowStyle.BORDER) == 0;
            set => Style = value
                ? Style & ~(WindowStyle.BORDER | WindowStyle.POPUP | WindowStyle.THICKFRAME)
                : Style | WindowStyle.BORDER | WindowStyle.POPUP | WindowStyle.THICKFRAME;
        }

        public static bool setToFrameless = false;

        public static IntPtr MainWindow;

        public static bool DisplayBorderlessFullscreen => Options.GetOption("OptionDisplayBorderlessFullscreen").EqualsNoCase("Yes");
        static void Postfix()
        {
            Debug.Log($"Starting Borderless Window Patch, main window {MainWindow}, active window {GetForegroundWindow()}\n Thread: {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            if (DisplayBorderlessFullscreen)
            {
                Debug.Log("Making Borderless");
                Screen.fullScreenMode = FullScreenMode.Windowed;
                setToFrameless = true;
            }
            else if (!DisplayBorderlessFullscreen)
            {
                Debug.Log("Making Fullscreen");
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                setToFrameless = false;
            }
            Debug.Log("Borderless Window Patch Complete");
        }

        public static void UpdateFrame()
        {
            try
            {
                if (!Options.DisplayFullscreen && DisplayBorderlessFullscreen && !IsFramelessFullscreen)
                {
                    Debug.Log($"Test1");
                    IsFramelessFullscreen = true;
                    Debug.Log($"Test2");
                }
                else if (!Options.DisplayFullscreen && !DisplayBorderlessFullscreen && IsFramelessFullscreen)
                {
                    Debug.Log($"Test1B");
                    IsFramelessFullscreen = false;
                    Debug.Log($"Test1B");
                }
            }
            catch (Exception e)
            {

                Debug.Log($"Borderless Window Patch Failed with exception {e}");

            }

        }

        [ModSensitiveCacheInit]
        public static void InitialiseMainWindow()
        {
            MainWindow = GetForegroundWindow();
            Debug.Log($"Starting Borderless Window Initialisation, active window {MainWindow}");
        }
    }

}