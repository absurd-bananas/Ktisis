// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Transforms.Types.ITransformTarget
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System.Collections.Generic;

using Ktisis.Scene.Decor;
using Ktisis.Scene.Entities;

namespace Ktisis.Editor.Transforms.Types;

public interface ITransformTarget : ITransform {
	SceneEntity? Primary { get; }

	IEnumerable<SceneEntity> Targets { get; }

	TransformSetup Setup { get; set; }
}
