// Decompiled with JetBrains decompiler
// Type: Ktisis.Services.Plugin.DalamudServices
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

using Microsoft.Extensions.DependencyInjection;

namespace Ktisis.Services.Plugin;

public sealed class DalamudServices {
	[PluginService]
	private IChatGui ChatGui { get; set; }

	[PluginService]
	private IClientState ClientState { get; set; }

	[PluginService]
	private ICommandManager Cmd { get; set; }

	[PluginService]
	private IFramework Framework { get; set; }

	[PluginService]
	private IGameGui Gui { get; set; }

	[PluginService]
	private IGameInteropProvider Interop { get; set; }

	[PluginService]
	private IObjectTable ObjectTable { get; set; }

	[PluginService]
	private IKeyState KeyState { get; set; }

	[PluginService]
	private IDataManager Data { get; set; }

	[PluginService]
	private ITextureProvider Tex { get; set; }

	[PluginService]
	private ISigScanner SigScanner { get; set; }

	[PluginService]
	private ITargetManager TargetManager { get; set; }

	public void Add(IDalamudPluginInterface dpi, IServiceCollection services) {
		dpi.Inject((object)this, Array.Empty<object>());
		services.AddSingleton<IDalamudPluginInterface>(dpi).AddSingleton<IUiBuilder>(dpi.UiBuilder).AddSingleton<IClientState>(this.ClientState).AddSingleton<ICommandManager>(this.Cmd).AddSingleton<IFramework>(this.Framework)
			.AddSingleton<IGameGui>(this.Gui).AddSingleton<IGameInteropProvider>(this.Interop).AddSingleton<IObjectTable>(this.ObjectTable).AddSingleton<IKeyState>(this.KeyState).AddSingleton<IDataManager>(this.Data)
			.AddSingleton<ITextureProvider>(this.Tex).AddSingleton<ISigScanner>(this.SigScanner).AddSingleton<IChatGui>(this.ChatGui).AddSingleton<ITargetManager>(this.TargetManager);
	}
}
