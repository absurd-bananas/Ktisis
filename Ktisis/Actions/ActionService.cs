// Decompiled with JetBrains decompiler
// Type: Ktisis.Actions.ActionService
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Actions.Attributes;
using Ktisis.Actions.Types;
using Ktisis.Core;
using Ktisis.Core.Attributes;
using Ktisis.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable enable
namespace Ktisis.Actions;

[Singleton]
public class ActionService
{
  private readonly DIBuilder _di;
  private readonly Dictionary<Type, ActionBase> Actions = new Dictionary<Type, ActionBase>();

  public ActionService(DIBuilder di) => this._di = di;

  public void RegisterActions(IPluginContext context)
  {
    this.Actions.Clear();
    foreach (KeyValuePair<Type, ActionAttribute> resolveAction in ActionService.ResolveActions())
    {
      Type type1;
      ActionAttribute actionAttribute1;
      resolveAction.Deconstruct(ref type1, ref actionAttribute1);
      Type type2 = type1;
      ActionAttribute actionAttribute2 = actionAttribute1;
      try
      {
        ActionBase actionBase = (ActionBase) this._di.Create(type2, (object) context);
        this.Actions.Add(type2, actionBase);
      }
      catch (Exception ex)
      {
        Ktisis.Ktisis.Log.Error($"Failed to create action '{actionAttribute2.Name}'\n{ex}", Array.Empty<object>());
      }
    }
  }

  public T Get<T>() where T : ActionBase => (T) this.Actions[typeof (T)];

  public bool TryGet<T>(out T action) where T : ActionBase
  {
    T obj = default (T);
    ActionBase actionBase;
    if (this.Actions.TryGetValue(typeof (T), out actionBase))
      obj = (T) actionBase;
    action = obj;
    return (object) obj != null;
  }

  public IEnumerable<ActionBase> GetAll() => (IEnumerable<ActionBase>) this.Actions.Values;

  public IEnumerable<KeyAction> GetBindable()
  {
    return this.GetAll().Where<ActionBase>((Func<ActionBase, bool>) (action => action is KeyAction)).Cast<KeyAction>();
  }

  private static Dictionary<Type, ActionAttribute> ResolveActions()
  {
    return ((IEnumerable<Type>) Assembly.GetExecutingAssembly().GetTypes()).Select<Type, (Type, ActionAttribute)>((Func<Type, (Type, ActionAttribute)>) (type => (type, CustomAttributeExtensions.GetCustomAttribute<ActionAttribute>((MemberInfo) type)))).Where<(Type, ActionAttribute)>((Func<(Type, ActionAttribute), bool>) (pair => pair.attr != null)).ToDictionary<(Type, ActionAttribute), Type, ActionAttribute>((Func<(Type, ActionAttribute), Type>) (k => k.type), (Func<(Type, ActionAttribute), ActionAttribute>) (v => v.attr));
  }
}
