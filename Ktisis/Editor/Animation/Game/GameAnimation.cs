// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Animation.Game.GameAnimation
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Editor.Animation.Types;

namespace Ktisis.Editor.Animation.Game;

public abstract class GameAnimation {
	public abstract string Name { get; }

	public abstract ushort Icon { get; }

	public abstract uint TimelineId { get; }

	public abstract TimelineSlot Slot { get; }
}
