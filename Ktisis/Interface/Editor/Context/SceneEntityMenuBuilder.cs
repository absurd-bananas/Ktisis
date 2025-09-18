// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Editor.Context.SceneEntityMenuBuilder
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using GLib.Popups.Context;
using Ktisis.Common.Extensions;
using Ktisis.Data.Config.Bones;
using Ktisis.Editor.Context.Types;
using Ktisis.Editor.Selection;
using Ktisis.Interface.Editor.Types;
using Ktisis.Scene.Decor;
using Ktisis.Scene.Entities;
using Ktisis.Scene.Entities.Game;
using Ktisis.Scene.Entities.Skeleton;
using Ktisis.Scene.Entities.World;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Ktisis.Interface.Editor.Context;

public class SceneEntityMenuBuilder
{
  private readonly IEditorContext _ctx;
  private readonly SceneEntity _entity;

  private IEditorInterface Ui => this._ctx.Interface;

  public SceneEntityMenuBuilder(IEditorContext ctx, SceneEntity entity)
  {
    this._ctx = ctx;
    this._entity = entity;
  }

  public ContextMenu Create()
  {
    return new ContextMenuBuilder().Group(new Action<ContextMenuBuilder>(this.BuildEntityBaseTop)).Group(new Action<ContextMenuBuilder>(this.BuildEntityType)).Group(new Action<ContextMenuBuilder>(this.BuildEntityBaseBottom)).Build($"EntityContextMenu_{this.GetHashCode():X}");
  }

  private void BuildEntityBaseTop(ContextMenuBuilder menu)
  {
    if (!this._entity.IsSelected)
      menu.Action("Select", (Action) (() => this._entity.Select(SelectMode.Multiple)));
    else
      menu.Action("Unselect", new Action(this._entity.Unselect));
    IVisibility vis = this._entity as IVisibility;
    if (vis != null)
      menu.Action("Toggle display", (Action) (() => vis.Toggle()));
    if (this._entity is ActorEntity entity1)
      menu.Action("Toggle visibility", new Action(entity1.ToggleObjectVisibility));
    if (this._entity is LightEntity entity2)
      menu.Action("Toggle visibility", new Action(entity2.ToggleObjectVisibility));
    if (this._entity is BoneNodeGroup entity3 && entity3 != null)
    {
      BoneCategory category = entity3.Category;
      if (category != null)
        menu.Action((category.HideOnPoseEntity ? "Show" : "Hide") + " group with 'Pose' entity", (Action) (() => category.HideOnPoseEntity = !category.HideOnPoseEntity));
    }
    BoneNode boneNode = this._entity as BoneNode;
    if (boneNode == null || !(boneNode.Parent is BoneNodeGroup parent))
      return;
    BoneCategory category1 = parent.Category;
    if (category1 == null)
      return;
    CategoryBone catBone = category1.Bones.FirstOrDefault<CategoryBone>((Func<CategoryBone, bool>) (x => x.Name == boneNode.Info.Name));
    if (catBone == null)
      return;
    menu.Action((catBone.HideOnPoseEntity ? "Show" : "Hide") + " bone with 'Pose' entity", (Action) (() => catBone.HideOnPoseEntity = !catBone.HideOnPoseEntity));
  }

  private void BuildEntityBaseBottom(ContextMenuBuilder menu)
  {
    IAttachable attach = this._entity as IAttachable;
    if (attach != null && attach.IsAttached())
      menu.Separator().Action("Detach", (Action) (() => this._ctx.Posing.Attachments.Detach(attach)));
    menu.Separator().Action("Rename", (Action) (() => this.Ui.OpenRenameEntity(this._entity)));
    IDeletable deletable = this._entity as IDeletable;
    if (deletable == null)
      return;
    menu.Separator().Action("Delete", (Action) (() => deletable.Delete()));
  }

  private void BuildEntityType(ContextMenuBuilder menu)
  {
    switch (this._entity)
    {
      case ActorEntity actor:
        this.BuildActorMenu(menu, actor);
        break;
      case EntityPose pose:
        this.BuildPoseMenu(menu, pose);
        break;
      case LightEntity light:
        this.BuildLightMenu(menu, light);
        break;
    }
  }

