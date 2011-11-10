/*
 * Copyright (c) 2011 WinBMA/Andrew Moore
 *
 * LICENSED UNDER THE MIT LICENSE
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of
 * this software and associated documentation files (the "Software"), to deal in
 * the Software without restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
 * Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
 * WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace WinBMA.Utilities
{
    public class SystemHotKey : IDisposable
    {
        private const int WM_HOTKEY = 786;
        private bool disposed;

        private bool enabled;

        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                if (value != enabled)
                {
                    if ((int)hwndSource.Handle != 0)
                    {
                        if (value)
                            RegisterHotKey(hwndSource.Handle, id, (int)Modifiers, (int)Key);
                        else
                            UnregisterHotKey(hwndSource.Handle, id);
                    }

                    enabled = value;
                }
            }
        }

        private HwndSourceHook hook;

        public event EventHandler<SystemHotKeyEventArgs> HotKeyPressed;

        private HwndSource hwndSource;
        private int id;

        public Keys Key { get; set; }

        public ModifierKeys Modifiers { get; set; }

        public SystemHotKey(System.Windows.Window window)
        {
            Initialize((HwndSource)HwndSource.FromVisual(window));
        }

        public SystemHotKey(HwndSource hwndSource)
        {
            Initialize(hwndSource);
        }

        ~SystemHotKey()
        {
            this.Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                hwndSource.RemoveHook(hook);
            }

            Enabled = false;

            disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Initialize(HwndSource hwndSource)
        {
            this.hook = new HwndSourceHook(WndProc);
            this.hwndSource = hwndSource;
            hwndSource.AddHook(hook);

            Random rand = new Random((int)DateTime.Now.Ticks);
            id = rand.Next();
        }

        [DllImport("user32", CharSet = CharSet.Ansi,
                                   SetLastError = true, ExactSpelling = true)]
        private static extern int RegisterHotKey(IntPtr hwnd,
                int id, int modifiers, int key);

        [DllImport("user32", CharSet = CharSet.Ansi,
                                   SetLastError = true, ExactSpelling = true)]
        private static extern int UnregisterHotKey(IntPtr hwnd, int id);

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam,
                                IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                if ((int)wParam == id)
                    if (HotKeyPressed != null)
                        HotKeyPressed(this, new SystemHotKeyEventArgs(this));
            }

            return new IntPtr(0);
        }

        [Flags()]
        public enum ModifierKeys : int
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            Windows = 8
        }
    }
}