// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Windows.ObjectWindow
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

using GLib.Widgets;

using Ktisis.Editor.Context.Types;
using Ktisis.Editor.Transforms.Types;
using Ktisis.ImGuizmo;
using Ktisis.Interface.Components.Objects;
using Ktisis.Interface.Components.Transforms;
using Ktisis.Interface.Types;
using Ktisis.Services.Game;

namespace Ktisis.Interface.Windows;

public class ObjectWindow : KtisisWindow {
	private readonly IEditorContext _ctx;
	private readonly Gizmo2D _gizmo;
	private readonly PropertyEditor _propEditor;
	private readonly TransformTable _table;
	private ITransformMemento? Transform;

	public ObjectWindow(
		IEditorContext ctx,
		Gizmo2D gizmo,
		TransformTable table,
		PropertyEditor propEditor
	)
		: base("Object Editor") {
		this._ctx = ctx;
		this._gizmo = gizmo;
		this._table = table;
		this._propEditor = propEditor;
	}

	public override void OnCreate() => this._propEditor.Prepare(this._ctx);

	public virtual void PreOpenCheck() {
		if (this._ctx.IsValid)
			return;
		Ktisis.Ktisis.Log.Verbose("Context for transform window is stale, closing...", Array.Empty<object>());
		this.Close();
	}

	public virtual void PreDraw() {
		var num1 = (double)TransformTable.CalcWidth();
		ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		var num2 = (double)((ImGuiStylePtr) ref style ).WindowPadding.X * 2.0;
		var x = (float)(num1 + num2);
		Window.WindowSizeConstraints windowSizeConstraints;
		// ISSUE: explicit constructor call
		((Window.WindowSizeConstraints) ref windowSizeConstraints).\u002Ector();
		((Window.WindowSizeConstraints) ref windowSizeConstraints).MinimumSize = new Vector2(x, 0.0f);
		this.SizeConstraints = new Window.WindowSizeConstraints?(windowSizeConstraints);
	}

	public virtual void Draw() {
		this.DrawToggles();
		var target = this._ctx.Transform.Target;
		this.DrawTransform(target);
		this.DrawProperties(target);
	}

	private void DrawTransform(ITransformTarget? target) {
		Ktisis.Common.Utility.Transform transform = target?.GetTransform() ?? new Ktisis.Common.Utility.Transform();
		var disabled = target == null;
		using (ImRaii.Disabled(disabled)) {
			bool isEnded;
			var flag = this.DrawTransform(ref transform, out isEnded, disabled);
			if (target != null & flag) {
				if (this.Transform == null)
					this.Transform = this._ctx.Transform.Begin(target);
				this.Transform.SetTransform(transform);
			}
			if (!isEnded)
				return;
			this.Transform?.Dispatch();
			this.Transform = null;
		}
	}

	private void DrawProperties(ITransformTarget? target) {
		var entity = this._ctx.Selection.GetFirstSelected() ?? target?.Primary;
		if (entity == null)
			return;
		this._propEditor.Draw(entity);
	}

	private bool DrawTransform(ref Ktisis.Common.Utility.Transform transform, out bool isEnded, bool disabled) {
		isEnded = false;
		var flag1 = false;
		if (!this._ctx.Config.Editor.TransformHide) {
			flag1 = this.DrawGizmo(ref transform, Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail().X, disabled);
			isEnded = this._gizmo.IsEnded;
		}
		Ktisis.Common.Utility.Transform transOut;
		var flag2 = this._table.Draw(transform, out transOut, TransformTableFlags.Default | TransformTableFlags.UseAvailable);
		if (flag2)
			transform = transOut;
		isEnded |= this._table.IsDeactivated;
		return flag1 | flag2;
	}

