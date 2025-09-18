// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Attachment.ElementParam
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System.Runtime.InteropServices;

namespace Ktisis.Structs.Attachment;

[StructLayout(LayoutKind.Explicit, Size = 64 /*0x40*/)]
public struct ElementParam {
	[FieldOffset(0)]
	public unsafe fixed char NameBytes[28];
	[FieldOffset(32 /*0x20*/)]
	public ElementId ElementId;
	[FieldOffset(36)]
	public Vector3 Position;
	[FieldOffset(48 /*0x30*/)]
	public Vector3 Rotation;
}
