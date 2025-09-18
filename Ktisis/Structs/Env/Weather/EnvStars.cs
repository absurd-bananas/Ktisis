// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Env.Weather.EnvStars
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System.Runtime.InteropServices;

namespace Ktisis.Structs.Env.Weather;

[StructLayout(LayoutKind.Explicit, Size = 40)]
public struct EnvStars {
	[FieldOffset(0)]
	public float ConstellationIntensity;
	[FieldOffset(4)]
	public float Constellations;
	[FieldOffset(8)]
	public float Stars;
	[FieldOffset(12)]
	public float GalaxyIntensity;
	[FieldOffset(16 /*0x10*/)]
	public float StarIntensity;
	[FieldOffset(20)]
	public Vector4 MoonColor;
	[FieldOffset(36)]
	public float MoonBrightness;
}
