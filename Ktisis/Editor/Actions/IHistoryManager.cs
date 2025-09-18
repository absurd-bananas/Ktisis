// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Actions.IHistoryManager
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Actions.Types;
using System.Collections.Generic;

#nullable enable
namespace Ktisis.Editor.Actions;

public interface IHistoryManager
{
  int Count { get; }

  bool CanUndo { get; }

  bool CanRedo { get; }

  void Add(IMemento item);

  void Clear();

  IEnumerable<IMemento> GetTimeline();

  void Undo();

  void Redo();
}
