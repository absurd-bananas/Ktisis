// Decompiled with JetBrains decompiler
// Type: Ktisis.GameData.Chara.CommonColors
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;

#nullable enable
namespace Ktisis.GameData.Chara;

public struct CommonColors
{
  public uint[] EyeColors;
  public uint[] HighlightColors;
  public uint[] LipColors;
  public uint[] FaceFeatureColors;
  public uint[] FacepaintColors;

  public CommonColors()
  {
    this.EyeColors = Array.Empty<uint>();
    this.HighlightColors = Array.Empty<uint>();
    this.LipColors = Array.Empty<uint>();
    this.FaceFeatureColors = Array.Empty<uint>();
    this.FacepaintColors = Array.Empty<uint>();
  }
}
