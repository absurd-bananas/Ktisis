// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Factory.Builders.IRefImageBuilder
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Scene.Entities.Utility;
using Ktisis.Scene.Factory.Types;

#nullable enable
namespace Ktisis.Scene.Factory.Builders;

public interface IRefImageBuilder : 
  IEntityBuilder<ReferenceImage, IRefImageBuilder>,
  IEntityBuilderBase<ReferenceImage, IRefImageBuilder>
{
  IRefImageBuilder FromData(ReferenceImage.SetupData data);

  IRefImageBuilder SetPath(string path);
}
