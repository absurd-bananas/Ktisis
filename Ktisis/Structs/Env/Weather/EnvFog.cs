// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Env.Weather.EnvFog
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System.Runtime.InteropServices;

namespace Ktisis.Structs.Env.Weather;

[StructLayout(LayoutKind.Explicit, Size = 40)]
public struct EnvFog {
	[FieldOffset(0)]
	public Vector4 Color;
	[FieldOffset(16 /*0x10*/)]
	public float Distance;
	[FieldOffset(20)]
	public float Thickness;
	[FieldOffset(24)]
	public float _unk1;
	[FieldOffset(28)]
	public float _unk2;
	[FieldOffset(32 /*0x20*/)]
	public float Opacity;
	[FieldOffset(36)]
	public float SkyVisibility;
}
