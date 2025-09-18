// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Editor.EditorInterface
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Plugin;
using GLib.Popups.Context;
using GLib.Popups.ImFileDialog;
using Ktisis.Data.Config;
using Ktisis.Data.Files;
using Ktisis.Editor.Context.Types;
using Ktisis.Editor.Selection;
using Ktisis.Interface.Components.Transforms;
using Ktisis.Interface.Editor.Context;
using Ktisis.Interface.Editor.Popup;
using Ktisis.Interface.Editor.Types;
using Ktisis.Interface.Overlay;
using Ktisis.Interface.Types;
using Ktisis.Interface.Windows;
using Ktisis.Interface.Windows.Editors;
using Ktisis.Interface.Windows.Import;
using Ktisis.Scene;
using Ktisis.Scene.Entities;
using Ktisis.Scene.Entities.Game;
using Ktisis.Scene.Entities.Skeleton;
using Ktisis.Scene.Entities.World;
using Ktisis.Scene.Modules;
using Ktisis.Scene.Modules.Actors;
using Ktisis.Scene.Types;
using System;
using System.Threading.Tasks;

#nullable enable
namespace Ktisis.Interface.Editor;

public class EditorInterface : IEditorInterface
{
  private readonly IDalamudPluginInterface _dpi;
  private readonly IEditorContext _ctx;
  private readonly GuiManager _gui;
  private readonly GizmoManager _gizmo;
  private static readonly FileDialogOptions CharaFileOptions = new FileDialogOptions()
  {
    Filters = "Character Files{.chara}",
    Extension = ".chara"
  };
  private static readonly FileDialogOptions PoseFileOptions = new FileDialogOptions()
  {
    Filters = "Pose Files{.pose}",
    Extension = ".pose"
  };
  private static readonly FileDialogOptions McdfFileOptions = new FileDialogOptions()
  {
    Filters = "MCDF Files{.mcdf}",
    Extension = ".mcdf"
  };
  private static readonly FileDialogOptions LightFileOptions = new FileDialogOptions()
  {
    Filters = "Light Files{.light}",
    Extension = ".light"
  };

  public EditorInterface(IDalamudPluginInterface dpi, IEditorContext ctx, GuiManager gui)
  {
    this._dpi = dpi;
    this._ctx = ctx;
    this._gui = gui;
    this._gizmo = new GizmoManager(this._dpi, ctx.Config);
  }

  public void Prepare()
  {
    if (this._ctx.Config.Editor.OpenOnEnterGPose)
      this._gui.GetOrCreate<WorkspaceWindow>((object) this._ctx).Open();
    this._ctx.Selection.Changed += new SelectChangedHandler(this.OnSelectChanged);
    this._gizmo.Initialize();
    this._gui.GetOrCreate<OverlayWindow>((object) this._ctx, (object) this._gizmo.Create(GizmoId.OverlayMain)).Open();
  }

  private void OnSelectChanged(ISelectManager sender)
  {
    if (!this._ctx.Config.Editor.ToggleEditorOnSelect)
      return;
    bool flag = sender.Count > 0;
    ObjectWindow objectWindow = this._gui.Get<ObjectWindow>();
    if (objectWindow == null)
    {
      if (!flag)
        return;
      this.OpenObjectEditor();
    }
    else
      objectWindow.IsOpen = flag;
  }

  public void OpenConfigWindow() => this._gui.GetOrCreate<ConfigWindow>().Open();

  public void ToggleWorkspaceWindow()
  {
    this._gui.GetOrCreate<WorkspaceWindow>((object) this._ctx).Toggle();
  }

  public void OpenCameraWindow() => this._gui.GetOrCreate<CameraWindow>((object) this._ctx).Open();

  public void OpenEnvironmentWindow()
  {
    ISceneManager scene = this._ctx.Scene;
    EnvModule module = scene.GetModule<EnvModule>();
    this._gui.GetOrCreate<EnvWindow>((object) scene, (object) module).Open();
  }

  public void OpenObjectEditor()
  {
    this._gui.GetOrCreate<ObjectWindow>((object) this._ctx, (object) new Gizmo2D((IGizmo) this._gizmo.Create(GizmoId.TransformEditor))).Open();
  }

  public void OpenObjectEditor(SceneEntity entity)
  {
    entity.Select(SelectMode.Force);
    this.OpenObjectEditor();
  }

  public void OpenPosingWindow()
  {
    this._gui.GetOrCreate<PosingWindow>((object) this._ctx, (object) this._ctx.Locale).Open();
  }

  public void OpenSceneCreateMenu()
  {
    this._gui.AddPopup<ContextMenu>(new SceneCreateMenuBuilder(this._ctx).Create()).Open();
  }

  public void OpenSceneEntityMenu(SceneEntity entity)
  {
    this._gui.AddPopup<ContextMenu>(new SceneEntityMenuBuilder(this._ctx, entity).Create()).Open();
  }

