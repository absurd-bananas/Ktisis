// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.GPose.GPoseState
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.Interop;
using Ktisis.Structs.Lights;
using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Ktisis.Structs.GPose;

[StructLayout(LayoutKind.Explicit)]
public struct GPoseState
{
  private const int LightCount = 3;
  [FieldOffset(224 /*0xE0*/)]
  public unsafe fixed ulong Lights[3];
  [FieldOffset(480)]
  public unsafe GameObject* GPoseTarget;

  public unsafe SceneLight* GetLight(uint index)
  {
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    return (SceneLight*) ^(long&) ((IntPtr) this.Lights + (IntPtr) ((long) index * 8L));
  }

  public unsafe Span<Pointer<SceneLight>> GetLights()
  {
    fixed (ulong* numPtr = this.Lights)
      return new Span<Pointer<SceneLight>>((void*) numPtr, 3);
  }
}
