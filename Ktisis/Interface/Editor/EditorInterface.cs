// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Editor.EditorInterface
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Threading.Tasks;

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

namespace Ktisis.Interface.Editor;

public class EditorInterface : IEditorInterface {
	private readonly static FileDialogOptions CharaFileOptions = new FileDialogOptions {
		Filters = "Character Files{.chara}",
		Extension = ".chara"
	};
	private readonly static FileDialogOptions PoseFileOptions = new FileDialogOptions {
		Filters = "Pose Files{.pose}",
		Extension = ".pose"
	};
	private readonly static FileDialogOptions McdfFileOptions = new FileDialogOptions {
		Filters = "MCDF Files{.mcdf}",
		Extension = ".mcdf"
	};
	private readonly static FileDialogOptions LightFileOptions = new FileDialogOptions {
		Filters = "Light Files{.light}",
		Extension = ".light"
	};
	private readonly IEditorContext _ctx;
	private readonly IDalamudPluginInterface _dpi;
	private readonly GizmoManager _gizmo;
	private readonly GuiManager _gui;

	public EditorInterface(IDalamudPluginInterface dpi, IEditorContext ctx, GuiManager gui) {
		this._dpi = dpi;
		this._ctx = ctx;
		this._gui = gui;
		this._gizmo = new GizmoManager(this._dpi, ctx.Config);
	}

	public void Prepare() {
		if (this._ctx.Config.Editor.OpenOnEnterGPose)
			this._gui.GetOrCreate<WorkspaceWindow>(this._ctx).Open();
		this._ctx.Selection.Changed += this.OnSelectChanged;
		this._gizmo.Initialize();
		this._gui.GetOrCreate<OverlayWindow>(this._ctx, this._gizmo.Create(GizmoId.OverlayMain)).Open();
	}

	public void OpenConfigWindow() => this._gui.GetOrCreate<ConfigWindow>().Open();

	public void ToggleWorkspaceWindow() {
		this._gui.GetOrCreate<WorkspaceWindow>(this._ctx).Toggle();
	}

	public void OpenCameraWindow() => this._gui.GetOrCreate<CameraWindow>(this._ctx).Open();

	public void OpenEnvironmentWindow() {
		var scene = this._ctx.Scene;
		var module = scene.GetModule<EnvModule>();
		this._gui.GetOrCreate<EnvWindow>(scene, module).Open();
	}

	public void OpenObjectEditor() {
		this._gui.GetOrCreate<ObjectWindow>(this._ctx, new Gizmo2D(this._gizmo.Create(GizmoId.TransformEditor))).Open();
	}

	public void OpenPosingWindow() {
		this._gui.GetOrCreate<PosingWindow>(this._ctx, this._ctx.Locale).Open();
	}

	public void OpenSceneCreateMenu() {
		this._gui.AddPopup(new SceneCreateMenuBuilder(this._ctx).Create()).Open();
	}

	public void OpenSceneEntityMenu(SceneEntity entity) {
		this._gui.AddPopup(new SceneEntityMenuBuilder(this._ctx, entity).Create()).Open();
	}

	public void OpenAssignCollection(ActorEntity entity) {
		this._gui.CreatePopup<ActorCollectionPopup>(this._ctx, entity).Open();
	}

	public void OpenAssignCProfile(ActorEntity entity) {
		this._gui.CreatePopup<ActorCProfilePopup>(this._ctx, entity).Open();
	}

	public void OpenOverworldActorList() {
		this._gui.CreatePopup<OverworldActorPopup>(this._ctx).Open();
	}

	public void RefreshGposeActors() => this._ctx.Scene.GetModule<ActorModule>().RefreshGPoseActors();

	public void OpenRenameEntity(SceneEntity entity) {
		this._gui.CreatePopup<EntityRenameModal>(entity).Open();
	}

	public void OpenActorEditor(ActorEntity actor) {
		if (!this._ctx.Config.Editor.UseLegacyWindowBehavior)
			actor.Select(SelectMode.Force);
		this.OpenEditor<ActorWindow, ActorEntity>(actor);
	}