  public void OpenAssignCollection(ActorEntity entity)
  {
    this._gui.CreatePopup<ActorCollectionPopup>((object) this._ctx, (object) entity).Open();
  }

  public void OpenAssignCProfile(ActorEntity entity)
  {
    this._gui.CreatePopup<ActorCProfilePopup>((object) this._ctx, (object) entity).Open();
  }

  public void OpenOverworldActorList()
  {
    this._gui.CreatePopup<OverworldActorPopup>((object) this._ctx).Open();
  }

  public void RefreshGposeActors() => this._ctx.Scene.GetModule<ActorModule>().RefreshGPoseActors();

  public void OpenRenameEntity(SceneEntity entity)
  {
    this._gui.CreatePopup<EntityRenameModal>((object) entity).Open();
  }

  public void OpenActorEditor(ActorEntity actor)
  {
    if (!this._ctx.Config.Editor.UseLegacyWindowBehavior)
      actor.Select(SelectMode.Force);
    this.OpenEditor<ActorWindow, ActorEntity>(actor);
  }

  public void OpenLightEditor(LightEntity light)
  {
    if (this._ctx.Config.Editor.UseLegacyLightEditor)
      this.OpenEditor<LightWindow, LightEntity>(light);
    else
      this.OpenObjectEditor((SceneEntity) light);
  }

  public void OpenLightExportNoActorSelected(Action exportLight, Configuration config)
  {
    this._gui.CreatePopup<LightExportNoActorSelectedModal>((object) exportLight, (object) config).Open();
  }

  public void OpenEditor<T, TA>(TA entity)
    where T : EntityEditWindow<TA>
    where TA : SceneEntity
  {
    T obj = this._gui.GetOrCreate<T>((object) this._ctx);
    obj.SetTarget(entity);
    obj.Open();
  }

  public void OpenEditorFor(SceneEntity entity)
  {
    switch (entity)
    {
      case ActorEntity actor:
        this.OpenActorEditor(actor);
        break;
      case LightEntity light:
        this.OpenLightEditor(light);
        break;
    }
  }

  public void OpenCharaImport(ActorEntity actor)
  {
    this.OpenEditor<CharaImportDialog, ActorEntity>(actor);
  }

  public async Task OpenCharaExport(ActorEntity actor)
  {
    this.ExportCharaFile(await this._ctx.Characters.SaveCharaFile(actor));
  }

  public void OpenPoseImport(ActorEntity actor)
  {
    this.OpenEditor<PoseImportDialog, ActorEntity>(actor);
  }

  public async Task OpenPoseExport(EntityPose pose)
  {
    this.ExportPoseFile(await this._ctx.Posing.SavePoseFile(pose));
  }

  public void OpenLightImport(LightEntity light)
  {
    this.OpenEditor<LightImportDialog, LightEntity>(light);
  }

  public async Task OpenLightExport(LightEntity light)
  {
    this.ExportLightFile(await this._ctx.Scene.SaveLightFile(light));
  }

  public void OpenCharaFile(Action<string, CharaFile> handler)
  {
    this._gui.FileDialogs.OpenFile<CharaFile>("Open Chara File", handler, EditorInterface.CharaFileOptions);
  }

  public void OpenPoseFile(Action<string, PoseFile> handler)
  {
    this._gui.FileDialogs.OpenFile<PoseFile>("Open Pose File", (Action<string, PoseFile>) ((path, file) =>
    {
      file.ConvertLegacyBones();
      handler(path, file);
    }), EditorInterface.PoseFileOptions);
  }

  public void OpenMcdfFile(Action<string> handler)
  {
    this._gui.FileDialogs.OpenFile("Open MCDF File", handler, EditorInterface.McdfFileOptions);
  }

  public void OpenLightFile(Action<string, SceneManager.LightFile> handler)
  {
    this._gui.FileDialogs.OpenFile<SceneManager.LightFile>("Open Light File", (Action<string, SceneManager.LightFile>) ((path, file) => handler(path, file)), EditorInterface.LightFileOptions);
  }

  public void OpenReferenceImages(Action<string> handler)
  {
    this._gui.FileDialogs.OpenImage("image", handler);
  }

  public void ExportCharaFile(CharaFile file)
  {
    this._gui.FileDialogs.SaveFile<CharaFile>("Export Chara File", file, EditorInterface.CharaFileOptions);
  }

  public void ExportPoseFile(PoseFile file)
  {
    this._gui.FileDialogs.SaveFile<PoseFile>("Export Pose File", file, EditorInterface.PoseFileOptions);
  }

  public void ExportLightFile(SceneManager.LightFile file)
  {
    this._gui.FileDialogs.SaveFile<SceneManager.LightFile>("Export Light File", file, EditorInterface.LightFileOptions);
  }
}
