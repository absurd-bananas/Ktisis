// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Characters.State.EquipIndexEx
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using Ktisis.GameData.Excel;

namespace Ktisis.Editor.Characters.State;

public static class EquipIndexEx {
	public static EquipSlot ToEquipSlot(this EquipIndex index) {
		var num = (int)index;
		EquipSlot equipSlot;
		switch (index) {
			case EquipIndex.RingRight:
				equipSlot = EquipSlot.RingRight;
				break;
			case EquipIndex.RingLeft:
				equipSlot = EquipSlot.RingLeft;
				break;
			default:
				equipSlot = (EquipSlot)(num + (num > 2 ? 3 : 2));
				break;
		}
		return equipSlot;
	}

	public static EquipIndex ToEquipIndex(this EquipSlot slot) {
		var num = (int)slot;
		EquipIndex equipIndex;
		switch (slot) {
			case EquipSlot.RingLeft:
				equipIndex = EquipIndex.RingLeft;
				break;
			case EquipSlot.RingRight:
				equipIndex = EquipIndex.RingRight;
				break;
			default:
				equipIndex = (EquipIndex)(num - (num >= 5 ? 3 : 2));
				break;
		}
		return equipIndex;
	}
}
