// Decompiled with JetBrains decompiler
// Type: Ktisis.Interop.InteropService
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

using Ktisis.Core;
using Ktisis.Core.Attributes;
using Ktisis.Interop.Hooking;

namespace Ktisis.Interop;

[Singleton]
public class InteropService : IDisposable {
	private readonly DIBuilder _di;
	private readonly IGameInteropProvider _interop;
	private readonly List<HookModule> Modules = new List<HookModule>();
	private bool IsDisposing;

	public InteropService(DIBuilder di, IGameInteropProvider interop) {
		this._di = di;
		this._interop = interop;
	}

	public void Dispose() {
		this.IsDisposing = true;
		this.Modules.ForEach(mod => mod.Dispose());
		this.Modules.Clear();
		GC.SuppressFinalize(this);
	}

	public T CreateModule<T>(params object[] param) where T : HookModule {
		var element = new HookMediator(this);
		return this._di.Create<T>(param.Append(element).ToArray());
	}

	public HookScope CreateScope() => new HookScope(new HookMediator(this));

	private bool InitModule(HookModule module) {
		if (module.IsInit)
			return true;
		bool flag;
		try {
			this._interop.InitializeFromAttributes((object)module);
			flag = true;
		} catch (Exception ex) {
			flag = false;
			Ktisis.Ktisis.Log.Error($"Failed to initialize module '{module.GetType().Name}'\n{ex}", Array.Empty<object>());
		}
		return flag;
	}

	private bool RemoveModule(HookModule module) => !this.IsDisposing && this.Modules.Remove(module);

	private class HookMediator : IHookMediator {
		private readonly InteropService _interop;
		private HookModule? Module;

		public HookMediator(InteropService interop) {
			this._interop = interop;
		}

		public bool IsValid {
			get {
				if (this._interop.IsDisposing)
					return false;
				var module = this.Module;
				return module != null && module.IsInit;
			}
		}

		public T Create<T>(params object[] param) where T : HookModule => this._interop.CreateModule<T>(param);

		public bool Init(HookModule module) => this._interop.InitModule(module);

		public bool Remove(HookModule module) => this._interop.RemoveModule(module);
	}
}
