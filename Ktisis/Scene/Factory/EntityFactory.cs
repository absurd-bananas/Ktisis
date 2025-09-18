// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Factory.EntityFactory
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game.ClientState.Objects.Types;
using Ktisis.Editor.Context.Types;
using Ktisis.Scene.Factory.Builders;
using Ktisis.Scene.Factory.Creators;
using Ktisis.Scene.Factory.Types;
using Ktisis.Scene.Types;
using Ktisis.Services.Data;
using Ktisis.Structs.Lights;

#nullable enable
namespace Ktisis.Scene.Factory;

public class EntityFactory : IEntityFactory
{
  private readonly IEditorContext _ctx;
  private readonly INameResolver _naming;

  private ISceneManager Scene => this._ctx.Scene;

  public EntityFactory(IEditorContext ctx, INameResolver naming)
  {
    this._ctx = ctx;
    this._naming = naming;
  }

  public IActorBuilder BuildActor(IGameObject actor)
  {
    return (IActorBuilder) new ActorBuilder(this.Scene, this.BuildPose(), actor);
  }

  public ILightBuilder BuildLight() => (ILightBuilder) new LightBuilder(this.Scene);

  public IObjectBuilder BuildObject()
  {
    return (IObjectBuilder) new ObjectBuilder(this.Scene, this.BuildPose(), this._naming);
  }

  public IPoseBuilder BuildPose() => (IPoseBuilder) new PoseBuilder(this.Scene);

  public IRefImageBuilder BuildRefImage() => (IRefImageBuilder) new RefImageBuilder(this.Scene);

  public IActorCreator CreateActor() => (IActorCreator) new ActorCreator(this.Scene);

  public ILightCreator CreateLight() => (ILightCreator) new LightCreator(this.Scene);

  public ILightCreator CreateLight(LightType type) => this.CreateLight().SetType(type);
}