	public void OpenLightEditor(LightEntity light) {
		if (this._ctx.Config.Editor.UseLegacyLightEditor)
			this.OpenEditor<LightWindow, LightEntity>(light);
		else
			this.OpenObjectEditor(light);
	}

	public void OpenLightExportNoActorSelected(Action exportLight, Configuration config) {
		this._gui.CreatePopup<LightExportNoActorSelectedModal>(exportLight, config).Open();
	}

	public void OpenEditor<T, TA>(TA entity)
		where T : EntityEditWindow<TA>
		where TA : SceneEntity {
		var obj = this._gui.GetOrCreate<T>(this._ctx);
		obj.SetTarget(entity);
		obj.Open();
	}

	public void OpenEditorFor(SceneEntity entity) {
		switch (entity) {
			case ActorEntity actor:
				this.OpenActorEditor(actor);
				break;
			case LightEntity light:
				this.OpenLightEditor(light);
				break;
		}
	}

	public void OpenCharaImport(ActorEntity actor) {
		this.OpenEditor<CharaImportDialog, ActorEntity>(actor);
	}

	public async Task OpenCharaExport(ActorEntity actor) => this.ExportCharaFile(await this._ctx.Characters.SaveCharaFile(actor));

	public void OpenPoseImport(ActorEntity actor) {
		this.OpenEditor<PoseImportDialog, ActorEntity>(actor);
	}

	public async Task OpenPoseExport(EntityPose pose) => this.ExportPoseFile(await this._ctx.Posing.SavePoseFile(pose));

	public void OpenLightImport(LightEntity light) {
		this.OpenEditor<LightImportDialog, LightEntity>(light);
	}

	public async Task OpenLightExport(LightEntity light) => this.ExportLightFile(await this._ctx.Scene.SaveLightFile(light));

	public void OpenCharaFile(Action<string, CharaFile> handler) {
		this._gui.FileDialogs.OpenFile("Open Chara File", handler, CharaFileOptions);
	}

	public void OpenPoseFile(Action<string, PoseFile> handler) {
		this._gui.FileDialogs.OpenFile("Open Pose File", (Action<string, PoseFile>)((path, file) => {
			file.ConvertLegacyBones();
			handler(path, file);
		}), PoseFileOptions);
	}

	public void OpenMcdfFile(Action<string> handler) {
		this._gui.FileDialogs.OpenFile("Open MCDF File", handler, McdfFileOptions);
	}

	public void OpenLightFile(Action<string, SceneManager.LightFile> handler) {
		this._gui.FileDialogs.OpenFile("Open Light File", (Action<string, SceneManager.LightFile>)((path, file) => handler(path, file)), LightFileOptions);
	}

	public void OpenReferenceImages(Action<string> handler) {
		this._gui.FileDialogs.OpenImage("image", handler);
	}

	public void ExportCharaFile(CharaFile file) {
		this._gui.FileDialogs.SaveFile("Export Chara File", file, CharaFileOptions);
	}

	public void ExportPoseFile(PoseFile file) {
		this._gui.FileDialogs.SaveFile("Export Pose File", file, PoseFileOptions);
	}

	public void ExportLightFile(SceneManager.LightFile file) {
		this._gui.FileDialogs.SaveFile("Export Light File", file, LightFileOptions);
	}

	private void OnSelectChanged(ISelectManager sender) {
		if (!this._ctx.Config.Editor.ToggleEditorOnSelect)
			return;
		var flag = sender.Count > 0;
		var objectWindow = this._gui.Get<ObjectWindow>();
		if (objectWindow == null) {
			if (!flag)
				return;
			this.OpenObjectEditor();
		} else
			objectWindow.IsOpen = flag;
	}

	public void OpenObjectEditor(SceneEntity entity) {
		entity.Select(SelectMode.Force);
		this.OpenObjectEditor();
	}
}
