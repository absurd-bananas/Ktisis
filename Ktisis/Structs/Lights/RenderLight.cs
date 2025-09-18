// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Lights.RenderLight
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System.Runtime.InteropServices;

using Ktisis.Structs.Common;

namespace Ktisis.Structs.Lights;

[StructLayout(LayoutKind.Explicit, Size = 160 /*0xA0*/)]
public struct RenderLight {
	[FieldOffset(24)]
	public LightFlags Flags;
	[FieldOffset(28)]
	public LightType LightType;
	[FieldOffset(32 /*0x20*/)]
	public unsafe FFXIVClientStructs.FFXIV.Client.Graphics.Transform* Transform;
	[FieldOffset(40)]
	public ColorHDR Color;
	[FieldOffset(56)]
	public Vector3 _unkVec0;
	[FieldOffset(68)]
	public Vector3 _unkVec1;
	[FieldOffset(80 /*0x50*/)]
	public Vector4 _unkVec2;
	[FieldOffset(96 /*0x60*/)]
	public float ShadowNear;
	[FieldOffset(100)]
	public float ShadowFar;
	[FieldOffset(104)]
	public FalloffType FalloffType;
	[FieldOffset(112 /*0x70*/)]
	public Vector2 AreaAngle;
	[FieldOffset(120)]
	public float _unk0;
	[FieldOffset(128 /*0x80*/)]
	public float Falloff;
	[FieldOffset(132)]
	public float LightAngle;
	[FieldOffset(136)]
	public float FalloffAngle;
	[FieldOffset(140)]
	public float Range;
	[FieldOffset(144 /*0x90*/)]
	public float CharaShadowRange;
}
