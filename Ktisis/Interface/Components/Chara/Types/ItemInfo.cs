// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Chara.Types.ItemInfo
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Interface.Textures;
using Ktisis.GameData.Excel;

#nullable enable
namespace Ktisis.Interface.Components.Chara.Types;

public abstract class ItemInfo
{
  public bool FlagUpdate { get; set; }

  public ItemSheet? Item { get; set; }

  public ISharedImmediateTexture? Texture { get; set; }

  public abstract EquipSlot Slot { get; }

  public abstract ushort ModelId { get; }

  public abstract byte[] StainIds { get; }

  public abstract void SetEquipItem(ItemSheet item);

  public abstract void SetStainId(byte id, int index = 0);

  public abstract void Unequip();

  public abstract bool IsHideable { get; }

  public abstract bool IsVisible { get; }

  public abstract void SetVisible(bool visible);

  public virtual bool IsVisor => false;

  public virtual bool IsVisorToggled => false;

  public virtual void SetVisorToggled(bool toggle)
  {
  }

  public abstract bool IsCurrent();

  public abstract bool IsItemPredicate(ItemSheet item);
}
