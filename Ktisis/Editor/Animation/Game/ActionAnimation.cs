// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Animation.Game.ActionAnimation
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Editor.Animation.Types;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;

#nullable enable
namespace Ktisis.Editor.Animation.Game;

public class ActionAnimation(Action action) : GameAnimation
{
  public override string Name
  {
    get
    {
      ReadOnlySeString name = ((Action) ref action).Name;
      return ((ReadOnlySeString) ref name).ExtractText();
    }
  }

  public override ushort Icon => ((Action) ref action).Icon;

  public override uint TimelineId
  {
    get
    {
      return !((Action) ref action).AnimationEnd.IsValid ? 0U : ((Action) ref action).AnimationEnd.RowId;
    }
  }

  public override TimelineSlot Slot
  {
    get
    {
      if (!((Action) ref action).AnimationEnd.IsValid)
        return TimelineSlot.FullBody;
      ActionTimeline actionTimeline = ((Action) ref action).AnimationEnd.Value;
      return (TimelineSlot) ((ActionTimeline) ref actionTimeline).Stance;
    }
  }
}
