// Decompiled with JetBrains decompiler
// Type: Ktisis.GameData.Excel.Glasses
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Common.Extensions;
using Lumina.Excel;

#nullable enable
namespace Ktisis.GameData.Excel;

[Sheet("Glasses", 799720129)]
public struct Glasses(uint row) : IExcelRow<Glasses>
{
  public uint RowId { get; } = row;

  public string Name { get; set; } = string.Empty;

  public uint Icon { get; set; } = 0;

  static Glasses IExcelRow<Glasses>.Create(ExcelPage page, uint offset, uint row)
  {
    return new Glasses(row)
    {
      Name = page.ReadColumn<string>(13, offset),
      Icon = (uint) page.ReadColumn<int>(2, offset)
    };
  }
}
