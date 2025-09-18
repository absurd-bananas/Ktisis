// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Workspace.SceneDragDropHandler
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

using GLib.Widgets;

using Ktisis.Editor.Context.Types;
using Ktisis.Editor.Posing.Attachment;
using Ktisis.Scene.Decor;
using Ktisis.Scene.Entities;

namespace Ktisis.Interface.Components.Workspace;

public class SceneDragDropHandler {
	private const string PayloadId = "KTISIS_SCENE_NODE";
	private readonly IEditorContext _ctx;
	private SceneEntity? Source;

	public SceneDragDropHandler(IEditorContext ctx) {
		this._ctx = ctx;
	}

	private IAttachManager Manager => this._ctx.Posing.Attachments;

	public void Handle(SceneEntity entity) {
		this.HandleSource(entity);
		if (this.Source == null)
			return;
		this.HandleTarget(entity);
	}

	private void HandleSource(SceneEntity entity) {
		using (ImRaii.IEndObject iendObject = ImRaii.DragDropSource((ImGuiDragDropFlags)2)) {
			if (!iendObject.Success)
				return;
			Dalamud.Bindings.ImGui.ImGui.SetDragDropPayload(ImU8String.op_Implicit("KTISIS_SCENE_NODE"), ReadOnlySpan<byte>.Empty, (ImGuiCond)0);
			this.Source = entity;
			EntityDisplay entityDisplay = this._ctx.Config.GetEntityDisplay(entity);
			using (ImRaii.PushColor((ImGuiCol)0, entityDisplay.Color, true)) {
				FontAwesomeIcon icon = entityDisplay.Icon;
				if (icon != null) {
					Icons.DrawIcon(icon);
					ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
					Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, ((ImGuiStylePtr) ref style).ItemInnerSpacing.X);
				}
				Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(entity.Name));
			}
		}
	}

	private void HandleTarget(SceneEntity entity) {
		using (ImRaii.IEndObject iendObject = ImRaii.DragDropTarget()) {
			if (!iendObject.Success || (IntPtr)Dalamud.Bindings.ImGui.ImGui.AcceptDragDropPayload(ImU8String.op_Implicit("KTISIS_SCENE_NODE"), (ImGuiDragDropFlags)0).Handle == IntPtr.Zero)
				return;
			var source = this.Source;
			if (source == null)
				return;
			this.HandlePayload(entity, source);
		}
	}

	private void HandlePayload(SceneEntity target, SceneEntity source) {
		Ktisis.Ktisis.Log.Info($"{target.Name} accepting payload from {source.Name}", Array.Empty<object>());
		if (!(target is IAttachTarget target1) || !(source is IAttachable child))
			return;
		this.Manager.Attach(child, target1);
	}
}
