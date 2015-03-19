using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace GlobalCaretPosition
{
    sealed class EventGrabber : IDisposable
    {
        public sealed class GrabbedEventArgs : EventArgs
        {
            public SetWinEventHookEventType EventType { get; set; }
            public int ChildId { get; set; }
            public uint ThreadId { get; set; }
            public uint Time { get; set; }

            private int _objectId;
            public int ObjectId
            {
                get
                {
                    return _objectId;
                }
                set
                {
                    _objectId = value;

                    if (Enum.IsDefined(typeof(SetWinEventHookStandardObjectId), _objectId))
                    {
                        StandardObject = (SetWinEventHookStandardObjectId)_objectId;
                    }
                    else
                    {
                        StandardObject = null;
                    }
                }
            }

            public SetWinEventHookStandardObjectId? StandardObject { get; private set; }

            private IntPtr window;
            public IntPtr Window
            {
                get
                {
                    return window;
                }
                set
                {
                    window = value;

                    uint processId;
                    Win32Api.GetWindowThreadProcessId(Window, out processId);
                    this.processId = processId;
                }
            }

            private uint processId;
            public uint ProcessId
            {
                get
                {
                    if (Window == IntPtr.Zero)
                    {
                        throw new InvalidOperationException("Window was not set");
                    }
                    else
                    {
                        if (processId == 0)
                        {
                            throw new PInvokeException("Unable to get process id from Window");
                        }

                        return processId;
                    }
                }
            }
        }


        private bool _started;
        private object _stateLock;

        private GCHandle _handleHook;
        private IntPtr _hook;

        private bool? _disposed;


        public SetWinEventHookEventType MinEventType { get; set; }
        public SetWinEventHookEventType MaxEventType { get; set; }
        public uint? ProcessId { get; set; }
        public uint? ThreadId { get; set; }

        public bool IsStarted
        {
            get
            {
                lock (_stateLock)
                {
                    return _started;
                }
            }
        }


        public event EventHandler Started;
        public event EventHandler Stopped;
        public event EventHandler<GrabbedEventArgs> Grabbed;


        public EventGrabber()
        {
            _started = false;
            _stateLock = new object();

            _handleHook = GCHandle.Alloc(new SetWinEventHookDelegate(HandleHook));

            _disposed = false;

            MinEventType = SetWinEventHookEventType.EVENT_MIN;
            MaxEventType = SetWinEventHookEventType.EVENT_MAX;
        }

        ~EventGrabber()
        {
            Dispose(false);
        }


        public void Start()
        {
            lock (_stateLock)
            {
                if (_started)
                {
                    throw new InvalidOperationException("Already started");
                }

                if ((uint)MinEventType > (uint)MaxEventType)
                {
                    throw new InvalidOperationException("MinEventType is greater then MaxEventType");
                }

                _hook = Win32Api.SetWinEventHook(MinEventType,
                    MaxEventType, IntPtr.Zero, (SetWinEventHookDelegate)_handleHook.Target,
                    0, 0,
                    SetWinEventHookFlag.WINEVENT_OUTOFCONTEXT | SetWinEventHookFlag.WINEVENT_SKIPOWNPROCESS);

                if (_hook == IntPtr.Zero)
                {
                    throw new PInvokeException("Unable to set event hook");
                }

                _started = true;

                if (Started != null)
                {
                    Started(this, EventArgs.Empty);
                }
            }
        }

        public void Stop()
        {
            lock (_stateLock)
            {
                if (!_started)
                {
                    throw new InvalidOperationException("Already stopped");
                }

                if (!Win32Api.UnhookWinEvent(_hook))
                {
                    throw new PInvokeException("Unable to remove event hook");
                }

                _started = false;

                if (Stopped != null)
                {
                    Stopped(this, EventArgs.Empty);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        private void HandleHook(IntPtr hook, SetWinEventHookEventType eventType,
            IntPtr window, int objectId, int childId, uint threadId, uint time)
        {
            if (_disposed == false)
            {
                lock (_stateLock)
                {
                    if (_started)
                    {
                        var eventArgs = new GrabbedEventArgs
                        {
                            EventType = eventType,
                            Window = window,
                            ObjectId = objectId,
                            ChildId = childId,
                            ThreadId = threadId,
                            Time = time
                        };

                        if (Grabbed != null)
                        {
                            Grabbed(this, eventArgs);
                        }
                    }
                }
            }
        }

        private void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                _disposed = null;

                if (disposing)
                {
                    //dispose managed resources
                }

                //dispose unmanaged resources
                lock (_stateLock)
                {
                    if (_started)
                    {
                        Win32Api.UnhookWinEvent(_hook);
                    }
                }
                _handleHook.Free();

                _disposed = true;
            }
        }
    }
}
