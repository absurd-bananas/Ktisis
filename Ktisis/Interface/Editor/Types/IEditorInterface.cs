// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Editor.Types.IEditorInterface
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Data.Config;
using Ktisis.Data.Files;
using Ktisis.Interface.Types;
using Ktisis.Scene;
using Ktisis.Scene.Entities;
using Ktisis.Scene.Entities.Game;
using Ktisis.Scene.Entities.Skeleton;
using Ktisis.Scene.Entities.World;
using System;
using System.Threading.Tasks;

#nullable enable
namespace Ktisis.Interface.Editor.Types;

public interface IEditorInterface
{
  void Prepare();

  void OpenConfigWindow();

  void ToggleWorkspaceWindow();

  void OpenCameraWindow();

  void OpenEnvironmentWindow();

  void OpenObjectEditor();

  void OpenPosingWindow();

  void OpenSceneCreateMenu();

  void OpenSceneEntityMenu(SceneEntity entity);

  void OpenAssignCollection(ActorEntity entity);

  void OpenAssignCProfile(ActorEntity entity);

  void OpenOverworldActorList();

  void RefreshGposeActors();

  void OpenRenameEntity(SceneEntity entity);

  void OpenActorEditor(ActorEntity actor);

  void OpenLightEditor(LightEntity light);

  void OpenLightExportNoActorSelected(Action exportLight, Configuration config);

  void OpenEditor<T, TA>(TA entity)
    where T : EntityEditWindow<TA>
    where TA : SceneEntity;

  void OpenEditorFor(SceneEntity entity);

  void OpenCharaImport(ActorEntity actor);

  Task OpenCharaExport(ActorEntity actor);

  void OpenPoseImport(ActorEntity actor);

  Task OpenPoseExport(EntityPose pose);

  void OpenLightImport(LightEntity light);

  Task OpenLightExport(LightEntity light);

  void OpenCharaFile(Action<string, CharaFile> handler);

  void OpenPoseFile(Action<string, PoseFile> handler);

  void OpenMcdfFile(Action<string> handler);

  void OpenLightFile(Action<string, SceneManager.LightFile> handler);

  void OpenReferenceImages(Action<string> handler);

  void ExportCharaFile(CharaFile file);

  void ExportPoseFile(PoseFile file);

  void ExportLightFile(SceneManager.LightFile file);
}
