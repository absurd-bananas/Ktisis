// Decompiled with JetBrains decompiler
// Type: Ktisis.Actions.ActionService
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Ktisis.Actions.Attributes;
using Ktisis.Actions.Types;
using Ktisis.Core;
using Ktisis.Core.Attributes;
using Ktisis.Core.Types;

namespace Ktisis.Actions;

[Singleton]
public class ActionService {
	private readonly DIBuilder _di;
	private readonly Dictionary<Type, ActionBase> Actions = new Dictionary<Type, ActionBase>();

	public ActionService(DIBuilder di) {
		this._di = di;
	}

	public void RegisterActions(IPluginContext context) {
		this.Actions.Clear();
		foreach (var resolveAction in ResolveActions()) {
			Type type1;
			ActionAttribute actionAttribute1;
			resolveAction.Deconstruct(ref type1, ref actionAttribute1);
			var type2 = type1;
			var actionAttribute2 = actionAttribute1;
			try {
				var actionBase = (ActionBase)this._di.Create(type2, context);
				this.Actions.Add(type2, actionBase);
			} catch (Exception ex) {
				Ktisis.Ktisis.Log.Error($"Failed to create action '{actionAttribute2.Name}'\n{ex}", Array.Empty<object>());
			}
		}
	}

	public T Get<T>() where T : ActionBase => (T)this.Actions[typeof(T)];

	public bool TryGet<T>(out T action) where T : ActionBase {
		var obj = default(T);
		ActionBase actionBase;
		if (this.Actions.TryGetValue(typeof(T), out actionBase))
			obj = (T)actionBase;
		action = obj;
		return obj != null;
	}

	public IEnumerable<ActionBase> GetAll() => this.Actions.Values;

	public IEnumerable<KeyAction> GetBindable() {
		return this.GetAll().Where(action => action is KeyAction).Cast<KeyAction>();
	}

	private static Dictionary<Type, ActionAttribute> ResolveActions() {
		return Assembly.GetExecutingAssembly().GetTypes().Select((Func<Type, (Type, ActionAttribute)>)(type => (type, type.GetCustomAttribute<ActionAttribute>()))).Where(pair => pair.attr != null)
			.ToDictionary((Func<(Type, ActionAttribute), Type>)(k => k.type), (Func<(Type, ActionAttribute), ActionAttribute>)(v => v.attr));
	}
}
