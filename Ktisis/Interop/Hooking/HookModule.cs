// Decompiled with JetBrains decompiler
// Type: Ktisis.Interop.Hooking.HookModule
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ktisis.Interop.Hooking;

public abstract class HookModule : IHookModule, IDisposable {
	private readonly IHookMediator _hook;
	private readonly List<IHookWrapper> Hooks = new List<IHookWrapper>();
	private bool _init;
	private bool IsDisposed;

	protected HookModule(IHookMediator hook) {
		this._hook = hook;
	}

	public bool IsInit => this._init && !this.IsDisposed;

	public virtual void EnableAll() {
		this.Hooks.ForEach(hook => hook.Enable());
	}

	public virtual void DisableAll() {
		this.Hooks.ForEach(hook => hook.Disable());
	}

	public void SetEnabled(bool enabled) {
		if (enabled)
			this.EnableAll();
		else
			this.DisableAll();
	}

	public bool TryGetHook<T>(out HookWrapper<T>? result) where T : Delegate {
		result = null;
		foreach (var hook in this.Hooks) {
			if (hook is HookWrapper<T> hookWrapper) {
				result = hookWrapper;
				return true;
			}
		}
		return false;
	}

	public virtual bool Initialize() {
		if (this.IsDisposed)
			throw new Exception("Attempted to initialize disposed module.");
		var flag = this._hook.Init(this);
		var list = this.GetHookWrappers().ToList();
		if (flag) {
			this.Hooks.AddRange(list);
			flag &= this.OnInitialize();
		}
		if (!flag)
			this.Dispose();
		return this._init = flag;
	}

	public virtual void Dispose() {
		if (this.IsDisposed)
			return;
		try {
			this.Hooks.ForEach(hook => hook.Dispose());
			this.Hooks.Clear();
			this._hook.Remove(this);
		} finally {
			this.IsDisposed = true;
			GC.SuppressFinalize(this);
		}
	}

	protected virtual bool OnInitialize() => true;

	private IEnumerable<IHookWrapper> GetHookWrappers() {
		var hookModule = this;
		FieldInfo[] fieldInfoArray = hookModule.GetType().GetFields((BindingFlags)52);
		for (var index = 0; index < fieldInfoArray.Length; ++index) {
			var field = fieldInfoArray[index];
			IHookWrapper hookFromField;
			try {
				hookFromField = hookModule.GetHookFromField(field);
			} catch (Exception ex) {
				Ktisis.Ktisis.Log.Error($"Failed to resolve hook for field '{field.Name}':\n{ex}", Array.Empty<object>());
				continue;
			}
			if (hookFromField != null)
				yield return hookFromField;
		}
		fieldInfoArray = null;
	}

	private IHookWrapper? GetHookFromField(FieldInfo field) {
		var fieldType = field.FieldType;
		if (!fieldType.IsGenericType || fieldType.GetGenericTypeDefinition() != typeof(Hook<>))
			return null;
		var obj = field.GetValue(this);
		if (obj == null)
			return null;
		return (IHookWrapper)Activator.CreateInstance(typeof(HookWrapper<>).GetGenericTypeDefinition().MakeGenericType(fieldType.GenericTypeArguments), obj);
	}
}
