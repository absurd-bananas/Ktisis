// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Types.EntityEditWindow`1
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Ktisis.Editor.Context.Types;
using Ktisis.Scene.Entities;
using System;
using System.Linq;

#nullable enable
namespace Ktisis.Interface.Types;

public abstract class EntityEditWindow<T> : KtisisWindow where T : SceneEntity
{
  protected readonly IEditorContext Context;
  private T? _target;

  protected T Target
  {
    get => this._target;
    private set => this._target = value;
  }

  protected EntityEditWindow(string name, IEditorContext ctx, ImGuiWindowFlags flags = 0)
    : base(name, flags)
  {
    this.Context = ctx;
  }

  public virtual void PreDraw()
  {
    if (this.Context.IsValid)
    {
      T target = this._target;
      if ((object) target != null && target.IsValid)
        return;
    }
    Ktisis.Ktisis.Log.Verbose($"State for {((object) this).GetType().Name} is stale, closing...", Array.Empty<object>());
    this.Close();
  }

  public virtual void SetTarget(T target)
  {
    this.Target = target.IsValid ? target : throw new Exception("Attempted to set invalid target.");
  }

  protected void UpdateTarget()
  {
    if (this.Context.Config.Editor.UseLegacyWindowBehavior)
      return;
    T target = (T) this.Context.Selection.GetSelected().FirstOrDefault<SceneEntity>((Func<SceneEntity, bool>) (entity => entity is T));
    if ((object) target == null || (object) this.Target == (object) target)
      return;
    this.SetTarget(target);
  }
}
