// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Entities.SceneRoot
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

using Ktisis.Scene.Types;

namespace Ktisis.Scene.Entities;

public class SceneRoot(ISceneManager scene) : SceneEntity(scene) {
	public override bool IsValid => this.Scene.IsValid;

	public override SceneEntity? Parent {
		get => null;
		set => throw new Exception("Attempted to set parent of scene root.");
	}

	public override bool Add(SceneEntity entity) {
		Ktisis.Ktisis.Log.Debug($"Adding entity to scene: '{entity.Name}' ({entity.GetType().Name})", Array.Empty<object>());
		return base.Add(entity);
	}
}
