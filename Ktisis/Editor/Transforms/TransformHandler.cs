// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Transforms.TransformHandler
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Actions.Types;
using Ktisis.Common.Utility;
using Ktisis.Editor.Actions;
using Ktisis.Editor.Context.Types;
using Ktisis.Editor.Selection;
using Ktisis.Editor.Transforms.Types;
using Ktisis.Scene.Decor;
using Ktisis.Scene.Entities;
using Ktisis.Scene.Entities.Skeleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Ktisis.Editor.Transforms;

public class TransformHandler : ITransformHandler
{
  private readonly IEditorContext _context;
  private readonly IActionManager _action;
  private readonly ISelectManager _select;

  public TransformHandler(IEditorContext context, IActionManager action, ISelectManager select)
  {
    this._context = context;
    this._action = action;
    this._select = select;
    select.Changed += new SelectChangedHandler(this.OnSelectionChanged);
  }

  public ITransformTarget? Target { get; private set; }

  private void OnSelectionChanged(ISelectManager sender)
  {
    List<SceneEntity> list = TransformResolver.GetCorrelatingBones(this._select.GetSelected().Where<SceneEntity>((Func<SceneEntity, bool>) (entity => entity != null && entity.IsValid)), true).Where<SceneEntity>((Func<SceneEntity, bool>) (entity => entity is ITransform)).ToList<SceneEntity>();
    if (list.Count == 0)
    {
      this.Target = (ITransformTarget) null;
    }
    else
    {
      SceneEntity primary = list.FirstOrDefault<SceneEntity>();
      if (primary is SkeletonNode)
        primary = TransformResolver.GetPoseTarget((IEnumerable<SceneEntity>) list);
      this.Target = (ITransformTarget) new TransformTarget(primary, (IEnumerable<SceneEntity>) list);
    }
  }

  public ITransformMemento Begin(ITransformTarget target)
  {
    target.Setup.Configure(this._context.Config.Gizmo);
    return new TransformHandler.TransformMemento(this, target).Save();
  }

  private class TransformMemento : ITransformMemento, IMemento
  {
    private readonly TransformHandler _handler;
    private readonly ITransformTarget Target;
    private TransformSetup Setup = new TransformSetup();
    private Transform? Initial;
    private Transform? Final;
    private bool IsDispatch;

    public TransformMemento(TransformHandler handler, ITransformTarget target)
    {
      this._handler = handler;
      this.Target = target;
    }

    public ITransformMemento Save()
    {
      this.Initial = this.Target.GetTransform();
      this.Setup = this.Target.Setup.\u003CClone\u003E\u0024();
      return (ITransformMemento) this;
    }

    public void SetTransform(Transform transform) => this.Target.SetTransform(transform);

    public void SetMatrix(Matrix4x4 matrix) => this.Target.SetMatrix(matrix);

    public void Restore()
    {
      if (this.Initial == null)
        return;
      this.ApplyState(this.Initial);
    }

    public void Apply()
    {
      if (this.Final == null)
        return;
      this.ApplyState(this.Final);
    }

    private void ApplyState(Transform transform)
    {
      this.Target.Setup = this.Setup.\u003CClone\u003E\u0024();
      this.Target.SetTransform(transform);
    }

    public void Dispatch()
    {
      if (this.IsDispatch)
        return;
      this.IsDispatch = true;
      this.Final = this.Target.GetTransform();
      this._handler._action.History.Add((IMemento) this);
    }
  }
}
