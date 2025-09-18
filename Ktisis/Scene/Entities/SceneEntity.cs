// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Entities.SceneEntity
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Editor.Selection;
using Ktisis.Scene.Types;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Ktisis.Scene.Entities;

public abstract class SceneEntity : IComposite
{
  protected readonly ISceneManager Scene;
  private readonly List<SceneEntity> _children = new List<SceneEntity>();

  public string Name { get; set; } = string.Empty;

  public EntityType Type { get; protected init; }

  public virtual bool IsValid => this.Scene.IsValid && this.Parent != null;

  protected SceneEntity(ISceneManager scene) => this.Scene = scene;

  public virtual void Update()
  {
    if (!this.IsValid)
      return;
    foreach (SceneEntity child in this.Children)
      child.Update();
  }

  private ISelectManager Selection => this.Scene.Context.Selection;

  public bool IsSelected => this.Selection.IsSelected(this);

  public void Select(SelectMode mode = SelectMode.Default) => this.Selection.Select(this, mode);

  public void Unselect() => this.Selection.Unselect(this);

  public virtual SceneEntity? Parent { get; set; }

  public virtual IEnumerable<SceneEntity> Children => (IEnumerable<SceneEntity>) this._children;

  protected List<SceneEntity> GetChildren() => this._children;

  public virtual bool Add(SceneEntity entity)
  {
    if (this._children.Contains(entity))
      return false;
    this._children.Add(entity);
    entity.Parent?.Remove(entity);
    entity.Parent = this;
    return true;
  }

  public virtual bool Remove(SceneEntity entity)
  {
    int num = this._children.Remove(entity) ? 1 : 0;
    entity.Parent = (SceneEntity) null;
    return num != 0;
  }

  public virtual void Remove()
  {
    this.Parent?.Remove(this);
    this.Clear();
  }

  public virtual void Clear()
  {
    foreach (SceneEntity sceneEntity in this.Children.ToList<SceneEntity>())
      sceneEntity.Remove();
  }

  public IEnumerable<SceneEntity> Recurse()
  {
    foreach (SceneEntity child in this.Children)
    {
      yield return child;
      IEnumerator<SceneEntity> enumerator = child.Recurse().GetEnumerator();
      while (enumerator.MoveNext())
        yield return enumerator.Current;
      enumerator = (IEnumerator<SceneEntity>) null;
    }
  }

  public bool IsChildOf(SceneEntity entity)
  {
    SceneEntity parent = this.Parent;
    for (int index = 0; parent != null && index++ < 1000; parent = parent.Parent)
    {
      if (parent == entity)
        return true;
    }
    return false;
  }
}