	private void DrawToggles() {
		ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		float x1 = ((ImGuiStylePtr) ref style ).ItemInnerSpacing.X;
		ImFontPtr iconFont = UiBuilder.IconFont;
		float num1 = ((ImFontPtr) ref iconFont ).FontSize * 2f;
		Vector2 vector2 = new Vector2(num1, num1);
		var mode = this._ctx.Config.Gizmo.Mode;
		if (Buttons.IconButtonTooltip(mode == Mode.World ? (FontAwesomeIcon)61612 : (FontAwesomeIcon)61461, this._ctx.Locale.Translate("transform_edit.mode." + (mode == Mode.World ? "world" : "local")), new Vector2?(vector2)))
			this._ctx.Config.Gizmo.Mode = mode == Mode.World ? Mode.Local : Mode.World;
		Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x1);
		var visible = this._ctx.Config.Gizmo.Visible;
		if (Buttons.IconButtonTooltip(visible ? (FontAwesomeIcon)61550 : (FontAwesomeIcon)61552, this._ctx.Locale.Translate("actions.Gizmo_Toggle"), new Vector2?(vector2)))
			this._ctx.Config.Gizmo.Visible = !visible;
		Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x1);
		var num2 = this._ctx.Config.Gizmo.MirrorRotation ? 1 : 0;
		if (Buttons.IconButtonTooltip(num2 != 0 ? (FontAwesomeIcon)58543 : (FontAwesomeIcon)63396, this._ctx.Locale.Translate("transform_edit.flags." + (num2 != 0 ? "mirror" : "parallel")), new Vector2?(vector2))) {
			var gizmo = this._ctx.Config.Gizmo;
			gizmo.MirrorRotation = !gizmo.MirrorRotation;
		}
		var actorFromTarget = this._ctx.Posing.GetActorFromTarget(this._ctx.Transform.Target);
		if (actorFromTarget != null) {
			Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x1);
			if (Buttons.IconButtonTooltip((FontAwesomeIcon)58561, "Flip Pose", new Vector2?(vector2)))
				this._ctx.Posing.ApplyPoseFlip(actorFromTarget);
		}
		Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x1);
		float x2 = Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail().X;
		if (x2 > (double)num1)
			Dalamud.Bindings.ImGui.ImGui.SetCursorPosX(Dalamud.Bindings.ImGui.ImGui.GetCursorPosX() + x2 - num1);
		var transformHide = this._ctx.Config.Editor.TransformHide;
		if (!Buttons.IconButtonTooltip(transformHide ? (FontAwesomeIcon)61656 : (FontAwesomeIcon)61655, this._ctx.Locale.Translate("transform_edit.gizmo." + (transformHide ? "show" : "hide")), new Vector2?(vector2)))
			return;
		this._ctx.Config.Editor.TransformHide = !transformHide;
	}

	private unsafe bool DrawGizmo(ref Ktisis.Common.Utility.Transform transform, float width, bool disabled) {
		Vector2 rectSize = new Vector2(width, 200f);
		this._gizmo.Begin(rectSize);
		this._gizmo.Mode = this._ctx.Config.Gizmo.Mode;
		if (disabled) {
			this._gizmo.End();
			return false;
		}
		Camera* gameCamera = CameraService.GetGameCamera();
		float num1 = (IntPtr)gameCamera != IntPtr.Zero ? gameCamera->FoV : 1f;
		Vector3 vector3 = (IntPtr)gameCamera != IntPtr.Zero ? Vector3.op_Implicit(gameCamera->CameraBase.SceneCamera.Object.Position) : Vector3.Zero;
		Matrix4x4 matrix = transform.ComposeMatrix();
		var gizmo = this._gizmo;
		Vector3 cameraPos = vector3;
		Vector3 translation = matrix.Translation;
		var fov = (double)num1;
		var x = (double)rectSize.X;
		ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		var num2 = (double)((ImGuiStylePtr) ref style ).WindowPadding.X * 2.0;
		var num3 = x - num2;
		var y = (double)rectSize.Y;
		style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		var num4 = (double)((ImGuiStylePtr) ref style ).WindowPadding.Y * 2.0;
		var num5 = y - num4;
		var aspect = num3 / num5;
		gizmo.SetLookAt(cameraPos, translation, (float)fov, (float)aspect);
		var num6 = this._gizmo.Manipulate(ref matrix, out Matrix4x4 _) ? 1 : 0;
		this._gizmo.End();
		if (num6 == 0)
			return num6 != 0;
		transform.DecomposeMatrix(matrix);
		return num6 != 0;
	}
}
