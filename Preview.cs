using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ClockSaver;

/// <summary>Fits the Clock artwork into the preview window owned by Windows.</summary>
internal sealed class Preview : Control
{
    private readonly IntPtr parent;
    private readonly Bitmap artwork;

    private Preview(IntPtr parent)
    {
        this.parent = parent;
        BackColor = Color.Black;
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ClockPreview.jpg");
        using var image = Image.FromStream(stream!);
        artwork = new Bitmap(image);
    }

    public static void Run(IntPtr parent)
    {
        using var preview = new Preview(parent);
        using var context = new ApplicationContext();
        using var watch = new Timer { Interval = 250 };

        bool Resize()
        {
            if (!GetClientRect(parent, out var bounds)) return false;
            preview.Size = new Size(bounds.Right, bounds.Bottom);
            return true;
        }

        if (!Resize()) return;
        preview.CreateControl();
        watch.Tick += (_, _) =>
        {
            if (!Resize()) context.ExitThread();
        };
        watch.Start();
        Application.Run(context);
    }

    protected override CreateParams CreateParams
    {
        get
        {
            var parameters = base.CreateParams;
            parameters.Parent = parent;
            parameters.Style = (parameters.Style & ~unchecked((int)0x80000000)) | 0x40000000; // WS_POPUP → WS_CHILD
            return parameters;
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var scale = Math.Min((double)Width / artwork.Width, (double)Height / artwork.Height);
        var width = (int)(artwork.Width * scale);
        var height = (int)(artwork.Height * scale);
        e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        e.Graphics.DrawImage(artwork, new Rectangle((Width - width) / 2, (Height - height) / 2, width, height));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) artwork.Dispose();
        base.Dispose(disposing);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        public int Left, Top, Right, Bottom;
    }

    [DllImport("user32.dll")]
    private static extern bool GetClientRect(IntPtr window, out Rect rect);
}
