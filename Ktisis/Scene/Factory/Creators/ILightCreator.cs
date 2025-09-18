// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Factory.Creators.ILightCreator
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Scene.Entities.World;
using Ktisis.Scene.Factory.Types;
using Ktisis.Structs.Lights;

namespace Ktisis.Scene.Factory.Creators;

public interface ILightCreator :
	IEntityCreator<LightEntity, ILightCreator>,
	IEntityBuilderBase<LightEntity, ILightCreator> {
	ILightCreator SetType(LightType type);
}
