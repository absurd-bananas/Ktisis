// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Factory.Builders.IBoneTreeBuilder
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Scene.Entities.Skeleton;

namespace Ktisis.Scene.Factory.Builders;

public interface IBoneTreeBuilder {
	IBoneTreeBuilder BuildBoneList();

	IBoneTreeBuilder BuildCategoryMap();

	void BindTo(EntityPose pose);
}
