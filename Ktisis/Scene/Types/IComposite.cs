// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Types.IComposite
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System.Collections.Generic;

using Ktisis.Scene.Entities;

namespace Ktisis.Scene.Types;

public interface IComposite {
	SceneEntity? Parent { get; set; }

	IEnumerable<SceneEntity> Children { get; }

	bool Add(SceneEntity entity);

	bool Remove(SceneEntity entity);

	IEnumerable<SceneEntity> Recurse();
}
