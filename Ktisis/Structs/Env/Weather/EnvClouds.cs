// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Env.Weather.EnvClouds
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System.Runtime.InteropServices;

namespace Ktisis.Structs.Env.Weather;

[StructLayout(LayoutKind.Explicit, Size = 40)]
public struct EnvClouds {
	[FieldOffset(0)]
	public Vector3 CloudColor;
	[FieldOffset(12)]
	public Vector3 Color2;
	[FieldOffset(24)]
	public float Gradient;
	[FieldOffset(28)]
	public float SideHeight;
	[FieldOffset(32 /*0x20*/)]
	public uint CloudTexture;
	[FieldOffset(36)]
	public uint CloudSideTexture;
}
