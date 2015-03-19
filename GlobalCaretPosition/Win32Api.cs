using System;
using System.Runtime.InteropServices;
using Accessibility;

namespace GlobalCaretPosition
{
    class Win32Api
    {
        internal static Guid IID_IAccessible = new Guid(1636251360, (short)15421, (short)4559, (byte)129, (byte)12, (byte)0, (byte)170, (byte)0, (byte)56, (byte)155, (byte)113);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr window, out uint processId);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWinEventHook(SetWinEventHookEventType eventTypeMin,
            SetWinEventHookEventType eventTypeMax, IntPtr library, SetWinEventHookDelegate handler,
            uint processId, uint threadId, SetWinEventHookFlag flags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnhookWinEvent(IntPtr hook);

        [DllImport("oleacc.dll")]
        public static extern uint AccessibleObjectFromEvent(IntPtr hwnd, uint dwObjectID, uint dwChildID, out IAccessible ppacc, [MarshalAs(UnmanagedType.Struct)] out object pvarChild);

    }

    public delegate void SetWinEventHookDelegate(IntPtr hook, SetWinEventHookEventType eventType,
        IntPtr window, int objectId, int childId, uint threadId, uint time);

    [Flags]
    public enum SetWinEventHookFlag : uint
    {
        WINEVENT_OUTOFCONTEXT = 0x0,
        WINEVENT_SKIPOWNTHREAD = 0x1,
        WINEVENT_SKIPOWNPROCESS = 0x2,
        WINEVENT_INCONTEXT = 0x4
    }

    public enum SetWinEventHookStandardObjectId : int
    {
        OBJID_SELF = 0,
        OBJID_SYSMENU = -1,
        OBJID_TITLEBAR = -2,
        OBJID_MENU = -3,
        OBJID_CLIENT = -4,
        OBJID_VSCROLL = -5,
        OBJID_HSCROLL = -6,
        OBJID_SIZEGRIP = -7,
        OBJID_CARET = -8,
        OBJID_CURSOR = -9,
        OBJID_ALERT = -10,
        OBJID_SOUND = -11,
        OBJID_QUERYCLASSNAMEIDX = -12,
        OBJID_NATIVEOM = -16
    }

    public enum SetWinEventHookEventType : uint
    {
        EVENT_MIN = 0x00000001,
        EVENT_MAX = 0x7FFFFFFF,
        EVENT_SYSTEM_SOUND = 0x00000001,
        EVENT_SYSTEM_ALERT = 0x00000002,
        EVENT_SYSTEM_FOREGROUND = 0x00000003,
        EVENT_SYSTEM_MENUSTART = 0x00000004,
        EVENT_SYSTEM_MENUEND = 0x00000005,
        EVENT_SYSTEM_MENUPOPUPSTART = 0x00000006,
        EVENT_SYSTEM_MENUPOPUPEND = 0x00000007,
        EVENT_SYSTEM_CAPTURESTART = 0x00000008,
        EVENT_SYSTEM_CAPTUREEND = 0x00000009,
        EVENT_SYSTEM_MOVESIZESTART = 0x0000000A,
        EVENT_SYSTEM_MOVESIZEEND = 0x0000000B,
        EVENT_SYSTEM_CONTEXTHELPSTART = 0x0000000C,
        EVENT_SYSTEM_CONTEXTHELPEND = 0x0000000D,
        EVENT_SYSTEM_DRAGDROPSTART = 0x0000000E,
        EVENT_SYSTEM_DRAGDROPEND = 0x0000000F,
        EVENT_SYSTEM_DIALOGSTART = 0x00000010,
        EVENT_SYSTEM_DIALOGEND = 0x00000011,
        EVENT_SYSTEM_SCROLLINGSTART = 0x00000012,
        EVENT_SYSTEM_SCROLLINGEND = 0x00000013,
        EVENT_SYSTEM_SWITCHSTART = 0x00000014,
        EVENT_SYSTEM_SWITCHEND = 0x00000015,
        EVENT_SYSTEM_MINIMIZESTART = 0x00000016,
        EVENT_SYSTEM_MINIMIZEEND = 0x00000017,
        EVENT_CONSOLE_CARET = 0x00004001,
        EVENT_CONSOLE_UPDATE_REGION = 0x00004002,
        EVENT_CONSOLE_UPDATE_SIMPLE = 0x00004003,
        EVENT_CONSOLE_UPDATE_SCROLL = 0x00004004,
        EVENT_CONSOLE_LAYOUT = 0x00004005,
        EVENT_CONSOLE_START_APPLICATION = 0x00004006,
        EVENT_CONSOLE_END_APPLICATION = 0x00004007,
        EVENT_OBJECT_CREATE = 0x00008000,
        EVENT_OBJECT_DESTROY = 0x00008001,
        EVENT_OBJECT_SHOW = 0x00008002,
        EVENT_OBJECT_HIDE = 0x00008003,
        EVENT_OBJECT_REORDER = 0x00008004,
        EVENT_OBJECT_FOCUS = 0x00008005,
        EVENT_OBJECT_SELECTION = 0x00008006,
        EVENT_OBJECT_SELECTIONADD = 0x00008007,
        EVENT_OBJECT_SELECTIONREMOVE = 0x00008008,
        EVENT_OBJECT_SELECTIONWITHIN = 0x00008009,
        EVENT_OBJECT_STATECHANGE = 0x0000800A,
        EVENT_OBJECT_LOCATIONCHANGE = 0x0000800B,
        EVENT_OBJECT_NAMECHANGE = 0x0000800C,
        EVENT_OBJECT_DESCRIPTIONCHANGE = 0x0000800D,
        EVENT_OBJECT_VALUECHANGE = 0x0000800E,
        EVENT_OBJECT_PARENTCHANGE = 0x0000800F,
        EVENT_OBJECT_HELPCHANGE = 0x00008010,
        EVENT_OBJECT_DEFACTIONCHANGE = 0x00008011,
        EVENT_OBJECT_ACCELERATORCHANGE = 0x00008012
    }
}
