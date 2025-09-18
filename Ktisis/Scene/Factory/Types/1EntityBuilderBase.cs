// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Factory.Types.EntityBuilderBase`2
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Scene.Entities;
using Ktisis.Scene.Types;

#nullable enable
namespace Ktisis.Scene.Factory.Types;

public abstract class EntityBuilderBase<T, TBuilder> : IEntityBuilderBase<T, TBuilder>
  where T : SceneEntity
  where TBuilder : IEntityBuilderBase<T, TBuilder>
{
  protected readonly ISceneManager Scene;

  protected string Name { get; set; } = string.Empty;

  protected EntityBuilderBase(ISceneManager scene) => this.Scene = scene;

  protected abstract TBuilder Builder { get; }

  public virtual TBuilder SetName(string name)
  {
    this.Name = name;
    return this.Builder;
  }
}
