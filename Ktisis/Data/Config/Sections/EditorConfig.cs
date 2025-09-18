// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Config.Sections.EditorConfig
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System.Collections.Generic;

using Ktisis.Scene.Entities.Utility;
using Ktisis.Scene.Types;

namespace Ktisis.Data.Config.Sections;

public class EditorConfig {
	public Dictionary<EntityType, EntityDisplay> Display = EntityDisplay.GetDefaults();
	public bool FlipRotationCorrection = true;
	public bool FlipYawCorrection = true;
	public bool ForceLoop = true;
	public bool OpenOnEnterGPose = true;
	public bool PlayEmoteStart = true;
	public List<ReferenceImage.SetupData> ReferenceImages = new List<ReferenceImage.SetupData>();
	public bool ToggleEditorOnSelect = true;
	public bool TransformHide;
	public bool UseLegacyLightEditor;
	public bool UseLegacyPoseViewTabs;
	public bool UseLegacyWindowBehavior;

	public EntityDisplay GetDisplayForType(EntityType type) => CollectionExtensions.GetValueOrDefault<EntityType, EntityDisplay>((IReadOnlyDictionary<EntityType, EntityDisplay>)this.Display, type, new EntityDisplay());
}
