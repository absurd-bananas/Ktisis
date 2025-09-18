// Decompiled with JetBrains decompiler
// Type: Ktisis.Localization.LocaleMetaData
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
namespace Ktisis.Localization;

public class LocaleMetaData
{
  public string TechnicalName { get; }

  public string DisplayName { get; }

  public string SelfName { get; }

  public string?[] Maintainers { get; }

  internal LocaleMetaData(
    string technicalName,
    string displayName,
    string selfName,
    string?[] maintainers)
  {
    this.TechnicalName = technicalName;
    this.DisplayName = displayName;
    this.SelfName = selfName;
    this.Maintainers = maintainers;
  }
}
