// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Factory.Types.EntityBuilder`2
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

using Ktisis.Scene.Entities;
using Ktisis.Scene.Entities.World;
using Ktisis.Scene.Types;

namespace Ktisis.Scene.Factory.Types;

public abstract class EntityBuilder<T, TBuilder>(ISceneManager scene) :
	EntityBuilderBase<T, TBuilder>(scene)
	where T : SceneEntity
	where TBuilder : IEntityBuilder<T, TBuilder> {
	protected abstract T Build();

	public T Add() => this.Add(this.Scene);

	public virtual T Add(IComposite parent) {
		if (!this.Scene.IsValid)
			throw new Exception("Attempted to build entity for invalid scene.");
		var result = this.GetResult();
		parent.Add(result);
		if (result is WorldEntity worldEntity)
			worldEntity.Setup();
		return result;
	}

	private T GetResult() {
		var result = this.Build();
		if (StringExtensions.IsNullOrEmpty(result.Name))
			result.Name = result.GetType().Name;
		return result;
	}
}
