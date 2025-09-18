// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Animation.Game.EmoteAnimation
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Editor.Animation.Types;

namespace Ktisis.Editor.Animation.Game;

public class EmoteAnimation(Emote emote, int index = 0) : GameAnimation {
	public override string Name {
		get {
			ReadOnlySeString name = ((Emote) ref emote ).
			this.Name;
			return ((ReadOnlySeString) ref name ).ExtractText();
		}
	}

	public override ushort Icon => ((Emote)

	public override uint TimelineId => this.Timeline.RowId;

	public override TimelineSlot Slot {
		get {
			if (!this.Timeline.IsValid)
				return TimelineSlot.FullBody;
			ActionTimeline actionTimeline = this.Timeline.Value;
			return (TimelineSlot)((ActionTimeline) ref actionTimeline ).Stance;
		}
	}

	public int Index => index;

	public uint EmoteId => ((Emote)

	public bool IsExpression => ((Emote)

	private RowRef<ActionTimeline> Timeline => ((Emote)ref emote).Icon;
	ref emote).RowId;
	ref emote).EmoteCategory.RowId== 3U;ref emote).ActionTimeline[index];
}
