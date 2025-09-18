// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Characters.State.AppearanceState
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Structs.Characters;

#nullable enable
namespace Ktisis.Editor.Characters.State;

public class AppearanceState
{
  public readonly CustomizeState Customize = new CustomizeState();
  public readonly EquipmentState Equipment = new EquipmentState();
  public readonly WeaponState Weapons = new WeaponState();

  public uint? ModelId { get; set; }

  public EquipmentToggle HatVisible { get; set; }

  public bool CheckHatVisible(bool visible)
  {
    return this.HatVisible == EquipmentToggle.None ? visible : this.HatVisible == EquipmentToggle.On;
  }

  public EquipmentToggle VisorToggled { get; set; }

  public bool CheckVisorToggled(bool toggled)
  {
    return this.VisorToggled == EquipmentToggle.None ? toggled : this.VisorToggled == EquipmentToggle.On;
  }

  public WetnessState? Wetness { get; set; }
}
