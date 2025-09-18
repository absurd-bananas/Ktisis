// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Env.EnvState
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System.Runtime.InteropServices;

using Ktisis.Structs.Env.Weather;

namespace Ktisis.Structs.Env;

[StructLayout(LayoutKind.Explicit, Size = 760)]
public struct EnvState {
	[FieldOffset(8)]
	public uint SkyId;
	[FieldOffset(32 /*0x20*/)]
	public EnvLighting Lighting;
	[FieldOffset(152)]
	public EnvStars Stars;
	[FieldOffset(192 /*0xC0*/)]
	public EnvFog Fog;
	[FieldOffset(328)]
	public EnvClouds Clouds;
	[FieldOffset(368)]
	public EnvRain Rain;
	[FieldOffset(420)]
	public EnvDust Dust;
	[FieldOffset(472)]
	public EnvWind Wind;
}
