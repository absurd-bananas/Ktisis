// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Factory.Creators.ActorCreator
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Utility;
using Ktisis.Data.Files;
using Ktisis.Scene.Entities.Game;
using Ktisis.Scene.Factory.Types;
using Ktisis.Scene.Modules.Actors;
using Ktisis.Scene.Types;
using System.Threading.Tasks;

#nullable enable
namespace Ktisis.Scene.Factory.Creators;

public sealed class ActorCreator(ISceneManager scene) : 
  EntityCreator<ActorEntity, IActorCreator>(scene),
  IActorCreator,
  IEntityCreator<ActorEntity, IActorCreator>,
  IEntityBuilderBase<ActorEntity, IActorCreator>
{
  private CharaFile? Appearance;

  protected override IActorCreator Builder => (IActorCreator) this;

  public IActorCreator WithAppearance(CharaFile file)
  {
    this.Appearance = file;
    return (IActorCreator) this;
  }

  public async Task<ActorEntity> Spawn()
  {
    ActorCreator actorCreator = this;
    ActorEntity entity = await actorCreator.Scene.GetModule<ActorModule>().Spawn();
    ActorEntity actorEntity1 = entity;
    string str;
    if (!StringExtensions.IsNullOrEmpty(actorCreator.Name))
      str = actorCreator.Name;
    else
      str = $"Actor #{entity.Actor.ObjectIndex}";
    actorEntity1.Name = str;
    if (actorCreator.Appearance != null)
      await actorCreator.Scene.Context.Characters.ApplyCharaFile(entity, actorCreator.Appearance, gameState: true);
    ActorEntity actorEntity2 = entity;
    entity = (ActorEntity) null;
    return actorEntity2;
  }
}
