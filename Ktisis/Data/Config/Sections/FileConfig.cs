// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Config.Sections.FileConfig
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System.Collections.Generic;

using Ktisis.Editor.Characters;
using Ktisis.Editor.Posing.Data;
using Ktisis.Editor.Posing.Types;

namespace Ktisis.Data.Config.Sections;

public class FileConfig {
	public bool AnchorPoseSelectedBones;
	public bool ExportLightIgnoreNoActorSelectedWarning;
	public SaveModes ImportCharaModes = SaveModes.All;
	public bool ImportLightColor = true;
	public bool ImportLightLighting = true;
	public bool ImportLightShadows = true;
	public PoseTransforms ImportLightTransforms = PoseTransforms.Rotation | PoseTransforms.Scale;
	public bool ImportNpcApplyOnSelect;
	public BoneTypeInclusion ImportPoseIncludeType;
	public PoseMode ImportPoseModes = PoseMode.All;
	public bool ImportPoseSelectedBones;
	public bool ImportPoseSelectedBonesIncludeChildBones;
	public PoseTransforms ImportPoseTransforms = PoseTransforms.Rotation;
	public Dictionary<string, string> LastOpenedPaths = new Dictionary<string, string>();
}
