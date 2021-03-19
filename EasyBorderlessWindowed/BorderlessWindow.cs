// Code by Novack at https://forum.unity.com/threads/free-borderless-unity-window.651076/


using System;
using System.Runtime.InteropServices;
using UnityEngine;
namespace UnityBorderlessWindow
{
    public class BorderlessWindow
    {
        public static bool framed = true;
        private const int GWL_STYLE = -16;
        private const int SW_MINIMIZE = 6;
        private const int SW_MAXIMIZE = 3;
        private const int SW_RESTORE = 9;
        private const uint WS_VISIBLE = 268435456;
        private const uint WS_POPUP = 2147483648;
        private const uint WS_BORDER = 8388608;
        private const uint WS_OVERLAPPED = 0;
        private const uint WS_CAPTION = 12582912;
        private const uint WS_SYSMENU = 524288;
        private const uint WS_THICKFRAME = 262144;
        private const uint WS_MINIMIZEBOX = 131072;
        private const uint WS_MAXIMIZEBOX = 65536;
        private const uint WS_OVERLAPPEDWINDOW = 13565952;

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool MoveWindow(
          IntPtr hWnd,
          int x,
          int y,
          int nWidth,
          int nHeight,
          bool bRepaint);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, out BorderlessWindow.WinRect lpRect);

        public static void InitializeOnLoad() => BorderlessWindow.SetFramelessWindow();

        public static void SetFramelessWindow()
        {
            BorderlessWindow.SetWindowLong(BorderlessWindow.GetActiveWindow(), -16, 2415919104U);
            BorderlessWindow.framed = false;
        }

        public static void SetFramedWindow()
        {
            BorderlessWindow.SetWindowLong(BorderlessWindow.GetActiveWindow(), -16, 282001408U);
            BorderlessWindow.framed = true;
        }

        public static void MinimizeWindow() => BorderlessWindow.ShowWindow(BorderlessWindow.GetActiveWindow(), 6);

        public static void MaximizeWindow() => BorderlessWindow.ShowWindow(BorderlessWindow.GetActiveWindow(), 3);

        public static void RestoreWindow() => BorderlessWindow.ShowWindow(BorderlessWindow.GetActiveWindow(), 9);

        public static void MoveWindowPos(Vector2 posDelta, int newWidth, int newHeight)
        {
            IntPtr activeWindow = BorderlessWindow.GetActiveWindow();
            BorderlessWindow.WinRect lpRect;
            BorderlessWindow.GetWindowRect(activeWindow, out lpRect);
            BorderlessWindow.MoveWindow(activeWindow, lpRect.left + (int)posDelta.x, lpRect.top - (int)posDelta.y, newWidth, newHeight, false);
        }

        private struct WinRect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
    }
}
