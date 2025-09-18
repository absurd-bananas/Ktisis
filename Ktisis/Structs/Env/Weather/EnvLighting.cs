// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Env.Weather.EnvLighting
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System.Runtime.InteropServices;

namespace Ktisis.Structs.Env.Weather;

[StructLayout(LayoutKind.Explicit, Size = 64 /*0x40*/)]
public struct EnvLighting {
	[FieldOffset(0)]
	public Vector3 SunLightColor;
	[FieldOffset(12)]
	public Vector3 MoonLightColor;
	[FieldOffset(24)]
	public Vector3 Ambient;
	[FieldOffset(36)]
	public float _unk1;
	[FieldOffset(40)]
	public float AmbientSaturation;
	[FieldOffset(44)]
	public float Temperature;
	[FieldOffset(48 /*0x30*/)]
	public float _unk2;
	[FieldOffset(52)]
	public float _unk3;
	[FieldOffset(56)]
	public float _unk4;
}
