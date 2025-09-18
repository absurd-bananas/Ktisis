// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Factory.Types.IEntityBuilderBase`2
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Scene.Entities;

namespace Ktisis.Scene.Factory.Types;

public interface IEntityBuilderBase<out T, out TBuilder>
	where T : SceneEntity
	where TBuilder : IEntityBuilderBase<T, TBuilder> {
	TBuilder SetName(string name);
}
