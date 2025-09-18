// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Windows.WorkspaceWindow
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using GLib.Widgets;
using Ktisis.Editor.Context.Types;
using Ktisis.Interface.Components.Workspace;
using Ktisis.Interface.Editor.Types;
using Ktisis.Interface.Types;
using System;
using System.Numerics;

#nullable enable
namespace Ktisis.Interface.Windows;

public class WorkspaceWindow : KtisisWindow
{
  private readonly IEditorContext _ctx;
  private readonly CameraSelector _cameras;
  private readonly WorkspaceState _workspace;
  private readonly SceneTree _sceneTree;
  private static readonly Vector2 MinimumSize = new Vector2(280f, 300f);

  private IEditorInterface Interface => this._ctx.Interface;

  public WorkspaceWindow(IEditorContext ctx)
    : base("KtisisPyon Workspace")
  {
    this._ctx = ctx;
    this._cameras = new CameraSelector(ctx);
    this._workspace = new WorkspaceState(ctx);
    this._sceneTree = new SceneTree(ctx);
  }

  public virtual void PreOpenCheck()
  {
    if (this._ctx.IsValid)
      return;
    Ktisis.Ktisis.Log.Verbose("Context for workspace window is stale, closing...", Array.Empty<object>());
    this.Close();
  }

  public virtual void PreDraw()
  {
    Window.WindowSizeConstraints windowSizeConstraints;
    // ISSUE: explicit constructor call
    ((Window.WindowSizeConstraints) ref windowSizeConstraints).\u002Ector();
    ((Window.WindowSizeConstraints) ref windowSizeConstraints).MinimumSize = WorkspaceWindow.MinimumSize;
    ref Window.WindowSizeConstraints local = ref windowSizeConstraints;
    ImGuiIOPtr io = Dalamud.Bindings.ImGui.ImGui.GetIO();
    Vector2 vector2 = ((ImGuiIOPtr) ref io).DisplaySize * 0.9f;
    ((Window.WindowSizeConstraints) ref local).MaximumSize = vector2;
    this.SizeConstraints = new Window.WindowSizeConstraints?(windowSizeConstraints);
  }

  public virtual void Draw()
  {
    ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
    this.DrawContextButtons();
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    this._cameras.Draw();
    this._workspace.Draw();
    ImFontPtr iconFont = UiBuilder.IconFont;
    float num = ((ImFontPtr) ref iconFont).FontSize + (float) (((double) ((ImGuiStylePtr) ref style).ItemSpacing.Y + (double) ((ImGuiStylePtr) ref style).ItemInnerSpacing.Y) * 2.0);
    this._sceneTree.Draw(Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail().Y - num);
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    this.DrawSceneTreeButtons();
  }

  private void DrawContextButtons()
  {
    ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
    float x = ((ImGuiStylePtr) ref style).ItemInnerSpacing.X;
    if (Buttons.IconButtonTooltip((FontAwesomeIcon) 61618, this._ctx.Locale.Translate("transform_edit.title")))
      this.Interface.OpenObjectEditor();
    Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x);
    if (Buttons.IconButtonTooltip((FontAwesomeIcon) 61829, this._ctx.Locale.Translate("env_edit.title")))
      this.Interface.OpenEnvironmentWindow();
    Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x);
    if (Buttons.IconButtonTooltip((FontAwesomeIcon) 62432, this._ctx.Locale.Translate("pose_view.title")))
      this.Interface.OpenPosingWindow();
    Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x);
    if (Buttons.IconButtonTooltip((FontAwesomeIcon) 61459, this._ctx.Locale.Translate("config.title")))
      this.Interface.OpenConfigWindow();
    Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x);
    Dalamud.Bindings.ImGui.ImGui.SetCursorPosX(Dalamud.Bindings.ImGui.ImGui.GetContentRegionMax().X - Buttons.CalcSize() * 2f - x);
    using (ImRaii.Disabled(!this._ctx.Actions.History.CanUndo))
    {
      if (Buttons.IconButtonTooltip((FontAwesomeIcon) 61512, this._ctx.Locale.Translate("actions.History_Undo")))
        this._ctx.Actions.History.Undo();
    }
    Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x);
    using (ImRaii.Disabled(!this._ctx.Actions.History.CanRedo))
    {
      if (!Buttons.IconButtonTooltip((FontAwesomeIcon) 61521, this._ctx.Locale.Translate("actions.History_Redo")))
        return;
      this._ctx.Actions.History.Redo();
    }
  }

  private void DrawSceneTreeButtons()
  {
    if (Buttons.IconButton((FontAwesomeIcon) 61543))
      this.Interface.OpenSceneCreateMenu();
    ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
    Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, ((ImGuiStylePtr) ref style).ItemInnerSpacing.X);
    if (!Buttons.IconButtonTooltip((FontAwesomeIcon) 61473, this._ctx.Locale.Translate("workspace.refresh_actors")))
      return;
    this.Interface.RefreshGposeActors();
  }
}
