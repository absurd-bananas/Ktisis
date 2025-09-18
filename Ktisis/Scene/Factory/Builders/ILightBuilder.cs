// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Factory.Builders.ILightBuilder
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Scene.Entities.World;
using Ktisis.Scene.Factory.Types;
using Ktisis.Structs.Lights;
using System;

#nullable enable
namespace Ktisis.Scene.Factory.Builders;

public interface ILightBuilder : 
  IEntityBuilder<LightEntity, ILightBuilder>,
  IEntityBuilderBase<LightEntity, ILightBuilder>
{
  ILightBuilder SetAddress(IntPtr address);

  unsafe ILightBuilder SetAddress(SceneLight* pointer);
}
