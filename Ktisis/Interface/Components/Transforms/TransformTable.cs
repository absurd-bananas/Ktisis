// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Transforms.TransformTable
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using GLib.Widgets;

using Ktisis.Common.Utility;
using Ktisis.Core.Attributes;
using Ktisis.Data.Config;
using Ktisis.Data.Config.Sections;
using Ktisis.Editor.Selection;
using Ktisis.ImGuizmo;

namespace Ktisis.Interface.Components.Transforms;

[Transient]
public class TransformTable {
	private const Operation PositionOp = Operation.TRANSLATE;
	private const Operation RotateOp = Operation.ROTATE;
	private const Operation ScaleOp = Operation.SCALE | Operation.SCALE_U;
	private readonly static Vector3 MinScale = new Vector3(0.1f, 0.1f, 0.1f);
	private readonly static uint[] AxisColors = new uint[3] {
		4281684991U,
		4278243668U,
		4294923264U
	};
	private readonly ConfigManager _cfg;
	private readonly Transform Transform = new Transform();
	private Vector3 Angles = Vector3.Zero;
	private bool IsUsed;
	private Quaternion Value = Quaternion.Identity;

	public TransformTable(ConfigManager cfg) {
		this._cfg = cfg;
	}

	private GizmoConfig GizmoConfig => this._cfg.File.Gizmo;

	public bool IsActive { get; private set; }

	public bool IsDeactivated { get; private set; }

	public bool Draw(Transform transIn, out Transform transOut, TransformTableFlags flags = TransformTableFlags.Default) {
		ImU8String imU8String;
		// ISSUE: explicit constructor call
		((ImU8String) ref imU8String).\u002Ector(15, 1);
		((ImU8String) ref imU8String).AppendLiteral("TransformTable_");
		((ImU8String) ref imU8String).AppendFormatted<int>(this.GetHashCode(), "X");
		using (ImRaii.PushId(imU8String, true)) {
			if (!this.IsActive && !transIn.Rotation.Equals(this.Value)) {
				this.Angles = HkaEulerAngles.ToEuler(transIn.Rotation);
				this.Value = transIn.Rotation;
			}
			this.IsUsed = false;
			this.IsActive = false;
			this.IsDeactivated = false;
			try {
				Dalamud.Bindings.ImGui.ImGui.PushItemWidth(flags.HasFlag(TransformTableFlags.UseAvailable) ? CalcTableAvail() : CalcTableWidth());
				var op = flags.HasFlag(TransformTableFlags.Operation);
				transOut = this.Transform.Set(transIn);
				if (flags.HasFlag(TransformTableFlags.Position))
					this.DrawPosition(ref transOut.Position, op);
				if (flags.HasFlag(TransformTableFlags.Rotation))
					this.DrawRotate(ref transOut.Rotation, op);
				if (flags.HasFlag(TransformTableFlags.Scale)) {
					if (this.DrawScale(ref transOut.Scale, op))
						transOut.Scale = Vector3.Max(transOut.Scale, MinScale);
				}
			} finally {
				Dalamud.Bindings.ImGui.ImGui.PopItemWidth();
			}
			return this.IsUsed;
		}
	}

	public bool DrawPosition(ref Vector3 position, TransformTableFlags flags = TransformTableFlags.Default) {
		ImU8String imU8String;
		// ISSUE: explicit constructor call
		((ImU8String) ref imU8String).\u002Ector(15, 1);
		((ImU8String) ref imU8String).AppendLiteral("TransformTable_");
		((ImU8String) ref imU8String).AppendFormatted<int>(this.GetHashCode(), "X");
		using (ImRaii.PushId(imU8String, true)) {
			this.IsUsed = false;
			this.IsDeactivated = false;
			try {
				Dalamud.Bindings.ImGui.ImGui.PushItemWidth(flags.HasFlag(TransformTableFlags.UseAvailable) ? Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail().X : CalcTableWidth());
				var op = flags.HasFlag(TransformTableFlags.Operation);
				this.DrawPosition(ref position, op);
			} finally {
				Dalamud.Bindings.ImGui.ImGui.PopItemWidth();
			}
			return this.IsUsed;
		}
	}

	private bool DrawPosition(ref Vector3 pos, bool op) {
		var num = this.DrawLinear("##TransformTable_Pos", ref pos) ? 1 : 0;
		if (!op)
			return num != 0;
		this.DrawOperation(Operation.TRANSLATE, (FontAwesomeIcon)61732, "transform.position");
		return num != 0;
	}

	private bool DrawRotate(ref Quaternion rot, bool op) {
		var num = this.DrawEuler("##TransformTable_Rotate", ref this.Angles) ? 1 : 0;
		if (num != 0) {
			rot = HkaEulerAngles.ToQuaternion(this.Angles);
			this.Value = rot;
		}
		if (!op)
			return num != 0;
		this.DrawOperation(Operation.ROTATE, (FontAwesomeIcon)58555, "transform.rotation");
		return num != 0;
	}

