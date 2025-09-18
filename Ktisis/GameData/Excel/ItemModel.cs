// Decompiled with JetBrains decompiler
// Type: Ktisis.GameData.Excel.ItemModel
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
namespace Ktisis.GameData.Excel;

public class ItemModel(ulong var, bool isWep = false)
{
  public ushort Id = (ushort) var;
  public ushort Base = isWep ? (ushort) (var >> 16 /*0x10*/) : (ushort) 0;
  public ushort Variant = isWep ? (ushort) (var >> 32 /*0x20*/) : (ushort) (var >> 16 /*0x10*/);

  public bool Matches(ushort id, ushort variant)
  {
    return (int) this.Id == (int) id && (int) this.Variant == (int) variant;
  }

  public bool Matches(ushort id, ushort secondId, ushort variant)
  {
    return (int) this.Id == (int) id && (int) this.Base == (int) secondId && (int) this.Variant == (int) variant;
  }
}
