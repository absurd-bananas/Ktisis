// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Input.KeyboardDeviceData
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game.ClientState.Keys;
using System;

#nullable disable
namespace Ktisis.Structs.Input;

public struct KeyboardDeviceData
{
  public const int Length = 160 /*0xA0*/;
  public byte IsKeyPressed;
  public unsafe fixed uint KeyMap[160];
  public KeyboardQueue Queue;
  public int KeyboardQueueCount;
  public int ControllerQueueCount;

  public unsafe bool IsKeyDown(VirtualKey key, bool consume = false)
  {
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    int num = ^(uint&) ((IntPtr) this.KeyMap + (IntPtr) key * 4) > 0U ? 1 : 0;
    if ((num & (consume ? 1 : 0)) == 0)
      return num != 0;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(int&) ((IntPtr) this.KeyMap + (IntPtr) key * 4) = 0;
    return num != 0;
  }
}
