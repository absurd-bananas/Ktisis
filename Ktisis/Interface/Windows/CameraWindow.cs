// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Windows.CameraWindow
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

using GLib.Widgets;

using Ktisis.Common.Utility;
using Ktisis.Editor.Camera.Types;
using Ktisis.Editor.Context.Types;
using Ktisis.Interface.Components.Transforms;
using Ktisis.Interface.Types;

namespace Ktisis.Interface.Windows;

public class CameraWindow : KtisisWindow {
	private const TransformTableFlags TransformFlags = TransformTableFlags.Default | TransformTableFlags.UseAvailable;
	private readonly IEditorContext _ctx;
	private readonly TransformTable _fixedPos;
	private readonly TransformTable _relativePos;

	public CameraWindow(IEditorContext ctx, TransformTable fixedPos, TransformTable relativePos)
		: base("Camera Editor") {
		this._ctx = ctx;
		this._fixedPos = fixedPos;
		this._relativePos = relativePos;
	}

	public virtual void PreOpenCheck() {
		var ctx = this._ctx;
		if (ctx != null && ctx.IsValid) {
			var cameras = ctx.Cameras;
			if (cameras != null && cameras.Current != null)
				return;
		}
		Ktisis.Ktisis.Log.Verbose("State for camera window is stale, closing.", Array.Empty<object>());
		this.Close();
	}

	public virtual void PreDraw() {
		this.SizeCondition = (ImGuiCond)1;
		Window.WindowSizeConstraints windowSizeConstraints;
		// ISSUE: explicit constructor call
		((Window.WindowSizeConstraints) ref windowSizeConstraints).\u002Ector();
		((Window.WindowSizeConstraints) ref windowSizeConstraints).MinimumSize = new Vector2(TransformTable.CalcWidth(), 300f);
		ref Window.WindowSizeConstraints local = ref windowSizeConstraints;
		ImGuiIOPtr io = Dalamud.Bindings.ImGui.ImGui.GetIO();
		Vector2 vector2 = ((ImGuiIOPtr) ref io ).DisplaySize * 0.75f;
		((Window.WindowSizeConstraints) ref local).MaximumSize = vector2;
		this.SizeConstraints = new Window.WindowSizeConstraints?(windowSizeConstraints);
	}

	public virtual void Draw() {
		var current = this._ctx.Cameras.Current;
		if (current == null || !current.IsValid)
			return;
		this.DrawToggles(current);
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		Dalamud.Bindings.ImGui.ImGui.SetNextItemWidth(Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail().X);
		Dalamud.Bindings.ImGui.ImGui.InputText(ImU8String.op_Implicit("##CameraName"), ref current.Name, 64 /*0x40*/, (ImGuiInputTextFlags)0, (Dalamud.Bindings.ImGui.ImGui.ImGuiInputTextCallbackDelegate)null);
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		Dalamud.Bindings.ImGui.ImGui.Separator();
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		this.DrawOrbitTarget(current);
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		this.DrawFixedPosition(current);
		this.DrawRelativeOffset(current);
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		this.DrawAnglePan(current);
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		this.DrawSliders(current);
		if (!(current is WorkCamera))
			return;
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		Dalamud.Bindings.ImGui.ImGui.Separator();
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		this.DrawFreeCamOptions(current);
	}

	private void DrawFreeCamOptions(EditorCamera camera) {
		Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Work camera options"));
		Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Adjust move speed with scroll wheel##"), ref this._ctx.Config.Camera.ScrollWheelAdjustsSpeed);
		if (Dalamud.Bindings.ImGui.ImGui.IsItemHovered())
			Dalamud.Bindings.ImGui.ImGui.SetTooltip(ImU8String.op_Implicit("While work camera is active, scrolling will adust movement speed instead of zoom.\nThis speed is a modifier on the below default speed."));
		this.DrawSliderFloat("##WorkCameraSpeed", (FontAwesomeIcon)61518, ref this._ctx.Config.Camera.DefaultWorkCamSpeed, 0.01f, 2f, 0.05f, "Default movement speed for work camera.");
		this.DrawSliderFloat("##WorkCameraPanSensitivity", (FontAwesomeIcon)61541, ref this._ctx.Config.Camera.PanSensitivityModifier, 0.01f, 2f, 0.05f, "Sensitivity modifier for mouse camera pan.");
	}

