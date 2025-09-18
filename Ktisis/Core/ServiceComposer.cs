// Decompiled with JetBrains decompiler
// Type: Ktisis.Core.ServiceComposer
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Linq;
using System.Reflection;

using Ktisis.Core.Attributes;
using Ktisis.Services.Plugin;

using Microsoft.Extensions.DependencyInjection;

namespace Ktisis.Core;

public sealed class ServiceComposer {
	private readonly ServiceCollection _services = new ServiceCollection();

	public ServiceComposer AddSingleton<T>(T inst) where T : class {
		this._services.AddSingleton(inst);
		return this;
	}

	public ServiceComposer AddDalamudServices(IDalamudPluginInterface dpi) {
		dpi.Create<DalamudServices>(Array.Empty<object>()).Add(dpi, (IServiceCollection)this._services);
		return this;
	}

	public ServiceComposer AddFromAttributes() {
		foreach (var serviceType in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.CustomAttributes.Any(attr => attr.AttributeType.IsAssignableTo(typeof(ServiceAttribute))))) {
			var attribute = serviceType.GetCustomAttributes().First(attr => attr is ServiceAttribute);
			if (!(attribute is SingletonAttribute)) {
				if (attribute is TransientAttribute)
					this._services.AddTransient(serviceType);
			} else
				this._services.AddSingleton(serviceType);
		}
		this._services.BuildServiceProvider(new ServiceProviderOptions());
		return this;
	}

	public ServiceProvider BuildProvider() => this._services.BuildServiceProvider();
}
