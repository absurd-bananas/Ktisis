// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Factory.Builders.RefImageBuilder
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System.IO;

using Ktisis.Scene.Entities.Utility;
using Ktisis.Scene.Factory.Types;
using Ktisis.Scene.Types;

namespace Ktisis.Scene.Factory.Builders;

public sealed class RefImageBuilder(ISceneManager scene) :
	EntityBuilder<ReferenceImage, IRefImageBuilder>(scene),
	IRefImageBuilder,
	IEntityBuilder<ReferenceImage, IRefImageBuilder>,
	IEntityBuilderBase<ReferenceImage, IRefImageBuilder> {
	private ReferenceImage.SetupData Data = new ReferenceImage.SetupData();

	protected override IRefImageBuilder Builder => this;

	public IRefImageBuilder FromData(ReferenceImage.SetupData data) {
		this.Data = data;
		return this;
	}

	public IRefImageBuilder SetPath(string path) {
		this.Data.FilePath = path;
		return this;
	}

	protected override ReferenceImage Build() {
		if (StringExtensions.IsNullOrEmpty(this.Name))
			this.Name = Path.GetFileName(this.Data.FilePath);
		var referenceImage = new ReferenceImage(this.Scene, this.Data);
		referenceImage.Name = this.Name;
		return referenceImage;
	}
}
