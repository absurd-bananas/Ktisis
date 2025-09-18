// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Factory.Types.IEntityFactory
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Scene.Factory.Builders;
using Ktisis.Scene.Factory.Creators;
using Ktisis.Structs.Lights;

namespace Ktisis.Scene.Factory.Types;

public interface IEntityFactory {
	IActorBuilder BuildActor(IGameObject actor);

	ILightBuilder BuildLight();

	IObjectBuilder BuildObject();

	IPoseBuilder BuildPose();

	IActorCreator CreateActor();

	ILightCreator CreateLight();

	ILightCreator CreateLight(LightType type);

	IRefImageBuilder BuildRefImage();
}
