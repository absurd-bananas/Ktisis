// Decompiled with JetBrains decompiler
// Type: Ktisis.Common.Extensions.NumericEx
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;

#nullable disable
namespace Ktisis.Common.Extensions;

public static class NumericEx
{
  public static byte GetAlpha(this uint rgba) => (byte) (rgba & 4278190080U /*0xFF000000*/);

  public static uint SetAlpha(this uint rgba, byte alpha)
  {
    return (uint) ((int) rgba & 16777215 /*0xFFFFFF*/ | (int) alpha << 24);
  }

  public static uint SetAlpha(this uint rgba, float alpha)
  {
    return rgba.SetAlpha((byte) Math.Floor((double) alpha * (double) byte.MaxValue));
  }

  public static uint FlipEndian(this uint value)
  {
    return (uint) ((int) ((value & 4278190080U /*0xFF000000*/) >> 24) | (int) ((value & 16711680U /*0xFF0000*/) >> 16 /*0x10*/) << 8 | (int) ((value & 65280U) >> 8) << 16 /*0x10*/ | ((int) value & (int) byte.MaxValue) << 24);
  }
}
