// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Chara.Types.EquipInfo
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.FFXIV.Client.Game.Character;
using Ktisis.Editor.Characters.State;
using Ktisis.Editor.Characters.Types;
using Ktisis.GameData.Excel;

#nullable enable
namespace Ktisis.Interface.Components.Chara.Types;

public class EquipInfo(IEquipmentEditor editor) : ItemInfo
{
  public required EquipIndex Index;
  public required EquipmentModelId Model;

  public override EquipSlot Slot => this.Index.ToEquipSlot();

  public override ushort ModelId => this.Model.Id;

  public override byte[] StainIds
  {
    get
    {
      return new byte[2]
      {
        this.Model.Stain0,
        this.Model.Stain1
      };
    }
  }

  public void SetModel(ushort id, byte variant)
  {
    editor.SetEquipIdVariant(this.Index, id, variant);
  }

  public override void SetEquipItem(ItemSheet item)
  {
    this.SetModel(item.Model.Id, (byte) item.Model.Variant);
  }

  public override void SetStainId(byte id, int index = 0)
  {
    editor.SetEquipStainId(this.Index, id, index);
  }

  public override void Unequip() => this.SetModel((ushort) 0, (byte) 0);

  public override bool IsHideable => this.Slot == EquipSlot.Head;

  public override bool IsVisible => this.Slot == EquipSlot.Head && editor.GetHatVisible();

  public override void SetVisible(bool visible)
  {
    if (this.Slot != EquipSlot.Head)
      return;
    editor.SetHatVisible(visible);
  }

  public override bool IsVisor => this.Slot == EquipSlot.Head;

  public override bool IsVisorToggled => this.Slot == EquipSlot.Head && editor.GetVisorToggled();

  public override void SetVisorToggled(bool toggled)
  {
    if (this.Slot != EquipSlot.Head)
      return;
    editor.SetVisorToggled(toggled);
  }

  public override bool IsCurrent() => editor.GetEquipIndex(this.Index).Equals((object) this.Model);

  public override bool IsItemPredicate(ItemSheet item)
  {
    return item.Model.Matches(this.Model.Id, (ushort) this.Model.Variant);
  }
}
