// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Actions.HistoryManager
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Actions.Types;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Ktisis.Editor.Actions;

public class HistoryManager : IHistoryManager
{
  private const int TimelineMax = 100;
  private readonly List<IMemento> Timeline = new List<IMemento>();
  private int Cursor;

  public int Count => this.Timeline.Count;

  public void Add(IMemento item)
  {
    int num = this.Timeline.Count<IMemento>();
    if (this.Cursor < num)
    {
      Ktisis.Ktisis.Log.Verbose($"If history must be unwritten, let it be unwritten. ({this.Cursor} <- {num})", Array.Empty<object>());
      this.Timeline.RemoveRange(this.Cursor, num - this.Cursor);
    }
    this.Timeline.Add(item);
    ++this.Cursor;
  }

  public void Clear()
  {
    this.Timeline.Clear();
    this.Cursor = 0;
  }

  public IEnumerable<IMemento> GetTimeline() => (IEnumerable<IMemento>) this.Timeline;

  public bool CanUndo => this.Cursor > 0;

  public bool CanRedo => this.Cursor < this.Timeline.Count;

  public void Undo()
  {
    if (!this.CanUndo)
      return;
    Ktisis.Ktisis.Log.Info("Undoing", Array.Empty<object>());
    --this.Cursor;
    this.Timeline[this.Cursor].Restore();
  }

  public void Redo()
  {
    if (!this.CanRedo)
      return;
    Ktisis.Ktisis.Log.Info("Redoing", Array.Empty<object>());
    this.Timeline[this.Cursor].Apply();
    ++this.Cursor;
  }
}
