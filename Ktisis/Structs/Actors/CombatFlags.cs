// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Actors.CombatFlags
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;

#nullable disable
namespace Ktisis.Structs.Actors;

[Flags]
public enum CombatFlags : byte
{
  None = 0,
  WeaponDrawn = 64, // 0x40
}
