// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Editor.Properties.PosePropertyList
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using GLib.Widgets;
using Ktisis.Common.Utility;
using Ktisis.Editor.Context.Types;
using Ktisis.Editor.Posing.Ik.TwoJoints;
using Ktisis.Editor.Posing.Ik.Types;
using Ktisis.Interface.Editor.Properties.Types;
using Ktisis.Localization;
using Ktisis.Scene.Decor.Ik;
using Ktisis.Scene.Entities;
using Ktisis.Scene.Entities.Game;
using Ktisis.Scene.Entities.Skeleton;
using Ktisis.Scene.Entities.Skeleton.Constraints;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Ktisis.Interface.Editor.Properties;

public class PosePropertyList : ObjectPropertyList
{
  private readonly IEditorContext _ctx;
  private readonly LocaleManager _locale;
  private const string IkCfgPopup = "##IkCfgPopup";

  public PosePropertyList(IEditorContext ctx, LocaleManager locale)
  {
    this._ctx = ctx;
    this._locale = locale;
  }

  public override void Invoke(IPropertyListBuilder builder, SceneEntity entity)
  {
    EntityPose pose;
    if (!PosePropertyList.TryGetEntityPose(entity, out pose))
      return;
    builder.AddHeader("Pose", (Action) (() => this.DrawPoseTab(pose)), 1);
    if (pose.IkController.GroupCount <= 0)
      return;
    builder.AddHeader("Inverse Kinematics", (Action) (() => this.DrawConstraintsTab(pose)), 2);
  }

