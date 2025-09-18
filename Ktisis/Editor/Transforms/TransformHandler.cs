// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Transforms.TransformHandler
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System.Linq;

using Ktisis.Actions.Types;
using Ktisis.Common.Utility;
using Ktisis.Editor.Actions;
using Ktisis.Editor.Context.Types;
using Ktisis.Editor.Selection;
using Ktisis.Editor.Transforms.Types;
using Ktisis.Scene.Decor;
using Ktisis.Scene.Entities.Skeleton;

namespace Ktisis.Editor.Transforms;

public class TransformHandler : ITransformHandler {
	private readonly IActionManager _action;
	private readonly IEditorContext _context;
	private readonly ISelectManager _select;

	public TransformHandler(IEditorContext context, IActionManager action, ISelectManager select) {
		this._context = context;
		this._action = action;
		this._select = select;
		select.Changed += this.OnSelectionChanged;
	}

	public ITransformTarget? Target { get; private set; }

	public ITransformMemento Begin(ITransformTarget target) {
		target.Setup.Configure(this._context.Config.Gizmo);
		return new TransformMemento(this, target).Save();
	}

	private void OnSelectionChanged(ISelectManager sender) {
		var list = TransformResolver.GetCorrelatingBones(this._select.GetSelected().Where(entity => entity != null && entity.IsValid), true).Where(entity => entity is ITransform).ToList();
		if (list.Count == 0) {
			this.Target = null;
		} else {
			var primary = list.FirstOrDefault();
			if (primary is SkeletonNode)
				primary = TransformResolver.GetPoseTarget(list);
			this.Target = new TransformTarget(primary, list);
		}
	}

	private class TransformMemento : ITransformMemento, IMemento {
		private readonly TransformHandler _handler;
		private readonly ITransformTarget Target;
		private Transform? Final;
		private Transform? Initial;
		private bool IsDispatch;
		private TransformSetup Setup = new TransformSetup();

		public TransformMemento(TransformHandler handler, ITransformTarget target) {
			this._handler = handler;
			this.Target = target;
		}

		public ITransformMemento Save() {
			this.Initial = this.Target.GetTransform();
			this.Setup = this.Target.Setup.\u003CClone\u003E\u0024();
			return this;
		}

		public void SetTransform(Transform transform) => this.Target.SetTransform(transform);

		public void SetMatrix(Matrix4x4 matrix) => this.Target.SetMatrix(matrix);

		public void Restore() {
			if (this.Initial == null)
				return;
			this.ApplyState(this.Initial);
		}

		public void Apply() {
			if (this.Final == null)
				return;
			this.ApplyState(this.Final);
		}

		public void Dispatch() {
			if (this.IsDispatch)
				return;
			this.IsDispatch = true;
			this.Final = this.Target.GetTransform();
			this._handler._action.History.Add(this);
		}

		private void ApplyState(Transform transform) {
			this.Target.Setup = this.Setup.\u003CClone\u003E\u0024();
			this.Target.SetTransform(transform);
		}
	}
}
