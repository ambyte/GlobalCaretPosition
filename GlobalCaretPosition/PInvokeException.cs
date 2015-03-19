using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace GlobalCaretPosition
{
    sealed class PInvokeException : Exception
    {
        public PInvokeException()
            : base("An platform invocation exception has occurred",
                new Win32Exception(Marshal.GetLastWin32Error()))
        { }

        public PInvokeException(string message)
            : base(message, new Win32Exception(Marshal.GetLastWin32Error()))
        { }

        public PInvokeException(string message, Win32Exception exception)
            : base(message, exception)
        { }

        public PInvokeException(string message, Exception exception)
            : base(message, exception)
        { }
    }
}
