// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Config.Configuration
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

using Ktisis.Data.Config.Actions;
using Ktisis.Data.Config.Sections;
using Ktisis.Scene.Entities;
using Ktisis.Scene.Entities.Skeleton;

namespace Ktisis.Data.Config;

[Serializable]
public class Configuration : IPluginConfiguration {
	public const int CurrentVersion = 10;
	public AutoSaveConfig AutoSave = new AutoSaveConfig();
	public CameraConfig Camera = new CameraConfig();
	public CategoryConfig Categories = new CategoryConfig();
	public EditorConfig Editor = new EditorConfig();
	public FileConfig File = new FileConfig();
	public GizmoConfig Gizmo = new GizmoConfig();
	public InputConfig Keybinds = new InputConfig();
	public LocaleConfig Locale = new LocaleConfig();
	public OverlayConfig Overlay = new OverlayConfig();
	public PyonConfig Pyon = new PyonConfig();

	public int Version { get; set; } = 10;

	public EntityDisplay GetEntityDisplay(SceneEntity entity) {
		EntityDisplay displayForType = this.Editor.GetDisplayForType(entity.Type);
		EntityDisplay entityDisplay;
		switch (entity) {
			case BoneNodeGroup boneNodeGroup:
				var category1 = boneNodeGroup.Category;
				if (category1 != null) {
					entityDisplay = displayForType with {
						Color = category1.GroupColor
					};
					break;
				}
				goto default;
			case BoneNode _:
				if (entity.Parent is BoneNodeGroup parent) {
					var category2 = parent.Category;
					if (category2 != null) {
						entityDisplay = displayForType with {
							Color = category2.LinkedColors ? category2.GroupColor : category2.BoneColor
						};
						break;
					}
				}
				goto default;
			default:
				entityDisplay = displayForType;
				break;
		}
		return entityDisplay;
	}
}