  private void OpenEditor() => this.Ui.OpenEditorFor(this._entity);

  private void BuildActorMenu(ContextMenuBuilder menu, ActorEntity actor)
  {
    menu.Separator().Action("Target", new Action(((GameObjectEx) actor.Actor).SetGPoseTarget)).Separator().Group((Action<ContextMenuBuilder>) (sub => this.BuildActorIpcMenu(sub, actor))).Action("Edit appearance", new Action(this.OpenEditor)).Separator().SubMenu("Import...", (Action<ContextMenuBuilder>) (sub =>
    {
      ContextMenuBuilder contextMenuBuilder = sub.Action("Character (.chara)", (Action) (() => this.Ui.OpenCharaImport(actor))).Action("Pose file (.pose)", (Action) (() => this.Ui.OpenPoseImport(actor)));
      if (!this._ctx.Plugin.Ipc.IsAnyMcdfActive)
        return;
      contextMenuBuilder.Action("Mare Appearance (.mcdf)", (Action) (() => this.Ui.OpenMcdfFile((Action<string>) (path => this.ImportMcdf(actor, path)))));
    })).SubMenu("Export...", (Action<ContextMenuBuilder>) (sub => sub.Action("Character (.chara)", (Action) (() => this.Ui.OpenCharaExport(actor))).Action("Pose file (.pose)", (Action) (() => this.ExportPose(actor.Pose)))));
  }

  private void BuildActorIpcMenu(ContextMenuBuilder menu, ActorEntity actor)
  {
    if (this._ctx.Plugin.Ipc.IsPenumbraActive)
      menu.Action("Assign collection", (Action) (() => this.Ui.OpenAssignCollection(actor)));
    if (!this._ctx.Plugin.Ipc.IsCustomizeActive)
      return;
    menu.Action("Assign C+ profile", (Action) (() => this.Ui.OpenAssignCProfile(actor)));
  }

  private void ImportMcdf(ActorEntity actor, string path)
  {
    this._ctx.Characters.Mcdf.LoadAndApplyTo(path, actor.Actor);
  }

  private void BuildPoseMenu(ContextMenuBuilder menu, EntityPose pose)
  {
    menu.Separator().Action("Import pose", (Action) (() => this.ImportPose(pose))).Action("Export pose", (Action) (() => this.ExportPose(pose))).Separator().Action("Set to reference pose", (Action) (() => this._ctx.Posing.ApplyReferencePose(pose)));
  }

  private void ImportPose(EntityPose pose)
  {
    if (!(pose.Parent is ActorEntity parent))
      return;
    this.Ui.OpenPoseImport(parent);
  }

  private async void ExportPose(EntityPose? pose)
  {
    if (pose == null)
      return;
    await this.Ui.OpenPoseExport(pose);
  }

  private void BuildLightMenu(ContextMenuBuilder menu, LightEntity light)
  {
    menu.Separator().Action("Edit lighting", new Action(this.OpenEditor)).Separator().Action("Import light preset", (Action) (() => this.ImportLight(light))).Action("Export light preset", (Action) (() =>
    {
      bool flag = false;
      if (!this._ctx.Config.File.ExportLightIgnoreNoActorSelectedWarning)
      {
        IEnumerable<ActorEntity> source = this._ctx.Scene.Children.Where<SceneEntity>((Func<SceneEntity, bool>) (entity => entity is ActorEntity)).Cast<ActorEntity>();
        if (source.Count<ActorEntity>() > 0 && source.FirstOrDefault<ActorEntity>((Func<ActorEntity, bool>) (x => x.IsSelected)) == null && source.Count<ActorEntity>() > 1)
          flag = true;
      }
      if (flag)
        this.Ui.OpenLightExportNoActorSelected((Action) (() => this.ExportLight(light)), this._ctx.Config);
      else
        this.ExportLight(light);
    }));
  }

  private void ImportLight(LightEntity light) => this.Ui.OpenLightImport(light);

  private async void ExportLight(LightEntity? light)
  {
    if (light == null)
      return;
    await this.Ui.OpenLightExport(light);
  }
}
