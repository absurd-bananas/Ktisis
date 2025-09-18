// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Windows.TransformWindowOld
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Common.Math;
using GLib.Widgets;
using Ktisis.Data.Config.Sections;
using Ktisis.Editor.Context.Types;
using Ktisis.Editor.Posing.Ik.TwoJoints;
using Ktisis.Editor.Transforms.Types;
using Ktisis.ImGuizmo;
using Ktisis.Interface.Components.Transforms;
using Ktisis.Interface.Types;
using Ktisis.Scene.Decor.Ik;
using Ktisis.Scene.Entities;
using Ktisis.Scene.Entities.Game;
using Ktisis.Scene.Entities.Skeleton;
using Ktisis.Services.Game;
using System;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Ktisis.Interface.Windows;

public class TransformWindowOld : KtisisWindow
{
  private readonly IEditorContext _ctx;
  private readonly Gizmo2D _gizmo;
  private readonly TransformTable _table;
  private ITransformMemento? Transform;

  public TransformWindowOld(IEditorContext ctx, Gizmo2D gizmo, TransformTable table)
    : base("Transform Editor", (ImGuiWindowFlags) 66)
  {
    this._ctx = ctx;
    this._gizmo = gizmo;
    this._table = table;
  }

  public virtual void PreOpenCheck()
  {
    if (this._ctx.IsValid)
      return;
    Ktisis.Ktisis.Log.Verbose("Context for transform window is stale, closing...", Array.Empty<object>());
    this.Close();
  }

  public virtual void PreDraw()
  {
    double num1 = (double) TransformTable.CalcWidth();
    ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
    double num2 = (double) ((ImGuiStylePtr) ref style).WindowPadding.X * 2.0;
    float x = (float) (num1 + num2);
    Window.WindowSizeConstraints windowSizeConstraints;
    // ISSUE: explicit constructor call
    ((Window.WindowSizeConstraints) ref windowSizeConstraints).\u002Ector();
    ((Window.WindowSizeConstraints) ref windowSizeConstraints).MaximumSize = new Vector2(x, -1f);
    this.SizeConstraints = new Window.WindowSizeConstraints?(windowSizeConstraints);
  }

  public virtual void Draw()
  {
    this.DrawToggles();
    ITransformTarget target = this._ctx.Transform.Target;
    Ktisis.Common.Utility.Transform transform = target?.GetTransform() ?? new Ktisis.Common.Utility.Transform();
    bool disabled = target == null;
    using (ImRaii.Disabled(disabled))
    {
      bool isEnded;
      bool flag = this.DrawTransform(ref transform, out isEnded, disabled);
      if (target != null & flag)
      {
        if (this.Transform == null)
          this.Transform = this._ctx.Transform.Begin(target);
        this.Transform.SetTransform(transform);
      }
      if (isEnded)
      {
        this.Transform?.Dispatch();
        this.Transform = (ITransformMemento) null;
      }
      if (target != null && target.Targets.Any<SceneEntity>((Func<SceneEntity, bool>) (tar => tar is ActorEntity)))
        this.DrawActorControls();
      if (target?.Primary is SkeletonNode)
        this.DrawBoneTransformSetup();
      if (target != null && target.Targets.Any<SceneEntity>((Func<SceneEntity, bool>) (tar => tar is ActorEntity)))
        this.DrawFlipToolOptions();
      if (!(target?.Primary is IIkNode primary))
        return;
      this.DrawIkSetup(primary);
    }
  }

  private bool DrawTransform(ref Ktisis.Common.Utility.Transform transform, out bool isEnded, bool disabled)
  {
    isEnded = false;
    bool flag1 = false;
    if (!this._ctx.Config.Editor.TransformHide)
    {
      flag1 = this.DrawGizmo(ref transform, TransformTable.CalcWidth(), disabled);
      isEnded = this._gizmo.IsEnded;
    }
    Ktisis.Common.Utility.Transform transOut;
    bool flag2 = this._table.Draw(transform, out transOut);
    if (flag2)
      transform = transOut;
    isEnded |= this._table.IsDeactivated;
    return flag1 | flag2;
  }

