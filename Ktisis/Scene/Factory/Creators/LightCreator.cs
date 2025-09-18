// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Factory.Creators.LightCreator
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Scene.Entities.World;
using Ktisis.Scene.Factory.Types;
using Ktisis.Scene.Modules.Lights;
using Ktisis.Scene.Types;
using Ktisis.Structs.Lights;
using System.Threading.Tasks;

#nullable enable
namespace Ktisis.Scene.Factory.Creators;

public sealed class LightCreator : 
  EntityCreator<LightEntity, ILightCreator>,
  ILightCreator,
  IEntityCreator<LightEntity, ILightCreator>,
  IEntityBuilderBase<LightEntity, ILightCreator>
{
  private LightType Type = LightType.SpotLight;

  public LightCreator(ISceneManager scene)
    : base(scene)
  {
    this.Name = "Light";
  }

  protected override ILightCreator Builder => (ILightCreator) this;

  public ILightCreator SetType(LightType type)
  {
    this.Type = type;
    return (ILightCreator) this;
  }

  public async Task<LightEntity> Spawn()
  {
    LightCreator lightCreator = this;
    LightEntity lightEntity = await lightCreator.Scene.GetModule<LightModule>().Spawn();
    lightEntity.Name = lightCreator.Name;
    lightEntity.SetType(lightCreator.Type);
    return lightEntity;
  }
}
