// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Characters.State.WeaponIndexEx
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System;

using Ktisis.GameData.Excel;

namespace Ktisis.Editor.Characters.State;

public static class WeaponIndexEx {
	public static EquipSlot ToEquipSlot(this WeaponIndex index) {
		if (index < WeaponIndex.Prop)
			return (EquipSlot)index;
		if (index == WeaponIndex.Prop)
			return EquipSlot.OffHand;
		throw new Exception($"Cannot convert invalid weapon index ({index})");
	}
}
