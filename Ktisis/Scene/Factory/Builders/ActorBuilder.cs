// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Factory.Builders.ActorBuilder
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game.ClientState.Objects.Types;
using Ktisis.Common.Extensions;
using Ktisis.Scene.Entities.Game;
using Ktisis.Scene.Factory.Types;
using Ktisis.Scene.Types;
using System.Runtime.CompilerServices;

#nullable enable
namespace Ktisis.Scene.Factory.Builders;

public sealed class ActorBuilder : 
  EntityBuilder<ActorEntity, IActorBuilder>,
  IActorBuilder,
  IEntityBuilder<ActorEntity, IActorBuilder>,
  IEntityBuilderBase<ActorEntity, IActorBuilder>
{
  private readonly IPoseBuilder _pose;
  private readonly IGameObject _gameObject;

  public ActorBuilder(ISceneManager scene, IPoseBuilder pose, IGameObject gameObject)
    : base(scene)
  {
    this.Name = gameObject.GetNameOrFallback();
    this._pose = pose;
    this._gameObject = gameObject;
  }

  virtual ActorBuilder EntityBuilderBase<ActorEntity, IActorBuilder>.Builder
  {
    [PreserveBaseOverrides] get => this;
  }

  protected override ActorEntity Build()
  {
    ActorEntity actorEntity = new ActorEntity(this.Scene, this._pose, this._gameObject);
    actorEntity.Name = this.Name;
    return actorEntity;
  }
}
