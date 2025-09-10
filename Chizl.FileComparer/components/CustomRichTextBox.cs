using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Chizl.Components
{
    public class CustomRichTextBox : RichTextBox
    {
        const int SB_LINEUP = 0;
        const int SB_LINELEFT = 0;
        const int SB_LINEDOWN = 1;
        const int SB_LINERIGHT = 1;

        const int WS_VSCROLL = 0x200000;
        const int WS_HSCROLL = 0x100000;

        // Import the Win32 function to send a message to the control.
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        // Constant for the Win32 message to show or hide a scrollbar.
        private const int WM_SHOWSCROLLBAR = 0x00E3;

        // Override theCreateParams to hide the native scrollbar.
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // Set a style flag to hide the vertical scrollbar.
                
                //cp.Style &= ~WS_VSCROLL; // Remove
                //cp.Style &= ~WS_HSCROLL; // Remove

                cp.Style |= WS_VSCROLL;  // Add
                cp.Style |= WS_HSCROLL;  // Add
                return cp;
            }
        }

        public CustomRichTextBox() : base()
        {
            // Set the scrollbar property to vertical so we can handle it ourselves.
            this.ScrollBars = RichTextBoxScrollBars.Vertical;
        }

        // Handle the mouse wheel event to enable scrolling.
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e); // Propagate the event

            // Send scroll messages based on mouse wheel movement
            int scrollLines = SystemInformation.MouseWheelScrollLines;
            
            if (e.Delta > 0) // Scroll up
                SendMessage(this.Handle, 0x0115, (IntPtr)SB_LINEUP, IntPtr.Zero); // SB_LINEUP
            else // Scroll down
                SendMessage(this.Handle, 0x0115, (IntPtr)SB_LINEDOWN, IntPtr.Zero); // SB_LINEDOWN
        }
    }
}
