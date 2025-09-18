// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Characters.WeaponContainer
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Ktisis.Structs.Characters;

[StructLayout(LayoutKind.Explicit, Size = 12)]
public struct WeaponContainer {
	public const int Length = 3;
	public const int Size = 12;
	[FieldOffset(0)]
	public unsafe fixed byte Bytes[12];
	[FieldOffset(0)]
	public WeaponModelId MainHand;
	[FieldOffset(4)]
	public WeaponModelId OffHand;
	[FieldOffset(8)]
	public WeaponModelId Prop;

	public WeaponModelId this[uint index] {
		get => this.Get(index);
		set => this.Set(index, value);
	}

	private unsafe WeaponModelId Get(uint index) => *this.GetData(index);

	private unsafe void Set(uint index, WeaponModelId equip) => *this.GetData(index) = equip;

	public unsafe WeaponModelId* GetData(uint index) {
		if (index >= 3U)
			throw new IndexOutOfRangeException($"Index {index} is out of range (< {3}).");
		fixed (byte* numPtr = this.Bytes)
			return (WeaponModelId*)(numPtr + index * sizeof(WeaponModelId));
	}
}
