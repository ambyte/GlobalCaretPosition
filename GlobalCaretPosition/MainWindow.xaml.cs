using System;
using System.ComponentModel;
using System.Windows;
using Accessibility;

namespace GlobalCaretPosition
{
    public partial class MainWindow : Window
    {
        private bool _started;
        private readonly EventGrabber _eventGrabber;
        private readonly CaretTrackWindow _caretTrackWindow;


        public MainWindow()
        {
            InitializeComponent();

            Closing += MainWindow_Closing;
            _caretTrackWindow = new CaretTrackWindow();
            _eventGrabber = new EventGrabber();
            _eventGrabber.Grabbed += eventGrabber_Grabbed;
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            _caretTrackWindow.Close();
            if (_started)
            {
                _eventGrabber.Stop();
            }
        }

        void eventGrabber_Grabbed(object sender, EventGrabber.GrabbedEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new EventHandler<EventGrabber.GrabbedEventArgs>(eventGrabber_Grabbed), new object[] { sender, e });
            }
            else
            {
                if ((e.EventType == SetWinEventHookEventType.EVENT_OBJECT_LOCATIONCHANGE) && (e.ObjectId == -8))
                {

                    IAccessible acc = null;
                    FromEvent(e.Window, e.ObjectId, 0, out acc);
                    if (acc != null)
                    {
                        int left = 0;
                        int top = 0;
                        int width = 0;
                        int height = 0;
                        acc.accLocation(out left, out top, out width, out height);
                        if (left != 0 && top != 0)
                        {
                            ShowCaretTrackWindow(left, top);
                        }
                        else
                        {
                            _caretTrackWindow.Hide();
                        }
                    }
                }
            }
        }

        public static int FromEvent(IntPtr hwnd, int idObject, int idChild, out IAccessible accessible)
        {
            accessible = (IAccessible)null;
            object varChild = (object)null;
            IAccessible ppvObject = (IAccessible)null;
            int num = (int) Win32Api.AccessibleObjectFromEvent(hwnd, (uint) idObject, (uint) idChild, out ppvObject, out varChild);
            if (ppvObject != null)
                accessible = ppvObject;
            return num;
        }

        void ShowCaretTrackWindow(int left, int top)
        {
            tLeft.Text = left.ToString();
            tTop.Text = top.ToString();
            _caretTrackWindow.Show();
            _caretTrackWindow.top.Content = "Top: " + top;
            _caretTrackWindow.left.Content = "Left: " + left;
            SetLocationResultWindow(_caretTrackWindow, left, top);
        }

        static public void SetLocationResultWindow(Window window, int left, int top)
        {
            Point point = new Point(left,top);
            var screenHeight = (int)SystemParameters.PrimaryScreenHeight;
            var screenWidth = Math.Abs((int)SystemParameters.PrimaryScreenWidth);

            var childHeight = (int)window.Height;
            var childWidth = (int)window.Width;

            int childPosX;
            int childPosY;

            if (screenHeight < point.Y + childHeight)
            {
                childPosY = (int) (point.Y - childHeight - 50);
            }
            else
            {
                childPosY = (int) (point.Y + 10);

            }
            if (screenWidth < point.X + 10 + childWidth)
            {
                childPosX = (int) (point.X - childWidth - 10);
            }
            else
            {
                childPosX = (int) (point.X + 10);
            }
            window.Left = childPosX;
            window.Top = childPosY;

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (_started)
            {
                button.Content = "Start";
                _eventGrabber.Stop();
                tLeft.Text = "-";
                tTop.Text = "-";
            }
            else
            {
                button.Content = "Stop";
                _eventGrabber.Start();
            }
            _started = !_started;
        }

      

        
    }

}
