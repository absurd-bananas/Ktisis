// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Characters.State.EquipmentState
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Structs.Characters;

namespace Ktisis.Editor.Characters.State;

public class EquipmentState {
	private readonly bool[] _state = new bool[10];
	private EquipmentContainer _container;

	public EquipmentModelId this[EquipIndex index] {
		get => this._container[(uint)index];
		set {
			this._container[(uint)index] = value;
			this._state[(int)index] = true;
		}
	}

	public bool IsSet(EquipIndex index) => this._state[(int)index];

	public void Unset(EquipIndex index) => this._state[(int)index] = false;
}
