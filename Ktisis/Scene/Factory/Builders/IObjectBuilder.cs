// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Factory.Builders.IObjectBuilder
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

using Ktisis.Scene.Entities.World;
using Ktisis.Scene.Factory.Types;

namespace Ktisis.Scene.Factory.Builders;

public interface IObjectBuilder :
	IEntityBuilder<WorldEntity, IObjectBuilder>,
	IEntityBuilderBase<WorldEntity, IObjectBuilder> {
	IObjectBuilder SetAddress(IntPtr address);

	unsafe IObjectBuilder SetAddress(object* pointer);
}