	private bool DrawScale(ref Vector3 scale, bool op) {
		var num = this.DrawLinear("##TransformTable_Scale", ref scale) ? 1 : 0;
		if (!op)
			return num != 0;
		this.DrawOperation(Operation.SCALE | Operation.SCALE_U, (FontAwesomeIcon)61541, "transform.scale");
		return num != 0;
	}

	private void DrawOperation(Operation op, FontAwesomeIcon icon, string hint) {
		ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, ((ImGuiStylePtr) ref style).ItemSpacing.X);
		Dalamud.Bindings.ImGui.ImGui.PushStyleColor((ImGuiCol)0, this.GizmoConfig.Operation.HasFlag(op) ? uint.MaxValue : 2952790015U /*0xAFFFFFFF*/);
		if (Buttons.IconButtonTooltip(icon, hint))
			this.ChangeOperation(op);
		Dalamud.Bindings.ImGui.ImGui.PopStyleColor();
	}

	private void ChangeOperation(Operation op) {
		if (GuiHelpers.GetSelectMode() == SelectMode.Multiple)
			this.GizmoConfig.Operation |= op;
		else
			this.GizmoConfig.Operation = op;
	}

	private bool DrawLinear(string id, ref Vector3 vec) {
		var flag = this.DrawXYZ(id, ref vec, 1f / 1000f);
		this.IsUsed |= flag;
		return flag;
	}

	private bool DrawEuler(string id, ref Vector3 vec) {
		var flag = this.DrawXYZ(id, ref vec, 0.2f);
		if (flag)
			vec = vec.NormalizeAngles();
		this.IsUsed |= flag;
		return flag;
	}

	private bool DrawXYZ(string id, ref Vector3 vec, float speed) {
		ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		float x = ((ImGuiStylePtr) ref style ).ItemInnerSpacing.X;
		Dalamud.Bindings.ImGui.ImGui.PushItemWidth((float)(((double)Dalamud.Bindings.ImGui.ImGui.CalcItemWidth() - x * 2.0) / 3.0));
		var num1 = 0 | (this.DrawAxis(id + "_X", ref vec.X, speed, AxisColors[0]) ? 1 : 0);
		Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x);
		var num2 = this.DrawAxis(id + "_Y", ref vec.Y, speed, AxisColors[1]) ? 1 : 0;
		var num3 = num1 | num2;
		Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x);
		var num4 = this.DrawAxis(id + "_Z", ref vec.Z, speed, AxisColors[2]) ? 1 : 0;
		var num5 = num3 | num4;
		Dalamud.Bindings.ImGui.ImGui.PopItemWidth();
		return num5 != 0;
	}

	private bool DrawAxis(string id, ref float value, float speed, uint col) {
		ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		Dalamud.Bindings.ImGui.ImGui.PushStyleVar((ImGuiStyleVar)10, ((ImGuiStylePtr) ref style).FramePadding + new Vector2(0.1f, 0.1f));
		Dalamud.Bindings.ImGui.ImGui.PushStyleVar((ImGuiStyleVar)12, 0.1f);
		Dalamud.Bindings.ImGui.ImGui.PushStyleColor((ImGuiCol)5, col);
		var num = Dalamud.Bindings.ImGui.ImGui.DragFloat(ImU8String.op_Implicit(id), ref value, speed, 0.0f, 0.0f, ImU8String.op_Implicit("%.3f"), (ImGuiSliderFlags)64 /*0x40*/) ? 1 : 0;
		Dalamud.Bindings.ImGui.ImGui.PopStyleColor();
		Dalamud.Bindings.ImGui.ImGui.PopStyleVar(2);
		this.IsActive |= Dalamud.Bindings.ImGui.ImGui.IsItemActive();
		this.IsDeactivated |= Dalamud.Bindings.ImGui.ImGui.IsItemDeactivatedAfterEdit();
		return num != 0;
	}

	private static float CalcTableAvail() => Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail().X - CalcIconSpacing();

	private static float CalcTableWidth() {
		ImFontPtr defaultFont = UiBuilder.DefaultFont;
		return (float)((double)((ImFontPtr) ref defaultFont ).FontSize * 4.0 * 3.0);
	}

	private static float CalcIconSpacing() {
		ImFontPtr iconFont = UiBuilder.IconFont;
		var num1 = (double)((ImFontPtr) ref iconFont ).FontSize;
		ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		var num2 = (double)((ImGuiStylePtr) ref style ).ItemSpacing.X * 2.0;
		return (float)(num1 + num2);
	}

	public static float CalcWidth() => CalcTableWidth() + CalcIconSpacing();
}
