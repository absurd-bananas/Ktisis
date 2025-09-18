// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Actions.Input.InputModule
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game.ClientState.Keys;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using Ktisis.Interop.Hooking;
using System;
using System.Linq;

#nullable enable
namespace Ktisis.Editor.Actions.Input;

public class InputModule(IHookMediator hook) : HookModule(hook)
{
  [Signature("48 89 5C 24 ?? 55 56 57 41 56 41 57 48 8D 6C 24 ?? 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 45 40 4D 8B F9", DetourName = "InputNotificationDetour")]
  private Hook<InputModule.InputNotificationDelegate> InputNotificationHook;

  public event KeyEventHandler? OnKeyEvent;

  private bool InvokeKeyEvent(VirtualKey key, VirtualKeyState state)
  {
    return this.OnKeyEvent != null && this.OnKeyEvent.GetInvocationList().Cast<KeyEventHandler>().Aggregate<KeyEventHandler, bool>(false, (Func<bool, KeyEventHandler, bool>) ((result, handler) => result | handler(key, state)));
  }

  private IntPtr InputNotificationDetour(
    IntPtr hWnd,
    InputModule.WinMsg uMsg,
    IntPtr wParam,
    uint lParam)
  {
    VirtualKey key = (VirtualKey) (int) (ushort) wParam;
    switch (uMsg)
    {
      case InputModule.WinMsg.WM_KEYDOWN:
        if (this.InvokeKeyEvent(key, lParam >> 30 != 0U ? VirtualKeyState.Held : VirtualKeyState.Down))
          return IntPtr.Zero;
        break;
      case InputModule.WinMsg.WM_KEYUP:
        if (this.InvokeKeyEvent(key, VirtualKeyState.Released))
          return IntPtr.Zero;
        break;
    }
    return this.InputNotificationHook.Original(hWnd, uMsg, wParam, lParam);
  }

  private enum WinMsg : uint
  {
    WM_KEYDOWN = 256, // 0x00000100
    WM_KEYUP = 257, // 0x00000101
    WM_MOUSEMOVE = 512, // 0x00000200
  }

  private delegate IntPtr InputNotificationDelegate(
    IntPtr a1,
    InputModule.WinMsg a2,
    IntPtr a3,
    uint a4);
}
