// Decompiled with JetBrains decompiler
// Type: Ktisis.Actions.Types.ActionBase
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System.Reflection;

using Ktisis.Actions.Attributes;
using Ktisis.Core.Types;

namespace Ktisis.Actions.Types;

public abstract class ActionBase {

	protected ActionBase(IPluginContext ctx) {
		this.Context = ctx;
	}
	protected IPluginContext Context { get; }

	public string GetName() => this.GetAttribute().Name;

	public ActionAttribute GetAttribute() => this.GetType().GetCustomAttribute<ActionAttribute>();

	public virtual bool CanInvoke() => true;

	public abstract bool Invoke();
}