  private void DrawToggles()
  {
    ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
    float x1 = ((ImGuiStylePtr) ref style).ItemInnerSpacing.X;
    ImFontPtr iconFont = UiBuilder.IconFont;
    float num1 = ((ImFontPtr) ref iconFont).FontSize * 2f;
    Vector2 vector2 = new Vector2(num1, num1);
    Mode mode = this._ctx.Config.Gizmo.Mode;
    if (Buttons.IconButtonTooltip(mode == Mode.World ? (FontAwesomeIcon) 61612 : (FontAwesomeIcon) 61461, this._ctx.Locale.Translate("transform_edit.mode." + (mode == Mode.World ? "world" : "local")), new Vector2?(vector2)))
      this._ctx.Config.Gizmo.Mode = mode == Mode.World ? Mode.Local : Mode.World;
    Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x1);
    bool visible = this._ctx.Config.Gizmo.Visible;
    if (Buttons.IconButtonTooltip(visible ? (FontAwesomeIcon) 61550 : (FontAwesomeIcon) 61552, this._ctx.Locale.Translate("actions.Gizmo_Toggle"), new Vector2?(vector2)))
      this._ctx.Config.Gizmo.Visible = !visible;
    Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x1);
    int num2 = this._ctx.Config.Gizmo.MirrorRotation ? 1 : 0;
    if (Buttons.IconButtonTooltip(num2 != 0 ? (FontAwesomeIcon) 58543 : (FontAwesomeIcon) 63396, this._ctx.Locale.Translate("transform_edit.flags." + (num2 != 0 ? "mirror" : "parallel")), new Vector2?(vector2)))
    {
      GizmoConfig gizmo = this._ctx.Config.Gizmo;
      gizmo.MirrorRotation = !gizmo.MirrorRotation;
    }
    Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x1);
    ActorEntity actorFromTarget = this._ctx.Posing.GetActorFromTarget(this._ctx.Transform.Target);
    if (actorFromTarget != null)
    {
      if (Buttons.IconButtonTooltip((FontAwesomeIcon) 58561, "Flip Pose", new Vector2?(vector2)))
        this._ctx.Posing.ApplyPoseFlip(actorFromTarget);
      Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x1);
    }
    float x2 = Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail().X;
    if ((double) x2 > (double) num1)
      Dalamud.Bindings.ImGui.ImGui.SetCursorPosX(Dalamud.Bindings.ImGui.ImGui.GetCursorPosX() + x2 - num1);
    bool transformHide = this._ctx.Config.Editor.TransformHide;
    if (!Buttons.IconButtonTooltip(transformHide ? (FontAwesomeIcon) 61656 : (FontAwesomeIcon) 61655, this._ctx.Locale.Translate("transform_edit.gizmo." + (transformHide ? "show" : "hide")), new Vector2?(vector2)))
      return;
    this._ctx.Config.Editor.TransformHide = !transformHide;
  }

  private unsafe bool DrawGizmo(ref Ktisis.Common.Utility.Transform transform, float width, bool disabled)
  {
    Vector2 cursorScreenPos = Dalamud.Bindings.ImGui.ImGui.GetCursorScreenPos();
    Vector2 vector2 = new Vector2(width, width);
    this._gizmo.Begin(vector2);
    this._gizmo.Mode = this._ctx.Config.Gizmo.Mode;
    this.DrawGizmoCircle(cursorScreenPos, vector2, width);
    if (disabled)
    {
      this._gizmo.End();
      return false;
    }
    Camera* gameCamera = CameraService.GetGameCamera();
    float fov = (IntPtr) gameCamera != IntPtr.Zero ? gameCamera->FoV : 1f;
    Vector3 cameraPos = (IntPtr) gameCamera != IntPtr.Zero ? Vector3.op_Implicit(gameCamera->CameraBase.SceneCamera.Object.Position) : Vector3.Zero;
    Matrix4x4 matrix = transform.ComposeMatrix();
    this._gizmo.SetLookAt(cameraPos, matrix.Translation, fov);
    int num = this._gizmo.Manipulate(ref matrix, out Matrix4x4 _) ? 1 : 0;
    this._gizmo.End();
    if (num == 0)
      return num != 0;
    transform.DecomposeMatrix(matrix);
    return num != 0;
  }

  private void DrawGizmoCircle(Vector2 pos, Vector2 size, float width)
  {
    ImDrawListPtr windowDrawList = Dalamud.Bindings.ImGui.ImGui.GetWindowDrawList();
    ((ImDrawListPtr) ref windowDrawList).AddCircleFilled(pos + size / 2f, (float) ((double) width * 0.5 / 2.0499999523162842), 3474989088U);
  }

  private void DrawBoneTransformSetup()
  {
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    if (!Dalamud.Bindings.ImGui.ImGui.CollapsingHeader(ImU8String.op_Implicit(this._ctx.Locale.Translate("transform_edit.transforms.title")), (ImGuiTreeNodeFlags) 0))
      return;
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    GizmoConfig gizmo = this._ctx.Config.Gizmo;
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this._ctx.Locale.Translate("transform_edit.transforms.parenting")), ref gizmo.ParentBones);
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this._ctx.Locale.Translate("transform_edit.transforms.relative")), ref gizmo.RelativeBones);
  }

  private void DrawFlipToolOptions()
  {
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    if (!Dalamud.Bindings.ImGui.ImGui.CollapsingHeader(ImU8String.op_Implicit("Flip Pose Options"), (ImGuiTreeNodeFlags) 0))
      return;
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    EditorConfig editor = this._ctx.Config.Editor;
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Flip Yaw Correction"), ref editor.FlipYawCorrection);
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Flip Rotation Correction"), ref editor.FlipRotationCorrection);
  }

  private void DrawActorControls()
  {
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    if (!Dalamud.Bindings.ImGui.ImGui.CollapsingHeader(ImU8String.op_Implicit(this._ctx.Locale.Translate("transform_edit.actors.title")), (ImGuiTreeNodeFlags) 0))
      return;
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    bool positionLockEnabled = this._ctx.Animation.PositionLockEnabled;
    if (Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this._ctx.Locale.Translate("actors.pos_lock")), ref positionLockEnabled))
      this._ctx.Animation.PositionLockEnabled = positionLockEnabled;
    Dalamud.Bindings.ImGui.ImGui.Spacing();
  }

  private void DrawIkSetup(IIkNode ik)
  {
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    if (!Dalamud.Bindings.ImGui.ImGui.CollapsingHeader(ImU8String.op_Implicit(this._ctx.Locale.Translate("transform_edit.ik.title")), (ImGuiTreeNodeFlags) 0))
      return;
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    bool isEnabled = ik.IsEnabled;
    if (Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this._ctx.Locale.Translate("transform_edit.ik.enable")), ref isEnabled))
      ik.Toggle();
    if (!ik.IsEnabled)
      return;
    switch (ik)
    {
      case ITwoJointsNode ik1:
        this.DrawTwoJoints(ik1);
        break;
      case ICcdNode ik2:
        this.DrawCcd(ik2);
        break;
    }
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    if (Buttons.IconButton((FontAwesomeIcon) 61761))
      Dalamud.Bindings.ImGui.ImGui.OpenPopup(ImU8String.op_Implicit("##IkAdvancedCfg"), (ImGuiPopupFlags) 0);
    ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
    Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, ((ImGuiStylePtr) ref style).ItemInnerSpacing.X);
    Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(this._ctx.Locale.Translate("transform_edit.ik.advanced")));
  }

  private void DrawCcd(ICcdNode ik)
  {
    if (!Dalamud.Bindings.ImGui.ImGui.IsPopupOpen(ImU8String.op_Implicit("##IkAdvancedCfg"), (ImGuiPopupFlags) 0))
      return;
    this.DrawCcdAdvanced(ik);
  }

  private void DrawCcdAdvanced(ICcdNode ik)
  {
    using (ImRaii.IEndObject iendObject = ImRaii.Popup(ImU8String.op_Implicit("##IkAdvancedCfg")))
    {
      if (!iendObject.Success)
        return;
      Dalamud.Bindings.ImGui.ImGui.Spacing();
      Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit(this._ctx.Locale.Translate("transform_edit.ik.ccd.gain")), ref ik.Group.Gain, 0.0f, 1f, ImU8String.op_Implicit("%.2f"), (ImGuiSliderFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.SliderInt(ImU8String.op_Implicit(this._ctx.Locale.Translate("transform_edit.ik.ccd.iterations")), ref ik.Group.Iterations, 0, 60, new ImU8String(), (ImGuiSliderFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.Spacing();
    }
  }

  private void DrawTwoJoints(ITwoJointsNode ik)
  {
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this._ctx.Locale.Translate("transform_edit.ik.two_joints.enforce")), ref ik.Group.EnforceRotation);
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(this._ctx.Locale.Translate("transform_edit.ik.two_joints.mode")));
    TransformWindowOld.DrawMode(this._ctx.Locale.Translate("transform_edit.ik.two_joints.fixed"), TwoJointsMode.Fixed, ik.Group);
    TransformWindowOld.DrawMode(this._ctx.Locale.Translate("transform_edit.ik.two_joints.relative"), TwoJointsMode.Relative, ik.Group);
    if (!Dalamud.Bindings.ImGui.ImGui.IsPopupOpen(ImU8String.op_Implicit("##IkAdvancedCfg"), (ImGuiPopupFlags) 0))
      return;
    this.DrawTwoJointsAdvanced(ik);
  }

  private void DrawTwoJointsAdvanced(ITwoJointsNode ik)
  {
    using (ImRaii.IEndObject iendObject = ImRaii.Popup(ImU8String.op_Implicit("##IkAdvancedCfg")))
    {
      if (!iendObject.Success)
        return;
      Dalamud.Bindings.ImGui.ImGui.Spacing();
      Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(this._ctx.Locale.Translate("transform_edit.ik.two_joints.gain")));
      Dalamud.Bindings.ImGui.ImGui.Spacing();
      Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Shoulder##FirstWeight"), ref ik.Group.FirstBoneGain, 0.0f, 1f, ImU8String.op_Implicit("%.2f"), (ImGuiSliderFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Elbow##SecondWeight"), ref ik.Group.SecondBoneGain, 0.0f, 1f, ImU8String.op_Implicit("%.2f"), (ImGuiSliderFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Hand##HandWeight"), ref ik.Group.EndBoneGain, 0.0f, 1f, ImU8String.op_Implicit("%.2f"), (ImGuiSliderFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.Spacing();
      Dalamud.Bindings.ImGui.ImGui.Separator();
      Dalamud.Bindings.ImGui.ImGui.Spacing();
      Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(this._ctx.Locale.Translate("transform_edit.ik.two_joints.hinges")));
      Dalamud.Bindings.ImGui.ImGui.Spacing();
      Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Minimum"), ref ik.Group.MinHingeAngle, -1f, 1f, ImU8String.op_Implicit("%.2f"), (ImGuiSliderFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Maximum"), ref ik.Group.MaxHingeAngle, -1f, 1f, ImU8String.op_Implicit("%.2f"), (ImGuiSliderFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.SliderFloat3(ImU8String.op_Implicit("Axis"), ref ik.Group.HingeAxis, -1f, 1f, ImU8String.op_Implicit("%.2f"), (ImGuiSliderFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.Spacing();
    }
  }

  private static void DrawMode(string label, TwoJointsMode mode, TwoJointsGroup group)
  {
    bool flag = group.Mode == mode;
    if (!Dalamud.Bindings.ImGui.ImGui.RadioButton(ImU8String.op_Implicit(label), flag))
      return;
    group.Mode = mode;
  }
}
