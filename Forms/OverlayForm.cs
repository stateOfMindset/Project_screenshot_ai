using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

public class OverlayForm : Form
{
    // Win32 constants
    private const int WS_EX_TRANSPARENT = 0x20;
    private const int WS_EX_LAYERED = 0x80000;
    private const int GWL_EXSTYLE = -20;

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    public Rectangle SelectionRectangle { get; private set; }
    private Point startPoint;
    private bool isDragging = false;
    private bool isSelectionFinalized = false;

    public OverlayForm()
    {
        this.FormBorderStyle = FormBorderStyle.None;
        this.BackColor = Color.Red; // Fully transparent background
        this.TransparencyKey = Color.Red;
        this.TopMost = true;
        this.WindowState = FormWindowState.Maximized;
        this.Opacity = 1;
        this.DoubleBuffered = true;
        this.ShowInTaskbar = false;
        this.Cursor = Cursors.Cross;
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (!isSelectionFinalized && e.Button == MouseButtons.Left)
        {
            isDragging = true;
            startPoint = e.Location;
            SelectionRectangle = new Rectangle(e.Location, new Size(0, 0));
            Invalidate();
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (isDragging && !isSelectionFinalized)
        {
            SelectionRectangle = new Rectangle(
                Math.Min(startPoint.X, e.X),
                Math.Min(startPoint.Y, e.Y),
                Math.Abs(startPoint.X - e.X),
                Math.Abs(startPoint.Y - e.Y));
            Invalidate();
        }
    }

    public event EventHandler SelectionFinalized;

    protected override void OnMouseUp(MouseEventArgs e)
    {
        if (isDragging && !isSelectionFinalized)
        {
            isDragging = false;
            isSelectionFinalized = true;
            Invalidate();

            // Make overlay click-through
            int exStyle = GetWindowLong(this.Handle, GWL_EXSTYLE);
            SetWindowLong(this.Handle, GWL_EXSTYLE, exStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT);

            SelectionFinalized?.Invoke(this, EventArgs.Empty);
       
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        if (SelectionRectangle.Width > 0 && SelectionRectangle.Height > 0)
        {
            using (var pen = new Pen(Color.Lime, 4))
            {
                e.Graphics.DrawRectangle(pen, SelectionRectangle);
            }

            
        }
    }
}