// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Factory.Types.IEntityBuilder`2
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Scene.Entities;
using Ktisis.Scene.Types;

namespace Ktisis.Scene.Factory.Types;

public interface IEntityBuilder<out T, out TBuilder> : IEntityBuilderBase<T, TBuilder>
	where T : SceneEntity
	where TBuilder : IEntityBuilder<T, TBuilder> {
	T Add();

	T Add(IComposite parent);
}
