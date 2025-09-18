// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Characters.FacialFeature
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;

#nullable disable
namespace Ktisis.Structs.Characters;

[Flags]
public enum FacialFeature : byte
{
  None = 0,
  First = 1,
  Second = 2,
  Third = 4,
  Fourth = 8,
  Fifth = 16, // 0x10
  Sixth = 32, // 0x20
  Seventh = 64, // 0x40
  Legacy = 128, // 0x80
}
