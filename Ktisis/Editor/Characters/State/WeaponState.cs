// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Characters.State.WeaponState
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Structs.Characters;

namespace Ktisis.Editor.Characters.State;

public class WeaponState {
	private readonly bool[] _state = new bool[3];
	private readonly EquipmentToggle[] _visible = new EquipmentToggle[3];
	private WeaponContainer _container;

	public WeaponModelId this[WeaponIndex index] {
		get => this._container[(uint)index];
		set {
			this._container[(uint)index] = value;
			this._state[(int)index] = true;
		}
	}

	public bool IsSet(WeaponIndex index) => this._state[(int)index];

	public void Unset(WeaponIndex index) => this._state[(int)index] = false;

	public EquipmentToggle GetVisible(WeaponIndex index) => this._visible[(int)index];

	public void SetVisible(WeaponIndex index, bool visible) {
		this._visible[(int)index] = visible ? EquipmentToggle.On : EquipmentToggle.Off;
	}

	public bool CheckVisible(WeaponIndex index, bool visible) {
		var equipmentToggle = this._visible[(int)index];
		return equipmentToggle == EquipmentToggle.None ? visible : equipmentToggle == EquipmentToggle.On;
	}
}
