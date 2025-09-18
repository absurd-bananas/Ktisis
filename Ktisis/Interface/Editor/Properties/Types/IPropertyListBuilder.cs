// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Editor.Properties.Types.IPropertyListBuilder
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;

#nullable enable
namespace Ktisis.Interface.Editor.Properties.Types;

public interface IPropertyListBuilder
{
  void AddHeader(string name, Action callback, int priority = -1);
}
