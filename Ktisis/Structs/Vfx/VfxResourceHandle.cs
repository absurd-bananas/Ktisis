// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Vfx.VfxResourceHandle
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System.Runtime.InteropServices;

namespace Ktisis.Structs.Vfx;

[StructLayout(LayoutKind.Explicit, Size = 8)]
public struct VfxResourceHandle {
	[FieldOffset(0)]
	public ulong Value;
	[FieldOffset(0)]
	public uint Id;
	[FieldOffset(4)]
	public uint Index;
}