  private void DrawPoseTab(EntityPose pose)
  {
    ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
    float x = ((ImGuiStylePtr) ref style).ItemInnerSpacing.X;
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this._locale.Translate("transform_edit.transforms.parenting")), ref this._ctx.Config.Gizmo.ParentBones);
    if (!(pose.Parent is ActorEntity parent))
      return;
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Separator();
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Flip Yaw Correction"), ref this._ctx.Config.Editor.FlipYawCorrection);
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Flip Rotation Correction"), ref this._ctx.Config.Editor.FlipRotationCorrection);
    if (Dalamud.Bindings.ImGui.ImGui.Button(ImU8String.op_Implicit("Flip Pose"), new Vector2()))
      this._ctx.Posing.ApplyPoseFlip(parent);
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Separator();
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    if (Dalamud.Bindings.ImGui.ImGui.Button(ImU8String.op_Implicit("Import"), new Vector2()))
      this._ctx.Interface.OpenPoseImport(parent);
    Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x);
    if (!Dalamud.Bindings.ImGui.ImGui.Button(ImU8String.op_Implicit("Export"), new Vector2()))
      return;
    this._ctx.Interface.OpenPoseExport(pose);
  }

  private void DrawConstraintsTab(EntityPose pose)
  {
    ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
    float x = ((ImGuiStylePtr) ref style).ItemInnerSpacing.X;
    foreach ((string name, IIkGroup group) in pose.IkController.GetGroups())
    {
      IkEndNode node;
      if (PosePropertyList.TryGetGroupEndNode(pose, group, out node))
      {
        ImU8String imU8String = new ImU8String(7, 1);
        ((ImU8String) ref imU8String).AppendLiteral("IkProp_");
        ((ImU8String) ref imU8String).AppendFormatted<string>(name);
        using (ImRaii.PushId(imU8String, true))
        {
          bool isEnabled = group.IsEnabled;
          if (Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(" " + this._locale.Translate("boneCategory." + name)), ref isEnabled))
            group.IsEnabled = isEnabled;
          float num = (float) ((double) Icons.CalcIconSize((FontAwesomeIcon) 62042).X + (double) Icons.CalcIconSize((FontAwesomeIcon) 61761).X + (double) x * 3.0);
          Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x);
          Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail().X - num);
          using (ImRaii.PushColor((ImGuiCol) 21, Dalamud.Bindings.ImGui.ImGui.GetColorU32((ImGuiCol) 23), node.IsSelected))
          {
            bool flag = !node.IsSelected || this._ctx.Selection.Count > 1;
            if (Buttons.IconButtonTooltip((FontAwesomeIcon) 62042, "Select", new Vector2?(Vector2.Zero)) & flag)
              node.Select(GuiHelpers.GetSelectMode());
          }
          Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x);
          if (Buttons.IconButtonTooltip((FontAwesomeIcon) 61761, "Configure", new Vector2?(Vector2.Zero)))
            Dalamud.Bindings.ImGui.ImGui.OpenPopup(ImU8String.op_Implicit("##IkCfgPopup"), (ImGuiPopupFlags) 0);
          if (Dalamud.Bindings.ImGui.ImGui.IsPopupOpen(ImU8String.op_Implicit("##IkCfgPopup"), (ImGuiPopupFlags) 0))
          {
            using (ImRaii.IEndObject iendObject = ImRaii.Popup(ImU8String.op_Implicit("##IkCfgPopup")))
            {
              if (iendObject.Success)
                this.DrawIkConfig((IIkNode) node);
            }
          }
        }
      }
    }
  }

  private void DrawIkConfig(IIkNode ik)
  {
    bool isEnabled = ik.IsEnabled;
    if (Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Enabled"), ref isEnabled))
    {
      if (isEnabled)
        ik.Enable();
      else
        ik.Disable();
    }
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Separator();
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    switch (ik)
    {
      case ICcdNode node1:
        this.DrawCcd(node1);
        break;
      case ITwoJointsNode node2:
        this.DrawTwoJoints(node2);
        break;
    }
  }

  private void DrawCcd(ICcdNode node)
  {
    Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit(this._locale.Translate("transform_edit.ik.ccd.gain")), ref node.Group.Gain, 0.0f, 1f, ImU8String.op_Implicit("%.2f"), (ImGuiSliderFlags) 0);
    Dalamud.Bindings.ImGui.ImGui.SliderInt(ImU8String.op_Implicit(this._locale.Translate("transform_edit.ik.ccd.iterations")), ref node.Group.Iterations, 0, 60, new ImU8String(), (ImGuiSliderFlags) 0);
  }

  private void DrawTwoJoints(ITwoJointsNode node)
  {
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this._locale.Translate("transform_edit.ik.two_joints.enforce")), ref node.Group.EnforceRotation);
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(this._locale.Translate("transform_edit.ik.two_joints.mode")));
    PosePropertyList.DrawIkMode(this._locale.Translate("transform_edit.ik.two_joints.fixed"), TwoJointsMode.Fixed, node.Group);
    PosePropertyList.DrawIkMode(this._locale.Translate("transform_edit.ik.two_joints.relative"), TwoJointsMode.Relative, node.Group);
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Separator();
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(this._locale.Translate("transform_edit.ik.two_joints.gain")));
    Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Shoulder##FirstWeight"), ref node.Group.FirstBoneGain, 0.0f, 1f, ImU8String.op_Implicit("%.2f"), (ImGuiSliderFlags) 0);
    Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Elbow##SecondWeight"), ref node.Group.SecondBoneGain, 0.0f, 1f, ImU8String.op_Implicit("%.2f"), (ImGuiSliderFlags) 0);
    Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Hand##HandWeight"), ref node.Group.EndBoneGain, 0.0f, 1f, ImU8String.op_Implicit("%.2f"), (ImGuiSliderFlags) 0);
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Separator();
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(this._locale.Translate("transform_edit.ik.two_joints.hinges")));
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Minimum"), ref node.Group.MinHingeAngle, -1f, 1f, ImU8String.op_Implicit("%.2f"), (ImGuiSliderFlags) 0);
    Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Maximum"), ref node.Group.MaxHingeAngle, -1f, 1f, ImU8String.op_Implicit("%.2f"), (ImGuiSliderFlags) 0);
    Dalamud.Bindings.ImGui.ImGui.SliderFloat3(ImU8String.op_Implicit("Axis"), ref node.Group.HingeAxis, -1f, 1f, ImU8String.op_Implicit("%.2f"), (ImGuiSliderFlags) 0);
    Dalamud.Bindings.ImGui.ImGui.Spacing();
  }

  private static void DrawIkMode(string label, TwoJointsMode mode, TwoJointsGroup group)
  {
    bool flag = group.Mode == mode;
    if (!Dalamud.Bindings.ImGui.ImGui.RadioButton(ImU8String.op_Implicit(label), flag))
      return;
    group.Mode = mode;
  }

  private static bool TryGetEntityPose(SceneEntity entity, [NotNullWhen(true)] out EntityPose? result)
  {
    EntityPose entityPose1;
    switch (entity)
    {
      case ActorEntity actorEntity:
        entityPose1 = actorEntity.Pose;
        break;
      case BoneNode boneNode:
        entityPose1 = boneNode.Pose;
        break;
      case EntityPose entityPose2:
        entityPose1 = entityPose2;
        break;
      default:
        entityPose1 = (EntityPose) null;
        break;
    }
    result = entityPose1;
    return result != null;
  }

  private static bool TryGetGroupEndNode(EntityPose pose, IIkGroup group, [NotNullWhen(true)] out IkEndNode? node1)
  {
    node1 = pose.Recurse().FirstOrDefault<SceneEntity>((Func<SceneEntity, bool>) (node2 => node2 is IkEndNode && node2.Parent is IkNodeGroupBase parent && parent.Group == group)) as IkEndNode;
    return node1 != null;
  }
}
