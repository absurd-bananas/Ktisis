// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Factory.Creators.LightCreator
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System.Threading.Tasks;

using Ktisis.Scene.Entities.World;
using Ktisis.Scene.Factory.Types;
using Ktisis.Scene.Modules.Lights;
using Ktisis.Scene.Types;
using Ktisis.Structs.Lights;

namespace Ktisis.Scene.Factory.Creators;

public sealed class LightCreator :
	EntityCreator<LightEntity, ILightCreator>,
	ILightCreator,
	IEntityCreator<LightEntity, ILightCreator>,
	IEntityBuilderBase<LightEntity, ILightCreator> {
	private LightType Type = LightType.SpotLight;

	public LightCreator(ISceneManager scene)
		: base(scene) {
		this.Name = "Light";
	}

	protected override ILightCreator Builder => this;

	public ILightCreator SetType(LightType type) {
		this.Type = type;
		return this;
	}

	public async Task<LightEntity> Spawn() {
		var lightCreator = this;
		var lightEntity = await lightCreator.Scene.GetModule<LightModule>().Spawn();
		lightEntity.Name = lightCreator.Name;
		lightEntity.SetType(lightCreator.Type);
		return lightEntity;
	}
}
