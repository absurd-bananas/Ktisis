// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Common.ColorHDR
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System.Numerics;
using System.Runtime.InteropServices;

#nullable disable
namespace Ktisis.Structs.Common;

[StructLayout(LayoutKind.Explicit, Size = 16 /*0x10*/)]
public struct ColorHDR
{
  [FieldOffset(0)]
  public Vector3 _vec3;
  [FieldOffset(0)]
  public float Red;
  [FieldOffset(4)]
  public float Green;
  [FieldOffset(8)]
  public float Blue;
  [FieldOffset(12)]
  public float Intensity;

  public Vector3 RGB
  {
    get => Vector3.SquareRoot(this._vec3) / 4f;
    set
    {
      value *= 4f;
      this._vec3 = value * value;
    }
  }

  public ColorHDR()
  {
    this.Red = 0.0f;
    this.Green = 0.0f;
    this.Blue = 0.0f;
    this._vec3 = new Vector3(16f, 16f, 16f);
    this.Intensity = 1f;
  }
}
