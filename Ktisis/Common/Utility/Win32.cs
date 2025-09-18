// Decompiled with JetBrains decompiler
// Type: KtisisPyon.Common.Utility.Win32
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
using Ktisis.Data.Config.Sections;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;

#nullable enable
namespace KtisisPyon.Common.Utility;

public static class Win32
{
  private const int SW_MAXIMIZE = 3;
  private const int SW_RESTORE = 9;
  private static IntPtr GameWindowHandle = IntPtr.Zero;

  [DllImport("user32.dll", SetLastError = true)]
  private static extern bool GetWindowRect(IntPtr hwnd, out Win32.RECT lpRect);

  [DllImport("user32.dll", SetLastError = true)]
  private static extern bool SetWindowPos(
    IntPtr hWnd,
    IntPtr hWndInsertAfter,
    int x,
    int y,
    int width,
    int height,
    uint uFlags);

  [DllImport("user32.DLL")]
  private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

  [DllImport("user32.DLL")]
  private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

  [DllImport("user32.dll")]
  private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

  public static unsafe (Point pos, Size size, int style, Size deviceSize) GetWinProperties()
  {
    if (Win32.GameWindowHandle == IntPtr.Zero)
      Win32.GameWindowHandle = Process.GetCurrentProcess().MainWindowHandle;
    Win32.RECT lpRect;
    Win32.GetWindowRect(Win32.GameWindowHandle, out lpRect);
    int windowLong = Win32.GetWindowLong(Win32.GameWindowHandle, -16);
    Device* devicePtr = Device.Instance();
    return (new Point(lpRect.X, lpRect.Y), new Size(lpRect.Width, lpRect.Height), windowLong, new Size((int) devicePtr->Width, (int) devicePtr->Height));
  }

  public static unsafe void SetWinDefault(PyonConfig cfg)
  {
    if (cfg.DefaultSize == Size.Empty)
      return;
    (Point pos, Size size, int style, Size deviceSize) = Win32.GetWinProperties();
    if (cfg.DefaultDeviceSize != deviceSize)
    {
      Device* devicePtr = Device.Instance();
      int width = cfg.DefaultDeviceSize.Width;
      devicePtr->NewWidth = (uint) width;
      int height = cfg.DefaultDeviceSize.Height;
      devicePtr->NewHeight = (uint) height;
      devicePtr->RequestResolutionChange = (byte) 1;
    }
    if (cfg.DefaultStyle == style && !(cfg.DefaultPosition != pos) && !(cfg.DefaultSize != size))
      return;
    if (((Win32.WindowStyles) cfg.DefaultStyle).HasFlag((Enum) Win32.WindowStyles.WS_MAXIMIZE))
    {
      Win32.SetWindowLong(Win32.GameWindowHandle, -16, cfg.DefaultStyle);
      Win32.ShowWindow(Win32.GameWindowHandle, 3);
      Win32.SetWindowPos(Win32.GameWindowHandle, IntPtr.Zero, cfg.DefaultPosition.X, cfg.DefaultPosition.Y, cfg.DefaultSize.Width + cfg.DefaultPosition.X, cfg.DefaultSize.Height + cfg.DefaultPosition.Y, 36U);
      Win32.SetWindowLong(Win32.GameWindowHandle, -16, cfg.DefaultStyle);
      Win32.ShowWindow(Win32.GameWindowHandle, 3);
    }
    else
    {
      Win32.SetWindowLong(Win32.GameWindowHandle, -16, cfg.DefaultStyle);
      Win32.ShowWindow(Win32.GameWindowHandle, 9);
      Win32.SetWindowPos(Win32.GameWindowHandle, IntPtr.Zero, cfg.DefaultPosition.X, cfg.DefaultPosition.Y, cfg.DefaultSize.Width + cfg.DefaultPosition.X, cfg.DefaultSize.Height + cfg.DefaultPosition.Y, 36U);
      Win32.SetWindowLong(Win32.GameWindowHandle, -16, cfg.DefaultStyle);
      Win32.ShowWindow(Win32.GameWindowHandle, 9);
    }
  }

  public static unsafe void SetWinRes(PyonConfig cfg)
  {
    if (cfg.HiResSize == Size.Empty)
      return;
    Win32.GetWinProperties();
    Device* devicePtr = Device.Instance();
    if ((int) devicePtr->Width != cfg.HiResSize.Width || (int) devicePtr->Height != cfg.HiResSize.Height)
    {
      Win32.SetWindowLong(Win32.GameWindowHandle, -16, cfg.DefaultStyle & 16777216 /*0x01000000*/);
      Win32.ShowWindow(Win32.GameWindowHandle, 3);
      devicePtr->NewWidth = (uint) cfg.HiResSize.Width;
      devicePtr->NewHeight = (uint) cfg.HiResSize.Height;
      devicePtr->RequestResolutionChange = (byte) 1;
      Win32.SetWindowLong(Win32.GameWindowHandle, -16, cfg.DefaultStyle & -29360129);
      Win32.ShowWindow(Win32.GameWindowHandle, 9);
    }
    else
      Win32.SetWinDefault(cfg);
  }

  public struct RECT(int left, int top, int right, int bottom)
  {
    public int Left = left;
    public int Top = top;
    public int Right = right;
    public int Bottom = bottom;

    public RECT(Rectangle r)
      : this(r.Left, r.Top, r.Right, r.Bottom)
    {
    }

    public int X
    {
      get => this.Left;
      set
      {
        this.Right -= this.Left - value;
        this.Left = value;
      }
    }

    public int Y
    {
      get => this.Top;
      set
      {
        this.Bottom -= this.Top - value;
        this.Top = value;
      }
    }

