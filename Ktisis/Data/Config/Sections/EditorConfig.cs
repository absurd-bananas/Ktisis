// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Config.Sections.EditorConfig
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Data.Config.Entity;
using Ktisis.Scene.Entities.Utility;
using Ktisis.Scene.Types;
using System.Collections.Generic;

#nullable enable
namespace Ktisis.Data.Config.Sections;

public class EditorConfig
{
  public bool OpenOnEnterGPose = true;
  public bool ToggleEditorOnSelect = true;
  public bool UseLegacyWindowBehavior;
  public bool UseLegacyPoseViewTabs;
  public bool UseLegacyLightEditor;
  public Dictionary<EntityType, EntityDisplay> Display = EntityDisplay.GetDefaults();
  public List<ReferenceImage.SetupData> ReferenceImages = new List<ReferenceImage.SetupData>();
  public bool TransformHide;
  public bool FlipYawCorrection = true;
  public bool FlipRotationCorrection = true;
  public bool PlayEmoteStart = true;
  public bool ForceLoop = true;

  public EntityDisplay GetDisplayForType(EntityType type)
  {
    return CollectionExtensions.GetValueOrDefault<EntityType, EntityDisplay>((IReadOnlyDictionary<EntityType, EntityDisplay>) this.Display, type, new EntityDisplay());
  }
}
