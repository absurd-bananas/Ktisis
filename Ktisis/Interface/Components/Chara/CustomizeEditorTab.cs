// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Chara.CustomizeEditorTab
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

using GLib.Widgets;

using Ktisis.Core.Attributes;
using Ktisis.Editor.Characters.Make;
using Ktisis.Editor.Characters.Types;
using Ktisis.Interface.Components.Chara.Popup;
using Ktisis.Services.Data;
using Ktisis.Structs.Characters;

namespace Ktisis.Interface.Components.Chara;

[Transient]
public class CustomizeEditorTab {
	private const float SideRatio = 0.35f;
	private const string LegacyTexPath = "chara/common/texture/decal_equip/_stigma.tex";
	private readonly static Vector2 MaxButtonSize;
	private readonly static CustomizeIndex[] FeatIconParams;
	private readonly ParamColorSelectPopup _colorPopup = new ParamColorSelectPopup();
	private readonly IDataManager _data;
	private readonly CustomizeService _discovery;
	private readonly FeatureSelectPopup _featurePopup;
	private readonly MakeTypeData _makeTypeData = new MakeTypeData();
	private readonly ITextureProvider _tex;
	private bool _isSetup;
	private Vector2 ButtonSize = MaxButtonSize;

	static CustomizeEditorTab() {
		// ISSUE: unable to decompile the method.
	}

	public CustomizeEditorTab(IDataManager data, ITextureProvider tex, CustomizeService discovery) {
		this._data = data;
		this._tex = tex;
		this._discovery = discovery;
		this._featurePopup = new FeatureSelectPopup(tex);
	}

	public ICustomizeEditor Editor { set; private get; }

	public void Setup() {
		if (this._isSetup)
			return;
		this._isSetup = true;
		this._makeTypeData.Build(this._data, this._discovery).ContinueWith(task => {
			if (task.Exception == null)
				return;
			Ktisis.Ktisis.Log.Error($"Failed to build customize data:\n{task.Exception}", Array.Empty<object>());
		});
	}

	public void Draw() {
		this.ButtonSize = CalcButtonSize();
		var data = this._makeTypeData.GetData((Tribe)this.Editor.GetCustomization((CustomizeIndex)4), (Gender)this.Editor.GetCustomization((CustomizeIndex)1));
		if (data == null)
			return;
		this.Draw(data);
		this._colorPopup.Draw(this.Editor);
		this._featurePopup.Draw(this.Editor);
	}

	private void Draw(MakeTypeRace data) {
		this.DrawSideFrame(data);
		Dalamud.Bindings.ImGui.ImGui.SameLine();
		this.DrawMainFrame(data);
	}

