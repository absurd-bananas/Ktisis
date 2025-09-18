// Decompiled with JetBrains decompiler
// Type: Ktisis.Common.Utility.KeyHelpers
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Ktisis.Common.Utility;

public static class KeyHelpers
{
  public static bool IsModifierKey(VirtualKey key) => key - 16 /*0x10*/ <= 2;

  public static IEnumerable<VirtualKey> GetKeysDown()
  {
    ImGuiIOPtr io = Dalamud.Bindings.ImGui.ImGui.GetIO();
    if (((ImGuiIOPtr) ref io).KeyCtrl)
      yield return (VirtualKey) 17;
    if (((ImGuiIOPtr) ref io).KeyShift)
      yield return (VirtualKey) 16 /*0x10*/;
    if (((ImGuiIOPtr) ref io).KeyAlt)
      yield return (VirtualKey) 18;
    int i = 0;
    while (true)
    {
      int num = i;
      Span<bool> keysDown = ((ImGuiIOPtr) ref io).KeysDown;
      int length = keysDown.Length;
      if (num < length)
      {
        keysDown = ((ImGuiIOPtr) ref io).KeysDown;
        if (keysDown[i])
        {
          VirtualKey virtualKey = ImGuiHelpers.ImGuiKeyToVirtualKey((ImGuiKey) i);
          if ((virtualKey < 160 /*0xA0*/ || virtualKey > 165) && virtualKey != null)
            yield return virtualKey;
        }
        ++i;
      }
      else
        break;
    }
  }
}
