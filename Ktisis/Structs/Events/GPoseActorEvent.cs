// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Events.GPoseActorEvent
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Ktisis.Structs.Events;

[StructLayout(LayoutKind.Explicit, Size = 304)]
public struct GPoseActorEvent
{
  [FieldOffset(0)]
  public unsafe IntPtr* __vfTable;
  [FieldOffset(32 /*0x20*/)]
  public ulong EntityID;
  [FieldOffset(208 /*0xD0*/)]
  public unsafe FFXIVClientStructs.FFXIV.Client.Game.Character.Character* Character;
  [FieldOffset(224 /*0xE0*/)]
  public uint ObjectID;
  [FieldOffset(264)]
  public uint _param4;
  [FieldOffset(268)]
  public uint _param5;
  [FieldOffset(272)]
  public uint _param6;
  [FieldOffset(276)]
  public uint CopyObjectIndex;
}
