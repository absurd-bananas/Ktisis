// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Context.ContextManager
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

using Ktisis.Core.Attributes;
using Ktisis.Core.Types;
using Ktisis.Data.Config.Sections;
using Ktisis.Editor.Context.Types;
using Ktisis.Services.Game;

using KtisisPyon.Common.Utility;

namespace Ktisis.Editor.Context;

[Singleton]
public class ContextManager : IDisposable {
	private readonly ContextBuilder _builder;
	private readonly GPoseService _gpose;
	private IEditorContext? _context;
	private bool _isInit;
	private IPluginContext? _plugin;

	public ContextManager(GPoseService gpose, ContextBuilder builder) {
		this._gpose = gpose;
		this._builder = builder;
	}

	public IEditorContext? Current {
		get {
			var context = this._context;
			return context == null || !context.IsValid ? null : context;
		}
	}

	public void Dispose() {
		this._isInit = false;
		this.Destroy();
		this._gpose.StateChanged -= this.OnGPoseEvent;
	}

	public void Initialize(IPluginContext context) {
		this._isInit = !this._isInit ? true : throw new Exception("Attempted double initialization of ContextManager.");
		this._plugin = context;
		this._gpose.StateChanged += this.OnGPoseEvent;
		this._gpose.Subscribe();
	}

	private void OnGPoseEvent(object sender, bool active) {
		if (!this._isInit)
			return;
		if (this._context != null && !active)
			Win32.SetWinDefault(this._context.Config.Pyon);
		this.Destroy();
		if (!active)
			return;
		this.SetupEditor();
		if (this._context == null)
			return;
		PyonConfig pyon;
		var pyonConfig1 = pyon = this._context.Config.Pyon;
		var pyonConfig2 = pyon;
		var pyonConfig3 = pyon;
		(pyonConfig1.DefaultPosition, pyonConfig2.DefaultSize, pyonConfig3.DefaultStyle, pyon.DefaultDeviceSize) = Win32.GetWinProperties();
	}

	private void SetupEditor() {
		if (!this._isInit || this._plugin == null)
			throw new Exception("Attempted to setup uninitialized context.");
		Ktisis.Ktisis.Log.Verbose("Creating new editor context...", Array.Empty<object>());
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		try {
			this._context = this._builder.Create(this._plugin);
			this._context.Initialize();
			this._gpose.Update += this.Update;
		} catch (Exception ex) {
			Ktisis.Ktisis.Log.Error($"failed to initialize editor state:\n{ex}", Array.Empty<object>());
			this.Destroy();
		}
		stopwatch.Stop();
		Ktisis.Ktisis.Log.Debug($"Editor context initialized in {stopwatch.Elapsed.TotalMilliseconds:00.00}ms", Array.Empty<object>());
	}

	private void Update() {
		if (!this._isInit)
			return;
		var context = this._context;
		if (context == null)
			return;
		if (context.IsValid)
			context.Update();
		else
			this.Destroy();
	}

	private void Destroy() {
		try {
			this._context?.Dispose();
		} catch (Exception ex) {
			Ktisis.Ktisis.Log.Error($"Failed to destroy editor state:\n{ex}", Array.Empty<object>());
		} finally {
			this._context = null;
		}
		this._gpose.Update -= this.Update;
	}
}
