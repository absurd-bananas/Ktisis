// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Windows.Editors.ActorWindow
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Ktisis.Editor.Animation.Types;
using Ktisis.Editor.Characters.State;
using Ktisis.Editor.Characters.Types;
using Ktisis.Editor.Context.Types;
using Ktisis.Interface.Components.Chara;
using Ktisis.Interface.Types;
using Ktisis.Scene.Entities.Game;
using Ktisis.Structs.Actors;
using Ktisis.Structs.Characters;
using System;
using System.Numerics;

#nullable enable
namespace Ktisis.Interface.Windows.Editors;

public class ActorWindow : EntityEditWindow<ActorEntity>
{
  private const string WindowId = "KtisisActorEditor";
  private readonly CustomizeEditorTab _custom;
  private readonly EquipmentEditorTab _equip;
  private readonly AnimationEditorTab _anim;
  private ICustomizeEditor _editCustom;

  private IAnimationManager Animation => this.Context.Animation;

  private ICharacterManager Manager => this.Context.Characters;

  public ActorWindow(
    IEditorContext ctx,
    CustomizeEditorTab custom,
    EquipmentEditorTab equip,
    AnimationEditorTab anim)
    : base("Actor Editor###KtisisActorEditor", ctx)
  {
    this._custom = custom;
    this._equip = equip;
    this._anim = anim;
  }

  public override void SetTarget(ActorEntity target)
  {
    this.WindowName = target.Name + "###KtisisActorEditor";
    base.SetTarget(target);
    this._editCustom = this._custom.Editor = this.Manager.GetCustomizeEditor(target);
    this._equip.Editor = this.Manager.GetEquipmentEditor(target);
    this._anim.Editor = this.Animation.GetAnimationEditor(target);
  }

  public virtual void OnOpen()
  {
    this._custom.Setup();
    this._anim.Setup();
  }

  public override void PreDraw()
  {
    base.PreDraw();
    Window.WindowSizeConstraints windowSizeConstraints;
    // ISSUE: explicit constructor call
    ((Window.WindowSizeConstraints) ref windowSizeConstraints).\u002Ector();
    ((Window.WindowSizeConstraints) ref windowSizeConstraints).MinimumSize = new Vector2(560f, 380f);
    ref Window.WindowSizeConstraints local = ref windowSizeConstraints;
    ImGuiIOPtr io = Dalamud.Bindings.ImGui.ImGui.GetIO();
    Vector2 vector2 = ((ImGuiIOPtr) ref io).DisplaySize * 0.9f;
    ((Window.WindowSizeConstraints) ref local).MaximumSize = vector2;
    this.SizeConstraints = new Window.WindowSizeConstraints?(windowSizeConstraints);
  }

  public virtual void Draw()
  {
    this.UpdateTarget();
    using (ImRaii.TabBar(ImU8String.op_Implicit("##ActorEditTabs")))
    {
      ActorWindow.DrawTab("Appearance", new Action(this._custom.Draw));
      ActorWindow.DrawTab("Equipment", new Action(this._equip.Draw));
      ActorWindow.DrawTab("Animation", new Action(this._anim.Draw));
      ActorWindow.DrawTab("Misc", new Action(this.DrawMisc));
    }
  }

  private static void DrawTab(string name, Action draw)
  {
    using (ImRaii.IEndObject iendObject = ImRaii.TabItem(ImU8String.op_Implicit(name)))
    {
      if (!iendObject.Success)
        return;
      draw();
    }
  }

  private unsafe void DrawMisc()
  {
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    int modelId = (int) this._editCustom.GetModelId();
    if (Dalamud.Bindings.ImGui.ImGui.InputInt(ImU8String.op_Implicit("Model ID"), ref modelId, 0, 0, new ImU8String(), (ImGuiInputTextFlags) 0))
      this._editCustom.SetModelId((uint) modelId);
    CharacterEx* character = (CharacterEx*) this.Target.Character;
    if ((IntPtr) character != IntPtr.Zero)
    {
      Dalamud.Bindings.ImGui.ImGui.Spacing();
      Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Opacity"), ref character->Opacity, 0.0f, 1f, new ImU8String(), (ImGuiSliderFlags) 0);
    }
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    this.DrawWetness();
  }

  private void DrawWetness()
  {
    bool hasValue = this.Target.Appearance.Wetness.HasValue;
    if (Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Wetness Override"), ref hasValue))
      this.ToggleWetness();
    WetnessState? wetness = this.GetWetness();
    if (!wetness.HasValue)
      return;
    using (ImRaii.Disabled(!hasValue))
    {
      Dalamud.Bindings.ImGui.ImGui.Spacing();
      WetnessState wetnessState = wetness.Value;
      if ((0 | (Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Weather Wetness"), ref wetnessState.WeatherWetness, 0.0f, 1f, new ImU8String(), (ImGuiSliderFlags) 0) ? 1 : 0) | (Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Swimming Wetness"), ref wetnessState.SwimmingWetness, 0.0f, 1f, new ImU8String(), (ImGuiSliderFlags) 0) ? 1 : 0) | (Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Wetness Depth"), ref wetnessState.WetnessDepth, 0.0f, 3f, new ImU8String(), (ImGuiSliderFlags) 0) ? 1 : 0)) == 0)
        return;
      this.Target.Appearance.Wetness = new WetnessState?(wetnessState);
    }
  }

  private unsafe WetnessState? GetWetness()
  {
    WetnessState? wetness = this.Target.Appearance.Wetness;
    if (wetness.HasValue)
      return new WetnessState?(wetness.GetValueOrDefault());
    CharacterBaseEx* characterBaseEx = this.Target.CharacterBaseEx;
    return (IntPtr) characterBaseEx == IntPtr.Zero ? new WetnessState?() : new WetnessState?(characterBaseEx->Wetness);
  }

  private unsafe void ToggleWetness()
  {
    AppearanceState appearance = this.Target.Appearance;
    if (appearance.Wetness.HasValue)
    {
      appearance.Wetness = new WetnessState?();
    }
    else
    {
      CharacterBaseEx* characterBaseEx = this.Target.CharacterBaseEx;
      appearance.Wetness = (IntPtr) characterBaseEx != IntPtr.Zero ? new WetnessState?(characterBaseEx->Wetness) : new WetnessState?();
    }
  }
}
