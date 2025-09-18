// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Posing.Data.MultipleMemento
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System.Collections.Generic;

using Ktisis.Actions.Types;

namespace Ktisis.Editor.Posing.Data;

public class MultipleMemento(IReadOnlyList<IMemento?> mementos) : IMemento {
	public IReadOnlyList<IMemento?> Mementos => mementos;

	public void Restore() {
		for (var index = mementos.Count - 1; index >= 0; --index)
			mementos[index]?.Restore();
	}

	public void Apply() {
		for (var index = 0; index < mementos.Count; ++index)
			mementos[index]?.Apply();
	}
}
