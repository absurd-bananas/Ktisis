// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Selection.SelectManager
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Editor.Context.Types;
using Ktisis.Events;
using Ktisis.Scene.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Ktisis.Editor.Selection;

public class SelectManager : ISelectManager
{
  private readonly IEditorContext _context;
  private readonly Event<Action<ISelectManager>> _changed = new Event<Action<ISelectManager>>();
  private readonly List<SceneEntity> Selected = new List<SceneEntity>();

  public event SelectChangedHandler Changed
  {
    add => this._changed.Add(new Action<ISelectManager>(value.Invoke));
    remove => this._changed.Remove(new Action<ISelectManager>(value.Invoke));
  }

  public SelectManager(IEditorContext context) => this._context = context;

  public void Update()
  {
    if (this.Selected.RemoveAll((Predicate<SceneEntity>) (item => !item.IsValid)) <= 0)
      return;
    this.InvokeChanged();
  }

  public int Count => this.Selected.Count;

  public IEnumerable<SceneEntity> GetSelected()
  {
    return (IEnumerable<SceneEntity>) this.Selected.AsReadOnly();
  }

  public SceneEntity? GetFirstSelected() => this.Selected.FirstOrDefault<SceneEntity>();

  public bool IsSelected(SceneEntity entity) => this.Selected.Contains(entity);

  public void Select(SceneEntity entity)
  {
    this.Selected.Remove(entity);
    this.Selected.Add(entity);
    this.InvokeChanged();
  }

  public void Select(SceneEntity entity, SelectMode mode)
  {
    if (mode == SelectMode.Force)
    {
      if (this.IsSelected(entity) && this.Count == 1)
        return;
      this.Selected.Clear();
      this.Selected.Add(entity);
      this.InvokeChanged();
    }
    else
    {
      int num = this.IsSelected(entity) ? 1 : 0;
      bool flag1 = this.Count > 1;
      bool flag2 = mode == SelectMode.Multiple;
      if (!flag2)
        this.Selected.Clear();
      if (num == 0 || !flag2 & flag1)
        this.Selected.Add(entity);
      else
        this.Selected.Remove(entity);
      this.InvokeChanged();
    }
  }

  public void Unselect(SceneEntity entity)
  {
    if (!this.Selected.Remove(entity))
      return;
    this.InvokeChanged();
  }

  public void Clear()
  {
    this.Selected.Clear();
    this.InvokeChanged();
  }

  private void InvokeChanged()
  {
    try
    {
      this._changed.Invoke<SelectManager>(this);
    }
    catch (Exception ex)
    {
      Ktisis.Ktisis.Log.Error(ex.ToString(), Array.Empty<object>());
    }
  }
}