    public int Height
    {
      get => this.Bottom - this.Top;
      set => this.Bottom = value + this.Top;
    }

    public int Width
    {
      get => this.Right - this.Left;
      set => this.Right = value + this.Left;
    }

    public Point Location
    {
      get => new Point(this.Left, this.Top);
      set
      {
        this.X = value.X;
        this.Y = value.Y;
      }
    }

    public Size Size
    {
      get => new Size(this.Width, this.Height);
      set
      {
        this.Width = value.Width;
        this.Height = value.Height;
      }
    }

    public static implicit operator Rectangle(Win32.RECT r)
    {
      return new Rectangle(r.Left, r.Top, r.Width, r.Height);
    }

    public static implicit operator Win32.RECT(Rectangle r) => new Win32.RECT(r);

    public static bool operator ==(Win32.RECT r1, Win32.RECT r2) => r1.Equals(r2);

    public static bool operator !=(Win32.RECT r1, Win32.RECT r2) => !r1.Equals(r2);

    public bool Equals(Win32.RECT r)
    {
      return r.Left == this.Left && r.Top == this.Top && r.Right == this.Right && r.Bottom == this.Bottom;
    }

    public override bool Equals(object obj)
    {
      switch (obj)
      {
        case Win32.RECT r1:
          return this.Equals(r1);
        case Rectangle r2:
          return this.Equals(new Win32.RECT(r2));
        default:
          return false;
      }
    }

    public override int GetHashCode() => ((Rectangle) this).GetHashCode();

    public override string ToString()
    {
      CultureInfo currentCulture = CultureInfo.CurrentCulture;
      \u003C\u003Ey__InlineArray4<object> buffer = new \u003C\u003Ey__InlineArray4<object>();
      // ISSUE: reference to a compiler-generated method
      \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray4<object>, object>(ref buffer, 0) = (object) this.Left;
      // ISSUE: reference to a compiler-generated method
      \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray4<object>, object>(ref buffer, 1) = (object) this.Top;
      // ISSUE: reference to a compiler-generated method
      \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray4<object>, object>(ref buffer, 2) = (object) this.Right;
      // ISSUE: reference to a compiler-generated method
      \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray4<object>, object>(ref buffer, 3) = (object) this.Bottom;
      // ISSUE: reference to a compiler-generated method
      ReadOnlySpan<object> readOnlySpan = \u003CPrivateImplementationDetails\u003E.InlineArrayAsReadOnlySpan<\u003C\u003Ey__InlineArray4<object>, object>(in buffer, 4);
      return string.Format((IFormatProvider) currentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", readOnlySpan);
    }
  }

  public struct POINT(int x, int y)
  {
    public int X = x;
    public int Y = y;

    public static implicit operator Point(Win32.POINT p) => new Point(p.X, p.Y);

    public static implicit operator Win32.POINT(Point p) => new Win32.POINT(p.X, p.Y);

    public override string ToString() => $"X: {this.X}, Y: {this.Y}";
  }

  [Flags]
  public enum WindowStyles : uint
  {
    WS_BORDER = 8388608, // 0x00800000
    WS_CAPTION = 12582912, // 0x00C00000
    WS_CHILD = 1073741824, // 0x40000000
    WS_CLIPCHILDREN = 33554432, // 0x02000000
    WS_CLIPSIBLINGS = 67108864, // 0x04000000
    WS_DISABLED = 134217728, // 0x08000000
    WS_DLGFRAME = 4194304, // 0x00400000
    WS_GROUP = 131072, // 0x00020000
    WS_HSCROLL = 1048576, // 0x00100000
    WS_MAXIMIZE = 16777216, // 0x01000000
    WS_MAXIMIZEBOX = 65536, // 0x00010000
    WS_MINIMIZE = 536870912, // 0x20000000
    WS_MINIMIZEBOX = WS_GROUP, // 0x00020000
    WS_OVERLAPPED = 0,
    WS_OVERLAPPEDWINDOW = 13565952, // 0x00CF0000
    WS_POPUP = 2147483648, // 0x80000000
    WS_POPUPWINDOW = 2156396544, // 0x80880000
    WS_SIZEFRAME = 262144, // 0x00040000
    WS_SYSMENU = 524288, // 0x00080000
    WS_TABSTOP = WS_MAXIMIZEBOX, // 0x00010000
    WS_VISIBLE = 268435456, // 0x10000000
    WS_VSCROLL = 2097152, // 0x00200000
  }

  [Flags]
  public enum SWP : uint
  {
    NOSIZE = 1,
    NOMOVE = 2,
    NOZORDER = 4,
    NOREDRAW = 8,
    NOACTIVATE = 16, // 0x00000010
    DRAWFRAME = 32, // 0x00000020
    FRAMECHANGED = DRAWFRAME, // 0x00000020
    SHOWWINDOW = 64, // 0x00000040
    HIDEWINDOW = 128, // 0x00000080
    NOCOPYBITS = 256, // 0x00000100
    NOOWNERZORDER = 512, // 0x00000200
    NOREPOSITION = NOOWNERZORDER, // 0x00000200
    NOSENDCHANGING = 1024, // 0x00000400
    DEFERERASE = 8192, // 0x00002000
    ASYNCWINDOWPOS = 16384, // 0x00004000
  }

  public enum GWL
  {
    USERDATA = -21, // 0xFFFFFFEB
    EXSTYLE = -20, // 0xFFFFFFEC
    STYLE = -16, // 0xFFFFFFF0
    ID = -12, // 0xFFFFFFF4
    HWNDPARENT = -8, // 0xFFFFFFF8
    HINSTANCE = -6, // 0xFFFFFFFA
    WNDPROC = -4, // 0xFFFFFFFC
  }
}
