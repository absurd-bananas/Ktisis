// Decompiled with JetBrains decompiler
// Type: Ktisis.Core.Attributes.VersionAttribute
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;

#nullable enable
namespace Ktisis.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class VersionAttribute : Attribute
{
  public readonly string Target;

  public VersionAttribute(string target) => this.Target = target;

  public bool IsValidated(out string target, out string current)
  {
    target = this.Target;
    current = GameVersion.GetCurrent();
    return target == current;
  }
}
