// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Config.Sections.FileConfig
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Editor.Characters;
using Ktisis.Editor.Posing.Data;
using Ktisis.Editor.Posing.Types;
using System.Collections.Generic;

#nullable enable
namespace Ktisis.Data.Config.Sections;

public class FileConfig
{
  public Dictionary<string, string> LastOpenedPaths = new Dictionary<string, string>();
  public SaveModes ImportCharaModes = SaveModes.All;
  public bool ImportNpcApplyOnSelect;
  public bool ImportPoseSelectedBones;
  public bool AnchorPoseSelectedBones;
  public bool ImportPoseSelectedBonesIncludeChildBones;
  public BoneTypeInclusion ImportPoseIncludeType;
  public PoseTransforms ImportPoseTransforms = PoseTransforms.Rotation;
  public PoseMode ImportPoseModes = PoseMode.All;
  public PoseTransforms ImportLightTransforms = PoseTransforms.Rotation | PoseTransforms.Scale;
  public bool ImportLightLighting = true;
  public bool ImportLightColor = true;
  public bool ImportLightShadows = true;
  public bool ExportLightIgnoreNoActorSelectedWarning;
}
