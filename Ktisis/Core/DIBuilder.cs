// Decompiled with JetBrains decompiler
// Type: Ktisis.Core.DIBuilder
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

using Ktisis.Core.Attributes;

using Microsoft.Extensions.DependencyInjection;

namespace Ktisis.Core;

[Singleton]
public class DIBuilder {
	private readonly IServiceProvider _services;

	public DIBuilder(IServiceProvider _services) {
		this._services = _services;
	}

	public object Create(Type type, params object[] parameters) => ActivatorUtilities.CreateInstance(this._services, type, parameters);

	public T Create<T>(params object[] parameters) => ActivatorUtilities.CreateInstance<T>(this._services, parameters);
}
