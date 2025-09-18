// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Chara.Types.WeaponInfo
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.FFXIV.Client.Game.Character;
using Ktisis.Editor.Characters.State;
using Ktisis.Editor.Characters.Types;
using Ktisis.GameData.Excel;

#nullable enable
namespace Ktisis.Interface.Components.Chara.Types;

public class WeaponInfo(IEquipmentEditor editor) : ItemInfo
{
  public required WeaponIndex Index;
  public required WeaponModelId Model;

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

  public void SetModel(ushort id, ushort second, byte variant)
  {
    editor.SetWeaponIdBaseVariant(this.Index, id, second, variant);
  }

  public override void SetEquipItem(ItemSheet item)
  {
    int num = this.Index == WeaponIndex.MainHand ? 1 : 0;
    ItemModel itemModel = num != 0 && item.Model.Id != (ushort) 0 || item.SubModel.Id == (ushort) 0 ? item.Model : item.SubModel;
    this.SetModel(itemModel.Id, itemModel.Base, (byte) itemModel.Variant);
    if (num == 0 || item.SubModel.Id == (ushort) 0)
      return;
    editor.SetWeaponIdBaseVariant(WeaponIndex.OffHand, item.SubModel.Id, item.SubModel.Base, (byte) item.SubModel.Variant);
  }

  public override void SetStainId(byte id, int index = 0)
  {
    editor.SetWeaponStainId(this.Index, id, index);
  }

  public override void Unequip() => this.SetModel((ushort) 0, (ushort) 0, (byte) 0);

  public override bool IsHideable => true;

  public override bool IsVisible => editor.GetWeaponVisible(this.Index);

  public override void SetVisible(bool visible) => editor.SetWeaponVisible(this.Index, visible);

  public override bool IsCurrent() => editor.GetWeaponIndex(this.Index).Equals((object) this.Model);

  public override bool IsItemPredicate(ItemSheet item)
  {
    if (item.Model.Matches(this.Model.Id, this.Model.Type, this.Model.Variant))
      return true;
    return item.SubModel.Id != (ushort) 0 && item.SubModel.Matches(this.Model.Id, this.Model.Type, this.Model.Variant);
  }
}