	private void DrawToggles(EditorCamera camera) {
		var flag = !camera.Flags.HasFlag(CameraFlags.NoCollide);
		if (Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this._ctx.Locale.Translate("camera_edit.toggles.collide")), ref flag))
			camera.Flags ^= CameraFlags.NoCollide;
		Dalamud.Bindings.ImGui.ImGui.SameLine();
		var delimit = camera.Flags.HasFlag(CameraFlags.Delimit);
		if (Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this._ctx.Locale.Translate("camera_edit.toggles.delimit")), ref delimit))
			camera.SetDelimited(delimit);
		this.DrawOrthographicToggle(camera);
	}

	private unsafe void DrawOrthographicToggle(EditorCamera camera) {
		if ((IntPtr)camera.Camera == IntPtr.Zero || (IntPtr)camera.Camera->RenderEx == IntPtr.Zero)
			return;
		Dalamud.Bindings.ImGui.ImGui.SameLine();
		var isOrthographic = camera.IsOrthographic;
		if (!Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this._ctx.Locale.Translate("camera_edit.toggles.ortho")), ref isOrthographic))
			return;
		camera.SetOrthographic(isOrthographic);
	}

	private void DrawOrbitTarget(EditorCamera camera) {
		using (ImRaii.PushId(ImU8String.op_Implicit("CameraOrbitTarget"), true)) {
			IGameObject igameObject = this._ctx.Cameras.ResolveOrbitTarget(camera);
			if (igameObject == null)
				return;
			var hasValue = camera.OrbitTarget.HasValue;
			if (Buttons.IconButtonTooltip(hasValue ? (FontAwesomeIcon)61475 : (FontAwesomeIcon)61596, hasValue ? this._ctx.Locale.Translate("camera_edit.orbit.unlock") : this._ctx.Locale.Translate("camera_edit.orbit.lock")))
				camera.OrbitTarget = hasValue ? new ushort?() : igameObject.ObjectIndex;
			Dalamud.Bindings.ImGui.ImGui.SameLine();
			var str = "Orbiting: " + igameObject.Name.TextValue;
			if (hasValue)
				Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(str));
			else
				Dalamud.Bindings.ImGui.ImGui.TextDisabled(ImU8String.op_Implicit(str));
			Dalamud.Bindings.ImGui.ImGui.SameLine();
			Dalamud.Bindings.ImGui.ImGui.SetCursorPosX(Dalamud.Bindings.ImGui.ImGui.GetCursorPosX() + Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail().X - Buttons.CalcSize());
			if (Buttons.IconButtonTooltip((FontAwesomeIcon)61473, this._ctx.Locale.Translate("camera_edit.offset.to_target")))
				camera.SetOffsetPositionToTarget(this._ctx, false);
			Dalamud.Bindings.ImGui.ImGui.SameLine();
			var num1 = (double)Dalamud.Bindings.ImGui.ImGui.GetCursorPosX() + (double)Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail().X;
			var num2 = Buttons.CalcSize() * 2.0;
			ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
			var x = (double)((ImGuiStylePtr) ref style ).ItemInnerSpacing.X;
			var num3 = num2 + x;
			Dalamud.Bindings.ImGui.ImGui.SetCursorPosX((float)(num1 - num3));
			if (!Buttons.IconButtonTooltip((FontAwesomeIcon)58594, "Offset camera to target pose"))
				return;
			camera.SetOffsetPositionToTarget(this._ctx, true);
		}
	}

	private void DrawFixedPosition(EditorCamera camera) {
		using (ImRaii.PushId(ImU8String.op_Implicit("CameraFixedPosition"), true)) {
			Vector3? position1 = camera.GetPosition();
			if (!position1.HasValue)
				return;
			Vector3 position2 = position1.Value;
			var hasValue = camera.FixedPosition.HasValue;
			if (!hasValue)
				position2 -= camera.RelativeOffset;
			if (Buttons.IconButtonTooltip(hasValue ? (FontAwesomeIcon)61475 : (FontAwesomeIcon)61596, hasValue ? this._ctx.Locale.Translate("camera_edit.position.unlock") : this._ctx.Locale.Translate("camera_edit.position.lock")))
				camera.FixedPosition = hasValue ? new Vector3?() : new Vector3?(position2);
			ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
			Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, ((ImGuiStylePtr) ref style).ItemInnerSpacing.X);
			using (ImRaii.Disabled(!hasValue)) {
				if (!this._fixedPos.DrawPosition(ref position2, TransformTableFlags.Default | TransformTableFlags.UseAvailable))
					return;
				camera.FixedPosition = new Vector3?(position2);
			}
		}
	}

	private void DrawRelativeOffset(EditorCamera camera) {
		float spacing;
		this.DrawIconAlign((FontAwesomeIcon)61543, out spacing, this._ctx.Locale.Translate("camera_edit.offset.from_base"));
		Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, spacing);
		this._relativePos.DrawPosition(ref camera.RelativeOffset, TransformTableFlags.Default | TransformTableFlags.UseAvailable);
	}

	private unsafe void DrawAnglePan(EditorCamera camera) {
		var camera1 = camera.Camera;
		if ((IntPtr)camera1 == IntPtr.Zero)
			return;
		var hint1 = this._ctx.Locale.Translate("camera_edit.angle");
		float spacing;
		this.DrawIconAlign((FontAwesomeIcon)58555, out spacing, hint1);
		Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, spacing);
		Dalamud.Bindings.ImGui.ImGui.SetNextItemWidth(Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail().X);
		Vector2 vector2_1 = camera1->Angle * MathHelpers.Rad2Deg;
		if (Dalamud.Bindings.ImGui.ImGui.DragFloat2(ImU8String.op_Implicit("##CameraAngle"), ref vector2_1, 0.25f, 0.0f, 0.0f, new ImU8String(), (ImGuiSliderFlags)0))
			camera1->Angle = vector2_1 * MathHelpers.Deg2Rad;
		var hint2 = this._ctx.Locale.Translate("camera_edit.pan");
		this.DrawIconAlign((FontAwesomeIcon)61618, out spacing, hint2);
		Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, spacing);
		Dalamud.Bindings.ImGui.ImGui.SetNextItemWidth(Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail().X);
		Vector2 vector2_2 = camera1->Pan * MathHelpers.Rad2Deg;
		if (!Dalamud.Bindings.ImGui.ImGui.DragFloat2(ImU8String.op_Implicit("##CameraPan"), ref vector2_2, 0.25f, 0.0f, 0.0f, new ImU8String(), (ImGuiSliderFlags)0))
			return;
		vector2_2.X %= 360f;
		vector2_2.Y %= 360f;
		camera1->Pan = vector2_2 * MathHelpers.Deg2Rad;
	}

	private unsafe void DrawSliders(EditorCamera camera) {
		var camera1 = camera.Camera;
		if ((IntPtr)camera1 == IntPtr.Zero)
			return;
		var hint1 = this._ctx.Locale.Translate("camera_edit.sliders.rotation");
		var hint2 = this._ctx.Locale.Translate("camera_edit.sliders.zoom");
		var hint3 = this._ctx.Locale.Translate("camera_edit.sliders.distance");
		this.DrawSliderAngle("##CameraRotate", (FontAwesomeIcon)57560, ref camera1->Rotation, -180f, 180f, 0.5f, hint1);
		this.DrawSliderAngle("##CameraZoom", (FontAwesomeIcon)62923, ref camera1->Zoom, -40f, 100f, 0.5f, hint2);
		this.DrawSliderFloat("##CameraDistance", (FontAwesomeIcon)61830, ref camera1->Distance, camera1->DistanceMin, camera1->DistanceMax, 0.05f, hint3);
		if (!camera.IsOrthographic)
			return;
		var hint4 = this._ctx.Locale.Translate("camera_edit.sliders.ortho_zoom");
		this.DrawSliderFloat("##OrthographicZoom", (FontAwesomeIcon)62977, ref camera.OrthographicZoom, 0.1f, 10f, 0.01f, hint4);
	}

	private void DrawSliderAngle(
		string label,
		FontAwesomeIcon icon,
		ref float value,
		float min,
		float max,
		float drag,
		string hint = ""
	) {
		this.DrawSliderIcon(icon, hint);
		Dalamud.Bindings.ImGui.ImGui.SliderAngle(ImU8String.op_Implicit(label), ref value, min, max, ImU8String.op_Implicit(""), (ImGuiSliderFlags)16 /*0x10*/);
		var num = value * MathHelpers.Rad2Deg;
		if (!this.DrawSliderDrag(label, ref num, min, max, drag, true))
			return;
		value = num * MathHelpers.Deg2Rad;
	}

	private void DrawSliderFloat(
		string label,
		FontAwesomeIcon icon,
		ref float value,
		float min,
		float max,
		float drag,
		string hint = ""
	) {
		this.DrawSliderIcon(icon, hint);
		Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit(label), ref value, min, max, ImU8String.op_Implicit(""), (ImGuiSliderFlags)0);
		this.DrawSliderDrag(label, ref value, min, max, drag, false);
	}

	private void DrawSliderIcon(FontAwesomeIcon icon, string hint = "") {
		float spacing;
		this.DrawIconAlign(icon, out spacing, hint);
		Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, spacing);
		Dalamud.Bindings.ImGui.ImGui.SetNextItemWidth(Dalamud.Bindings.ImGui.ImGui.CalcItemWidth() - (Dalamud.Bindings.ImGui.ImGui.GetCursorPosX() - Dalamud.Bindings.ImGui.ImGui.GetCursorStartPos().X));
	}

	private bool DrawSliderDrag(
		string label,
		ref float value,
		float min,
		float max,
		float drag,
		bool angle
	) {
		ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, ((ImGuiStylePtr) ref style).ItemInnerSpacing.X);
		Dalamud.Bindings.ImGui.ImGui.SetNextItemWidth(Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail().X);
		ImU8String imU8String;
		// ISSUE: explicit constructor call
		((ImU8String) ref imU8String).\u002Ector(6, 1);
		((ImU8String) ref imU8String).AppendFormatted<string>(label);
		((ImU8String) ref imU8String).AppendLiteral("##Drag");
		return Dalamud.Bindings.ImGui.ImGui.DragFloat(imU8String, ref value, drag, min, max, ImU8String.op_Implicit(angle ? "%.0f°" : "%.3f"), (ImGuiSliderFlags)0);
	}

	private void DrawIconAlign(FontAwesomeIcon icon, out float spacing, string hint = "") {
		ImGuiStylePtr style1 = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		float x1 = ((ImGuiStylePtr) ref style1 ).CellPadding.X;
		ImFontPtr iconFont = UiBuilder.IconFont;
		var num1 = (float)(((double)((ImFontPtr) ref iconFont ).FontSize - (double)Icons.CalcIconSize(icon).X) / 2.0);
		Dalamud.Bindings.ImGui.ImGui.SetCursorPosX(Dalamud.Bindings.ImGui.ImGui.GetCursorPosX() + x1 + num1);
		Icons.DrawIcon(icon);
		if (!string.IsNullOrEmpty(hint) && Dalamud.Bindings.ImGui.ImGui.IsItemHovered()) {
			using (ImRaii.Tooltip())
				Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(hint));
		}
		ref var local = ref spacing;
		var num2 = x1 + (double)num1;
		ImGuiStylePtr style2 = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		var x2 = (double)((ImGuiStylePtr) ref style2 ).ItemInnerSpacing.X;
		var num3 = num2 + x2;
		local = (float)num3;
	}
}
