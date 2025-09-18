// Decompiled with JetBrains decompiler
// Type: Ktisis.Common.Extensions.NumericEx
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System;

namespace Ktisis.Common.Extensions;

public static class NumericEx {
	public static byte GetAlpha(this uint rgba) => (byte)(rgba & 4278190080U /*0xFF000000*/);

	public static uint SetAlpha(this uint rgba, byte alpha) => (uint)((int)rgba & 16777215 /*0xFFFFFF*/ | alpha << 24);

	public static uint SetAlpha(this uint rgba, float alpha) => rgba.SetAlpha((byte)Math.Floor(alpha * (double)byte.MaxValue));

	public static uint FlipEndian(this uint value) =>
		(uint)((int)((value & 4278190080U /*0xFF000000*/) >> 24) | (int)((value & 16711680U /*0xFF0000*/) >> 16 /*0x10*/) << 8 | (int)((value & 65280U) >> 8) << 16 /*0x10*/ | ((int)value & byte.MaxValue) << 24);
}
