// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Characters.CharacterBaseEx
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using Ktisis.Structs.Attachment;
using System.Runtime.InteropServices;

#nullable disable
namespace Ktisis.Structs.Characters;

[StructLayout(LayoutKind.Explicit, Size = 2576)]
public struct CharacterBaseEx
{
  [FieldOffset(0)]
  public CharacterBase Base;
  [FieldOffset(80 /*0x50*/)]
  public Transform Transform;
  [FieldOffset(216)]
  public Attach Attach;
  [FieldOffset(736)]
  public WetnessState Wetness;
  [FieldOffset(2592)]
  public CustomizeContainer Customize;
  [FieldOffset(2624)]
  public EquipmentContainer Equipment;
}
