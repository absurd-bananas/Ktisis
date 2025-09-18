// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Camera.RenderCameraEx
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System.Runtime.InteropServices;

namespace Ktisis.Structs.Camera;

[StructLayout(LayoutKind.Explicit)]
public struct RenderCameraEx {
	[FieldOffset(0)]
	public FFXIVClientStructs.FFXIV.Client.Graphics.Render.Camera RenderCamera;
	[FieldOffset(492)]
	public float FoV;
	[FieldOffset(496)]
	public float AspectRatio;
	[FieldOffset(508)]
	public float OrthographicZoom;
	[FieldOffset(512 /*0x0200*/)]
	public bool OrthographicEnabled;
}
