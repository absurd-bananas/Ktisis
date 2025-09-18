// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Characters.State.AppearanceState
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Structs.Characters;

namespace Ktisis.Editor.Characters.State;

public class AppearanceState {
	public readonly CustomizeState Customize = new CustomizeState();
	public readonly EquipmentState Equipment = new EquipmentState();
	public readonly WeaponState Weapons = new WeaponState();

	public uint? ModelId { get; set; }

	public EquipmentToggle HatVisible { get; set; }

	public EquipmentToggle VisorToggled { get; set; }

	public WetnessState? Wetness { get; set; }

	public bool CheckHatVisible(bool visible) => this.HatVisible == EquipmentToggle.None ? visible : this.HatVisible == EquipmentToggle.On;

	public bool CheckVisorToggled(bool toggled) => this.VisorToggled == EquipmentToggle.None ? toggled : this.VisorToggled == EquipmentToggle.On;
}
