// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Editor.Properties.ImagePropertyList
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.IO;

using GLib.Widgets;

using Ktisis.Editor.Context.Types;
using Ktisis.Interface.Editor.Properties.Types;
using Ktisis.Scene.Entities;
using Ktisis.Scene.Entities.Utility;

namespace Ktisis.Interface.Editor.Properties;

public class ImagePropertyList : ObjectPropertyList {
	private readonly IEditorContext _ctx;

	public ImagePropertyList(IEditorContext ctx) {
		this._ctx = ctx;
	}

	public override void Invoke(IPropertyListBuilder builder, SceneEntity entity) {
		var img = entity as ReferenceImage;
		if (img == null)
			return;
		builder.AddHeader("Reference Image", (Action)(() => this.DrawImageTab(img)));
	}

	private void DrawImageTab(ReferenceImage img) {
		Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Enabled"), ref img.Data.Visible);
		var fileName = Path.GetFileName(img.Data.FilePath);
		Dalamud.Bindings.ImGui.ImGui.InputText(ImU8String.op_Implicit("##RefImgPath"), ref fileName, 512 /*0x0200*/, (ImGuiInputTextFlags)16400, (Dalamud.Bindings.ImGui.ImGui.ImGuiInputTextCallbackDelegate)null);
		ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, ((ImGuiStylePtr) ref style).ItemInnerSpacing.X);
		if (Buttons.IconButtonTooltip((FontAwesomeIcon)62831, "Load image", new Vector2?(new Vector2(0.0f, Dalamud.Bindings.ImGui.ImGui.GetFrameHeight()))))
			this._ctx.Interface.OpenReferenceImages(img.SetFilePath);
		Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Opacity##RefImgOpacity"), ref img.Data.Opacity, 0.0f, 1f, new ImU8String(), (ImGuiSliderFlags)0);
	}
}
