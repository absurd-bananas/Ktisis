// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Posing.Data.PoseTransforms
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;

#nullable disable
namespace Ktisis.Editor.Posing.Data;

[Flags]
public enum PoseTransforms
{
  None = 0,
  Rotation = 1,
  Position = 2,
  Scale = 4,
  PositionRoot = 8,
}
