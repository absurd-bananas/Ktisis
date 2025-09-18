// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Animation.Game.TimelineAnimation
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Editor.Animation.Types;

namespace Ktisis.Editor.Animation.Game;

public class TimelineAnimation(ActionTimeline timeline) : GameAnimation {
	public override string Name {
		get {
			ReadOnlySeString key = ((ActionTimeline) ref timeline ).Key;
			return ((ReadOnlySeString) ref key ).ExtractText();
		}
	}

	public override ushort Icon => 0;

	public override uint TimelineId => ((ActionTimeline)

	public override TimelineSlot Slot => (TimelineSlot)((ActionTimeline)ref timeline).RowId;
	ref timeline).Stance;
}
