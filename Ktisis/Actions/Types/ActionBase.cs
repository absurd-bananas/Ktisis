// Decompiled with JetBrains decompiler
// Type: Ktisis.Actions.Types.ActionBase
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Actions.Attributes;
using Ktisis.Core.Types;
using System.Reflection;

#nullable enable
namespace Ktisis.Actions.Types;

public abstract class ActionBase
{
  protected IPluginContext Context { get; }

  protected ActionBase(IPluginContext ctx) => this.Context = ctx;

  public string GetName() => this.GetAttribute().Name;

  public ActionAttribute GetAttribute()
  {
    return CustomAttributeExtensions.GetCustomAttribute<ActionAttribute>((MemberInfo) this.GetType());
  }

  public virtual bool CanInvoke() => true;

  public abstract bool Invoke();
}
