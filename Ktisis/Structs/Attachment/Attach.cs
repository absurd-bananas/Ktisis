// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Attachment.Attach
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Ktisis.Structs.Attachment;

[StructLayout(LayoutKind.Explicit)]
public struct Attach {
	[FieldOffset(80 /*0x50*/)]
	public AttachType Type;
	[FieldOffset(84)]
	public uint Capacity;
	[FieldOffset(88)]
	public unsafe Skeleton* Child;
	[FieldOffset(96 /*0x60*/)]
	public unsafe void* Parent;
	[FieldOffset(104)]
	public uint Count;
	[FieldOffset(112 /*0x70*/)]
	public unsafe AttachParam* Param;

	public bool IsActive() => this.IsValid() && this.Type != AttachType.None && this.Count > 0U;

	public unsafe bool IsValid() => (IntPtr)this.Param != IntPtr.Zero && (IntPtr)this.Child != IntPtr.Zero && (IntPtr)this.Parent != IntPtr.Zero;

	public unsafe Skeleton* GetParentSkeleton() {
		Skeleton* parentSkeleton;
		switch (this.Type) {
			case AttachType.ElementId:
				parentSkeleton = ((CharacterBase*)this.Parent)->Skeleton;
				break;
			case AttachType.BoneIndex:
				parentSkeleton = (Skeleton*)this.Parent;
				break;
			default:
				parentSkeleton = (Skeleton*)null;
				break;
		}
		return parentSkeleton;
	}
}
