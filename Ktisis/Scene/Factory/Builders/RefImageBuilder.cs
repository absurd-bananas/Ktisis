// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Factory.Builders.RefImageBuilder
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Utility;
using Ktisis.Scene.Entities.Utility;
using Ktisis.Scene.Factory.Types;
using Ktisis.Scene.Types;
using System.IO;

#nullable enable
namespace Ktisis.Scene.Factory.Builders;

public sealed class RefImageBuilder(ISceneManager scene) : 
  EntityBuilder<ReferenceImage, IRefImageBuilder>(scene),
  IRefImageBuilder,
  IEntityBuilder<ReferenceImage, IRefImageBuilder>,
  IEntityBuilderBase<ReferenceImage, IRefImageBuilder>
{
  private ReferenceImage.SetupData Data = new ReferenceImage.SetupData();

  protected override IRefImageBuilder Builder => (IRefImageBuilder) this;

  public IRefImageBuilder FromData(ReferenceImage.SetupData data)
  {
    this.Data = data;
    return (IRefImageBuilder) this;
  }

  public IRefImageBuilder SetPath(string path)
  {
    this.Data.FilePath = path;
    return (IRefImageBuilder) this;
  }

  protected override ReferenceImage Build()
  {
    if (StringExtensions.IsNullOrEmpty(this.Name))
      this.Name = Path.GetFileName(this.Data.FilePath);
    ReferenceImage referenceImage = new ReferenceImage(this.Scene, this.Data);
    referenceImage.Name = this.Name;
    return referenceImage;
  }
}
