// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Animation.Game.GameAnimationData
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game;
using Dalamud.Plugin.Services;
using Ktisis.Editor.Animation.Types;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#nullable enable
namespace Ktisis.Editor.Animation.Game;

public class GameAnimationData(IDataManager data)
{
  private readonly List<GameAnimation> Animations = new List<GameAnimation>();
  private ExcelSheet<ActionTimeline>? Timelines;

  public int Count
  {
    get
    {
      lock (this.Animations)
        return this.Animations.Count;
    }
  }

  public IEnumerable<GameAnimation> GetAll()
  {
    lock (this.Animations)
      return (IEnumerable<GameAnimation>) this.Animations.AsReadOnly();
  }

  public async Task Build()
  {
    await Task.Yield();
    this.FetchEmotes();
    this.FetchActions();
    this.FetchTimelines();
  }

  public ActionTimeline? GetTimelineById(uint id) => this.Timelines?.GetRow(id);

  private void FetchEmotes()
  {
    IEnumerable<EmoteAnimation> collection = Enumerable.DistinctBy<EmoteAnimation, (string, TimelineSlot)>(((IEnumerable<Emote>) data.GetExcelSheet<Emote>(new ClientLanguage?(), (string) null)).Where<Emote>((Func<Emote, bool>) (emote =>
    {
      ReadOnlySeString name = ((Emote) ref emote).Name;
      return !((ReadOnlySeString) ref name).IsEmpty;
    })).SelectMany<Emote, EmoteAnimation>(new Func<Emote, IEnumerable<EmoteAnimation>>(MapEmotes)), (Func<EmoteAnimation, (string, TimelineSlot)>) (emote => (emote.Name, emote.Slot)));
    lock (this.Animations)
      this.Animations.AddRange((IEnumerable<GameAnimation>) collection);

    static IEnumerable<EmoteAnimation> MapEmotes(Emote emote)
    {
      int i = 0;
      while (true)
      {
        int num = i;
        Collection<RowRef<ActionTimeline>> actionTimeline = ((Emote) ref emote).ActionTimeline;
        int count = actionTimeline.Count;
        if (num < count)
        {
          actionTimeline = ((Emote) ref emote).ActionTimeline;
          RowRef<ActionTimeline> rowRef = actionTimeline[i];
          if (rowRef.IsValid && rowRef.RowId != 0U)
            yield return new EmoteAnimation(emote, i);
          ++i;
        }
        else
          break;
      }
    }
  }

  private void FetchActions()
  {
    IEnumerable<ActionAnimation> collection = Enumerable.DistinctBy<Action, (string, ushort, uint)>(((IEnumerable<Action>) data.GetExcelSheet<Action>(new ClientLanguage?(), (string) null)).Where<Action>((Func<Action, bool>) (action =>
    {
      ReadOnlySeString name = ((Action) ref action).Name;
      return !((ReadOnlySeString) ref name).IsEmpty;
    })), (Func<Action, (string, ushort, uint)>) (action =>
    {
      ReadOnlySeString name = ((Action) ref action).Name;
      return (((ReadOnlySeString) ref name).ExtractText(), ((Action) ref action).Icon, ((Action) ref action).AnimationStart.RowId);
    })).Select<Action, ActionAnimation>((Func<Action, ActionAnimation>) (action => new ActionAnimation(action)));
    lock (this.Animations)
      this.Animations.AddRange((IEnumerable<GameAnimation>) collection);
  }

  private void FetchTimelines()
  {
    if (this.Timelines == null)
      this.Timelines = data.GetExcelSheet<ActionTimeline>(new ClientLanguage?(), (string) null);
    IEnumerable<TimelineAnimation> collection = ((IEnumerable<ActionTimeline>) this.Timelines).Where<ActionTimeline>((Func<ActionTimeline, bool>) (timeline =>
    {
      ReadOnlySeString key = ((ActionTimeline) ref timeline).Key;
      return !((ReadOnlySeString) ref key).IsEmpty;
    })).Select<ActionTimeline, TimelineAnimation>((Func<ActionTimeline, TimelineAnimation>) (timeline => new TimelineAnimation(timeline)));
    lock (this.Animations)
      this.Animations.AddRange((IEnumerable<GameAnimation>) collection);
  }
}