	private void DrawSideFrame(MakeTypeRace data) {
		Vector2 contentRegionAvail = Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail();
		contentRegionAvail.X = MathF.Max(contentRegionAvail.X * 0.35f, 240f);
		using (ImRaii.Child(ImU8String.op_Implicit("##CustomizeSideFrame"), contentRegionAvail, true)) {
			float cursorPosX = Dalamud.Bindings.ImGui.ImGui.GetCursorPosX();
			this.DrawBodySelect(data.Gender);
			ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
			Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, ((ImGuiStylePtr) ref style).ItemInnerSpacing.X);
			Dalamud.Bindings.ImGui.ImGui.SetNextItemWidth(Dalamud.Bindings.ImGui.ImGui.CalcItemWidth() - (Dalamud.Bindings.ImGui.ImGui.GetCursorPosX() - cursorPosX));
			this.DrawTribeSelect(data.Tribe);
			Dalamud.Bindings.ImGui.ImGui.Spacing();
			this.DrawFeatSlider((CustomizeIndex)3, data);
			this.DrawFeatSlider((CustomizeIndex)23, data);
			this.DrawFeatSlider((CustomizeIndex)21, data);
			Dalamud.Bindings.ImGui.ImGui.Spacing();
			this.DrawFeatParams((CustomizeIndex)16 /*0x10*/, data);
			this.DrawEyeColorSwitch();
			this.DrawIrisSizeSwitch();
			Dalamud.Bindings.ImGui.ImGui.Spacing();
			this.DrawFeatParams((CustomizeIndex)19, data);
			this.DrawLipColorSwitch();
			Dalamud.Bindings.ImGui.ImGui.Spacing();
			this.DrawFeatParams((CustomizeIndex)14, data);
			this.DrawFeatParams((CustomizeIndex)17, data);
			this.DrawFeatParams((CustomizeIndex)18, data);
		}
	}

	private void DrawBodySelect(Gender current) {
		if (!Buttons.IconButton(current == Gender.Masculine ? (FontAwesomeIcon)61986 : (FontAwesomeIcon)61985))
			return;
		this.Editor.SetCustomization((CustomizeIndex)1, (byte)(current != Gender.Feminine));
	}

	private void DrawTribeSelect(Tribe current) {
		using (ImRaii.IEndObject iendObject = ImRaii.Combo(ImU8String.op_Implicit("Body"), ImU8String.op_Implicit(current.ToString()))) {
			if (!iendObject.Success)
				return;
			foreach (Tribe tribe in Enum.GetValues<Tribe>()) {
				if (Dalamud.Bindings.ImGui.ImGui.Selectable(ImU8String.op_Implicit(tribe.ToString()), tribe == current, (ImGuiSelectableFlags)0, new Vector2()))
					this.Editor.Prepare().SetCustomization((CustomizeIndex)4, (byte)tribe).SetCustomization((CustomizeIndex)0, (byte)Math.Floor(((byte)tribe + 1M) / 2M)).Apply();
			}
		}
	}

	private void DrawSlider(string label, CustomizeIndex index) {
		var customization = (int)this.Editor.GetCustomization(index);
		if (!Dalamud.Bindings.ImGui.ImGui.SliderInt(ImU8String.op_Implicit(label), ref customization, 0, 100, new ImU8String(), (ImGuiSliderFlags)0))
			return;
		this.Editor.SetCustomization(index, (byte)customization);
	}

	private void DrawFeatSlider(CustomizeIndex index, MakeTypeRace data) {
		var feature = data.GetFeature(index);
		if (feature == null)
			return;
		this.DrawSlider(feature.Name, index);
	}

	private void DrawFeatParams(CustomizeIndex index, MakeTypeRace data) {
		var feature = data.GetFeature(index);
		if (feature == null)
			return;
		var customization = this.Editor.GetCustomization(index);
		var num1 = (int)(byte)(customization & 4294967167U);
		var nullable1 = feature.Params.FirstOrDefault()?.Value;
		var nullable2 = nullable1.HasValue ? nullable1.GetValueOrDefault() : new int?();
		var num2 = 0;
		var flag = nullable2.GetValueOrDefault() == num2 & nullable2.HasValue;
		var num3 = num1;
		if (flag)
			++num3;
		if (!Dalamud.Bindings.ImGui.ImGui.InputInt(ImU8String.op_Implicit(feature.Name), ref num3, 1, 0, new ImU8String(), (ImGuiInputTextFlags)0) || num3 < (flag ? 1 : 0))
			return;
		int num4;
		if (!flag)
			num4 = num3;
		else
			num3 = num4 = num3 - 1;
		var num5 = (byte)num4;
		this.Editor.SetCustomization(index, (byte)(num5 | customization & 128U /*0x80*/));
	}

	private void DrawIrisSizeSwitch() {
		float cursorPosX = Dalamud.Bindings.ImGui.ImGui.GetCursorPosX();
		Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, 0.0f);
		using (ImRaii.Group()) {
			Dalamud.Bindings.ImGui.ImGui.SetCursorPosX(cursorPosX);
			var customization = this.Editor.GetCustomization((CustomizeIndex)16 /*0x10*/);
			var flag = (customization & 128U /*0x80*/) > 0U;
			if (!Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Small Iris"), ref flag))
				return;
			this.Editor.SetCustomization((CustomizeIndex)16 /*0x10*/, (byte)(customization ^ 128U /*0x80*/));
		}
	}

	private void DrawMainFrame(MakeTypeRace data) {
		using (ImRaii.IEndObject iendObject = ImRaii.Child(ImU8String.op_Implicit("##CustomizeMainFrame"), Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail())) {
			if (!iendObject.Success)
				return;
			Dalamud.Bindings.ImGui.ImGui.Spacing();
			this.DrawSkinHairColors(data);
			Dalamud.Bindings.ImGui.ImGui.Spacing();
			this.DrawFacePaintOptions(data);
			Dalamud.Bindings.ImGui.ImGui.Spacing();
			if (Dalamud.Bindings.ImGui.ImGui.CollapsingHeader(ImU8String.op_Implicit("Primary Features"), (ImGuiTreeNodeFlags)0))
				this.DrawFeatIconParams(data);
			Dalamud.Bindings.ImGui.ImGui.Spacing();
			var str = "Facial Features";
			var feature = data.GetFeature((CustomizeIndex)12);
			if (feature != null && HasUniqueFeature(data.Tribe))
				str = $"{str} / {feature.Name}";
			if (!Dalamud.Bindings.ImGui.ImGui.CollapsingHeader(ImU8String.op_Implicit(str + " / Tattoos"), (ImGuiTreeNodeFlags)0))
				return;
			this.DrawFacialFeatures(data);
		}
	}

	private static bool HasUniqueFeature(Tribe tribe) {
		bool flag;
		switch (tribe) {
			case Tribe.Wildwood:
			case Tribe.MoonKeeper:
			case Tribe.Raen:
			case Tribe.Xaela:
				flag = true;
				break;
			default:
				flag = false;
				break;
		}
		return flag;
	}

	private static Vector2 CalcButtonSize() {
		var num = Dalamud.Bindings.ImGui.ImGui.GetWindowSize().X * 0.65f;
		Vector2 vector2 = new Vector2(num, num);
		return Vector2.Min(MaxButtonSize, vector2 / 8f);
	}

	private void DrawFeatIconParams(MakeTypeRace data) {
		ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		Dalamud.Bindings.ImGui.ImGui.PushItemWidth((float)((double)Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail().X / 2.0 - (double)this.ButtonSize.X - ((double)((ImGuiStylePtr) ref style).FramePadding.X +
			(double)((ImGuiStylePtr) ref style).ItemSpacing.X) *2.0));
		try {
			var num = 0;
			var flag = false;
			foreach (int featIconParam in FeatIconParams) {
				CustomizeIndex index = (CustomizeIndex)featIconParam;
				if (this.DrawFeatIconParams(data, index)) {
					flag = ++num % 2 != 0;
					if (flag)
						Dalamud.Bindings.ImGui.ImGui.SameLine();
				}
			}
			if (!flag)
				return;
			Dalamud.Bindings.ImGui.ImGui.Dummy(Vector2.Zero);
		} finally {
			Dalamud.Bindings.ImGui.ImGui.PopItemWidth();
		}
	}

	private bool DrawFeatIconParams(MakeTypeRace data, CustomizeIndex index) {
		var feature = data.GetFeature(index);
		if (feature == null)
			return false;
		var customization = this.Editor.GetCustomization(index);
		bool flag = index == 24;
		var value = flag ? (byte)(customization & 4294967167U) : customization;
		var makeTypeParam = feature.Params.FirstOrDefault(param => param.Value == value);
		if (this.DrawFeatIconButton($"{value}", makeTypeParam))
			this._featurePopup.Open(feature);
		float y = Dalamud.Bindings.ImGui.ImGui.GetItemRectSize().Y;
		Dalamud.Bindings.ImGui.ImGui.SameLine();
		using (ImRaii.Group()) {
			var num1 = y / 2.0;
			var heightWithSpacing = (double)Dalamud.Bindings.ImGui.ImGui.GetFrameHeightWithSpacing();
			ImFontPtr iconFont = UiBuilder.IconFont;
			var num2 = (double)((ImFontPtr) ref iconFont ).FontSize;
			var num3 = heightWithSpacing + num2;
			var num4 = (float)(num1 - num3);
			Dalamud.Bindings.ImGui.ImGui.Dummy(Vector2.Zero with {
				Y = num4
			});
			Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(feature.Name));
			var num5 = (int)value;
			ImU8String imU8String = new ImU8String(8, 1);
			((ImU8String) ref imU8String).AppendLiteral("##Input_");
			((ImU8String) ref imU8String).AppendFormatted<CustomizeIndex>(feature.Index);
			if (Dalamud.Bindings.ImGui.ImGui.InputInt(imU8String, ref num5, 1, 0, new ImU8String(), (ImGuiInputTextFlags)0) && (index != 5 ? 1 : feature.Params.Any(p => p.Value == value) ? 1 : 0) != 0)
				this.Editor.SetCustomization(index, flag ? (byte)(num5 | customization & 128 /*0x80*/) : (byte)num5);
			return true;
		}
	}

	private bool DrawFeatIconButton(string fallback, MakeTypeParam? param) {
		using (ImRaii.PushColor((ImGuiCol)21, 0U, true)) {
			ISharedImmediateTexture immediateTexture = (ISharedImmediateTexture)null;
			if (param != null && param.Graphic != 0U) {
				ITextureProvider tex = this._tex;
				GameIconLookup gameIconLookup = GameIconLookup.op_Implicit(param.Graphic);
				ref GameIconLookup local1 = ref gameIconLookup;
				ref ISharedImmediateTexture local2 = ref immediateTexture;
				tex.TryGetFromGameIcon(ref local1, ref local2);
			}
			bool flag;
			if (immediateTexture != null) {
				flag = Dalamud.Bindings.ImGui.ImGui.ImageButton(immediateTexture.GetWrapOrEmpty().Handle, this.ButtonSize);
			} else {
				ImU8String imU8String = ImU8String.op_Implicit(fallback);
				Vector2 buttonSize = this.ButtonSize;
				ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
				Vector2 vector2_1 = ((ImGuiStylePtr) ref style ).FramePadding * 2f;
				Vector2 vector2_2 = buttonSize + vector2_1;
				flag = Dalamud.Bindings.ImGui.ImGui.Button(imU8String, vector2_2);
			}
			return flag;
		}
	}

	private void DrawFacePaintOptions(MakeTypeRace data) {
		var cursorPosX = (double)Dalamud.Bindings.ImGui.ImGui.GetCursorPosX();
		ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		var x = (double)((ImGuiStylePtr) ref style ).FramePadding.X;
		Dalamud.Bindings.ImGui.ImGui.SetCursorPosX((float)(cursorPosX + x));
		using (ImRaii.Group()) {
			this.DrawFeatColor((CustomizeIndex)25, data);
			Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f);
			var customization = this.Editor.GetCustomization((CustomizeIndex)24);
			var flag = (customization & 128U /*0x80*/) > 0U;
			if (!Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Flip Face Paint"), ref flag))
				return;
			this.Editor.SetCustomization((CustomizeIndex)24, (byte)(customization ^ 128U /*0x80*/));
		}
	}

	private void DrawFacialFeatures(MakeTypeRace data) {
		ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		var customization = this.Editor.GetCustomization((CustomizeIndex)12);
		this.DrawFacialFeatureToggles(data, customization);
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		var num1 = (double)((ImGuiStylePtr) ref style ).ItemInnerSpacing.X + ((double)this.ButtonSize.X + (double)((ImGuiStylePtr) ref style).FramePadding.X * 2.0) *4.0;
		Dalamud.Bindings.ImGui.ImGui.SetCursorPosX(Dalamud.Bindings.ImGui.ImGui.GetCursorPosX() + ((ImGuiStylePtr) ref style).FramePadding.X);
		Dalamud.Bindings.ImGui.ImGui.SetNextItemWidth((float)(num1 / 2.0));
		var num2 = (int)customization;
		if (Dalamud.Bindings.ImGui.ImGui.InputInt(ImU8String.op_Implicit("##FaceFeatureFlags"), ref num2, 1, 0, new ImU8String(), (ImGuiInputTextFlags)0))
			this.Editor.SetCustomization((CustomizeIndex)12, (byte)num2);
		var feature = data.GetFeature((CustomizeIndex)13);
		if (feature == null)
			return;
		var colors = this._makeTypeData.GetColors((CustomizeIndex)13);
		Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, ((ImGuiStylePtr) ref style).ItemSpacing.X);
		this.DrawColorButton((CustomizeIndex)13, colors);
		Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, ((ImGuiStylePtr) ref style).ItemInnerSpacing.X);
		Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(feature.Name));
	}

	private void DrawFacialFeatureToggles(MakeTypeRace data, byte current) {
		using (ImRaii.Group()) {
			ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
			var customization = this.Editor.GetCustomization((CustomizeIndex)5);
			uint[] source;
			if (!data.FaceFeatureIcons.TryGetValue(customization, out source))
				source = data.FaceFeatureIcons.Values.FirstOrDefault();
			if (source == null)
				source = Array.Empty<uint>();
			IEnumerable<ISharedImmediateTexture> immediateTextures = source.Select((Func<uint, ISharedImmediateTexture>)(id => {
				ITextureProvider tex = this._tex;
				GameIconLookup gameIconLookup = GameIconLookup.op_Implicit(id);
				ref GameIconLookup local = ref gameIconLookup;
				return tex.GetFromGameIcon(ref local);
			})).Append(this._tex.GetFromGame("chara/common/texture/decal_equip/_stigma.tex"));
			var num1 = 0;
			foreach (ISharedImmediateTexture immediateTexture in immediateTextures) {
				if (num1++ % 4 != 0)
					Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, ((ImGuiStylePtr) ref style).ItemInnerSpacing.X)
				;
				var num2 = (byte)Math.Pow(2.0, num1 - 1);
				using (ImRaii.PushColor((ImGuiCol)21, (current & (uint)num2) > 0U ? Dalamud.Bindings.ImGui.ImGui.GetColorU32((ImGuiCol)23) : 0U, true)) {
					bool flag;
					if (immediateTexture != null) {
						flag = Dalamud.Bindings.ImGui.ImGui.ImageButton(immediateTexture.GetWrapOrEmpty().Handle, this.ButtonSize);
					} else {
						ImU8String imU8String = new ImU8String(0, 1);
						((ImU8String) ref imU8String).AppendFormatted<int>(num1);
						flag = Dalamud.Bindings.ImGui.ImGui.Button(imU8String, this.ButtonSize + ((ImGuiStylePtr) ref style).FramePadding * 2f);
					}
					if (flag)
						this.Editor.SetCustomization((CustomizeIndex)12, (byte)(current ^ (uint)num2));
				}
			}
		}
	}

	private void DrawSkinHairColors(MakeTypeRace data) {
		ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		Dalamud.Bindings.ImGui.ImGui.SetCursorPosX(Dalamud.Bindings.ImGui.ImGui.GetCursorPosX() + ((ImGuiStylePtr) ref style).CellPadding.X);
		using (ImRaii.Group()) {
			this.DrawFeatColor((CustomizeIndex)8, data);
			Dalamud.Bindings.ImGui.ImGui.SameLine();
			this.DrawFeatColor((CustomizeIndex)10, data);
			Dalamud.Bindings.ImGui.ImGui.SameLine();
			this.DrawHighlights();
		}
	}

	private void DrawColorButton(CustomizeIndex index, uint[] colors) {
		var customization = this.Editor.GetCustomization(index);
		if (colors.Length == 128 /*0x80*/)
			customization &= 127 /*0x7F*/;
		Vector4 float4 = Dalamud.Bindings.ImGui.ImGui.ColorConvertU32ToFloat4(customization < colors.Length ? colors[customization] : 0U);
		ImU8String imU8String;
		// ISSUE: explicit constructor call
		((ImU8String) ref imU8String).\u002Ector(2, 2);
		((ImU8String) ref imU8String).AppendFormatted<byte>(customization);
		((ImU8String) ref imU8String).AppendLiteral("##");
		((ImU8String) ref imU8String).AppendFormatted<CustomizeIndex>(index);
		if (!Dalamud.Bindings.ImGui.ImGui.ColorButton(imU8String, ref float4, (ImGuiColorEditFlags)0, new Vector2()))
			return;
		this._colorPopup.Open(index, colors);
	}

	private void DrawFeatColor(CustomizeIndex index, MakeTypeRace data) {
		var feature = data.GetFeature(index);
		if (feature == null)
			return;
		using (ImRaii.Group()) {
			var colors = this._makeTypeData.GetColors(index, data.Tribe, data.Gender);
			this.DrawColorButton(index, colors);
			ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
			Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, ((ImGuiStylePtr) ref style).ItemInnerSpacing.X);
			Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(feature.Name));
		}
	}

	private void DrawHighlights() {
		ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		using (ImRaii.Group()) {
			var customization = this.Editor.GetCustomization((CustomizeIndex)7);
			var flag = (customization & 128U /*0x80*/) > 0U;
			if (Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("##HighlightToggle"), ref flag))
				this.Editor.SetCustomization((CustomizeIndex)7, (byte)(customization ^ 128U /*0x80*/));
			Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, ((ImGuiStylePtr) ref style).ItemInnerSpacing.X);
			var colors = this._makeTypeData.GetColors((CustomizeIndex)11);
			using (ImRaii.Disabled(!flag)) {
				this.DrawColorButton((CustomizeIndex)11, colors);
				Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, ((ImGuiStylePtr) ref style).ItemInnerSpacing.X);
				Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Highlights"));
			}
		}
	}

	private void DrawEyeColorSwitch() {
		var colors = this._makeTypeData.GetColors((CustomizeIndex)9);
		if (colors.Length == 0)
			return;
		var enabled = this.Editor.GetHeterochromia();
		ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		float frameHeight = Dalamud.Bindings.ImGui.ImGui.GetFrameHeight();
		Dalamud.Bindings.ImGui.ImGui.SetCursorPosX((float)((double)Dalamud.Bindings.ImGui.ImGui.GetCursorPosX() + (double)Dalamud.Bindings.ImGui.ImGui.CalcItemWidth() - frameHeight * 3.0 - (double)((ImGuiStylePtr) ref style).ItemInnerSpacing.X *
			2.0));
		using (ImRaii.Group()) {
			using (ImRaii.PushColor((ImGuiCol)21, 0U, true)) {
				if (Buttons.IconButton(enabled ? (FontAwesomeIcon)61735 : (FontAwesomeIcon)61633, new Vector2?(new Vector2(frameHeight, frameHeight)))) {
					enabled = !enabled;
					this.Editor.SetHeterochromia(enabled);
				}
			}
			Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, ((ImGuiStylePtr) ref style).ItemInnerSpacing.X);
			using (ImRaii.Disabled(!enabled))
				this.DrawColorButton(enabled ? (CustomizeIndex)9 : (CustomizeIndex)15, colors);
			Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, ((ImGuiStylePtr) ref style).ItemInnerSpacing.X);
			this.DrawColorButton(enabled ? (CustomizeIndex)15 : (CustomizeIndex)9, colors);
			Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, ((ImGuiStylePtr) ref style).ItemInnerSpacing.X);
			Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Eye Color"));
		}
	}

	private void DrawLipColorSwitch() {
		var colors = this._makeTypeData.GetColors((CustomizeIndex)20);
		if (colors.Length == 0)
			return;
		ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		float frameHeight = Dalamud.Bindings.ImGui.ImGui.GetFrameHeight();
		Dalamud.Bindings.ImGui.ImGui.SetCursorPosX((float)((double)Dalamud.Bindings.ImGui.ImGui.GetCursorPosX() + (double)Dalamud.Bindings.ImGui.ImGui.CalcItemWidth() - frameHeight * 2.0) - ((ImGuiStylePtr) ref style).ItemInnerSpacing.X);
		var customization = this.Editor.GetCustomization((CustomizeIndex)19);
		var flag = (customization & 128U /*0x80*/) > 0U;
		if (Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("##ToggleLipColor"), ref flag))
			this.Editor.SetCustomization((CustomizeIndex)19, (byte)(customization ^ 128U /*0x80*/));
		Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, ((ImGuiStylePtr) ref style).ItemInnerSpacing.X);
		using (ImRaii.Disabled(!flag))
			this.DrawColorButton((CustomizeIndex)20, colors);
		Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, ((ImGuiStylePtr) ref style).ItemInnerSpacing.X);
		Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Lipstick"));
	}
}
