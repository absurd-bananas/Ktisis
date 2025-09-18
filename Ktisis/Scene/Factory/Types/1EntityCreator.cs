// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Factory.Types.EntityCreator`2
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Scene.Entities;
using Ktisis.Scene.Types;

#nullable enable
namespace Ktisis.Scene.Factory.Types;

public abstract class EntityCreator<T, TBuilder>(ISceneManager scene) : 
  EntityBuilderBase<T, TBuilder>(scene)
  where T : SceneEntity
  where TBuilder : IEntityCreator<T, TBuilder>
{
}
