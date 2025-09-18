// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Characters.EquipmentContainer
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.FFXIV.Client.Game.Character;
using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Ktisis.Structs.Characters;

[StructLayout(LayoutKind.Explicit, Size = 80 /*0x50*/)]
public struct EquipmentContainer
{
  public const int Length = 10;
  public const int Size = 80 /*0x50*/;
  [FieldOffset(0)]
  public unsafe fixed byte Bytes[80];
  [FieldOffset(0)]
  public EquipmentModelId Head;
  [FieldOffset(8)]
  public EquipmentModelId Chest;
  [FieldOffset(16 /*0x10*/)]
  public EquipmentModelId Hands;
  [FieldOffset(24)]
  public EquipmentModelId Legs;
  [FieldOffset(32 /*0x20*/)]
  public EquipmentModelId Feet;
  [FieldOffset(40)]
  public EquipmentModelId Earring;
  [FieldOffset(48 /*0x30*/)]
  public EquipmentModelId Necklace;
  [FieldOffset(56)]
  public EquipmentModelId Bracelet;
  [FieldOffset(64 /*0x40*/)]
  public EquipmentModelId RingRight;
  [FieldOffset(72)]
  public EquipmentModelId RingLeft;

  public EquipmentModelId this[uint index]
  {
    get => this.Get(index);
    set => this.Set(index, value);
  }

  private unsafe EquipmentModelId Get(uint index) => *this.GetData(index);

  private unsafe void Set(uint index, EquipmentModelId equip) => *this.GetData(index) = equip;

  public unsafe EquipmentModelId* GetData(uint index)
  {
    if (index >= 10U)
      throw new IndexOutOfRangeException($"Index {index} is out of range (< {10}).");
    fixed (byte* numPtr = this.Bytes)
      return (EquipmentModelId*) (numPtr + (long) index * (long) sizeof (EquipmentModelId));
  }
}
