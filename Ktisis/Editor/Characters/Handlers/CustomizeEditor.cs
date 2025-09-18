// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Characters.Handlers.CustomizeEditor
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;

using Ktisis.Editor.Characters.Types;
using Ktisis.Scene.Entities.Game;

namespace Ktisis.Editor.Characters.Handlers;

public class CustomizeEditor(ActorEntity actor) : ICustomizeEditor {
	private bool _isHetero;
	private bool _isHeteroGet;

	public unsafe byte GetCustomization(CustomizeIndex index) {
		if (!actor.IsValid)
			return 0;
		byte customization;
		if (this.TryGetFromState(index, out customization))
			return customization;
		return (IntPtr)actor.CharacterBaseEx == IntPtr.Zero ? (byte)0 : actor.CharacterBaseEx->Customize[(uint)index];
	}

	public void SetCustomization(CustomizeIndex index, byte value) {
		if (!this.SetCustomizeValue(index, value))
			return;
		this.UpdateCustomizeData(IsRedrawRequired(index));
	}

	public void SetHeterochromia(bool enabled) {
		this._isHetero = enabled;
		this._isHeteroGet = true;
		if (enabled)
			return;
		this.SetCustomization((CustomizeIndex)9, this.GetCustomization((CustomizeIndex)15));
	}

	public bool GetHeterochromia() {
		var customization1 = this.GetCustomization((CustomizeIndex)9);
		var customization2 = this.GetCustomization((CustomizeIndex)15);
		if (!this._isHeteroGet) {
			this._isHetero = customization1 != customization2;
			this._isHeteroGet = true;
		} else
			this._isHetero |= customization1 != customization2;
		return this._isHetero;
	}

	public void SetEyeColor(byte value) {
		var customizeBatch = this.Prepare().SetCustomization((CustomizeIndex)9, value);
		if (!this.GetHeterochromia())
			customizeBatch.SetCustomization((CustomizeIndex)15, value);
		customizeBatch.Apply();
	}

	public unsafe uint GetModelId() {
		if (!actor.IsValid)
			throw new Exception($"Actor entity '{actor.Name}' is invalid.");
		return actor.Appearance.ModelId ?? GetGameModel(actor.Character);
	}

	public void SetModelId(uint id, bool redraw = true) {
		if (!actor.IsValid)
			throw new Exception($"Actor entity '{actor.Name}' is invalid.");
		redraw &= this.ModelIdDiffers(id);
		actor.Appearance.ModelId = id;
		if (!redraw)
			return;
		actor.Redraw();
	}

	public unsafe void ApplyStateToGameObject() {
		if (!actor.IsValid || (IntPtr)actor.Character == IntPtr.Zero)
			return;
		for (var index = 0; index < 26; ++index) {
			var customization = this.GetCustomization((CustomizeIndex)index);
			((CustomizeData) ref actor.Character->DrawData.CustomizeData).Data[index] = customization;
		}
	}

	public ICustomizeBatch Prepare() => new CustomizeBatch(this);

	private bool TryGetFromState(CustomizeIndex index, out byte value) {
		value = byte.MaxValue;
		if (!actor.Appearance.Customize.IsSet(index))
			return false;
		value = actor.Appearance.Customize[index];
		return true;
	}

	private unsafe bool IsCurrentValue(CustomizeIndex index, byte value) {
		var flag1 = true;
		byte num1;
		var num2 = this.TryGetFromState(index, out num1) ? 1 : 0;
		if (num2 != 0)
			flag1 &= value == num1;
		var flag2 = (IntPtr)actor.CharacterBaseEx != IntPtr.Zero;
		if (flag2)
			flag1 &= value == (int)actor.CharacterBaseEx->Customize[(uint)index];
		return ((num2 | (flag2 ? 1 : 0)) & (flag1 ? 1 : 0)) != 0;
	}

	private unsafe bool SetCustomizeValue(CustomizeIndex index, byte value) {
		if (!actor.IsValid)
			return false;
		if ((index != 22 ? 0 : actor.IsViera() ? 1 : 0) != 0) {
			if (value > 4)
				value = value == byte.MaxValue ? (byte)4 : (byte)1;
			actor.Pose?.Refresh();
		}
		actor.Appearance.Customize[index] = value;
		CharacterBase* character = actor.GetCharacter();
		if ((IntPtr)character == IntPtr.Zero || ((CharacterBase)(IntPtr)character).GetModelType() != 1)
			return false;
		((CustomizeData) ref ((Human*)character)->Customize).Data[(int)index] = value;
		return true;
	}

	private unsafe void UpdateCustomizeData(bool redraw) {
		Human* human = actor.GetHuman();
		if (!redraw && (IntPtr)human != IntPtr.Zero)
			redraw = !((Human)(IntPtr)human).UpdateDrawData((byte*)&human->Customize, true);
		if (!redraw)
			return;
		actor.Redraw();
	}

	private static bool IsRedrawRequired(CustomizeIndex index) => index <= 1 || index - 4 <= 1;

	private unsafe bool ModelIdDiffers(uint id) => (int)id != ((int)actor.Appearance.ModelId ?? (int)GetGameModel(actor.Character));

	private unsafe static uint GetGameModel(FFXIVClientStructs.FFXIV.Client.Game.Character.Character* chara) {
		if ((IntPtr)chara == IntPtr.Zero)
			throw new Exception("Character is null.");
		return (uint)chara->ModelContainer.ModelCharaId;
	}

	private class CustomizeBatch(CustomizeEditor editor) : ICustomizeBatch {
		private readonly Dictionary<CustomizeIndex, byte> Values = new Dictionary<CustomizeIndex, byte>();
		private uint? ModelId;

		public ICustomizeBatch SetCustomization(CustomizeIndex index, byte value) {
			this.Values[index] = value;
			return this;
		}

		public ICustomizeBatch SetIfNotNull(CustomizeIndex index, byte? value) {
			if (!value.HasValue)
				return this;
			this.SetCustomization(index, value.Value);
			return this;
		}

		public ICustomizeBatch SetModelId(uint id) {
			this.ModelId = id;
			return this;
		}

		public void Apply() {
			var redraw = false;
			foreach (KeyValuePair<CustomizeIndex, byte> keyValuePair in this.Values) {
				CustomizeIndex customizeIndex;
				byte num1;
				keyValuePair.Deconstruct(ref customizeIndex, ref num1);
				CustomizeIndex index = customizeIndex;
				var num2 = num1;
				if (!editor.IsCurrentValue(index, num2))
					redraw = ((redraw ? 1 : 0) | (!editor.SetCustomizeValue(index, num2) ? 0 : IsRedrawRequired(index) ? 1 : 0)) != 0;
			}
			if (this.ModelId.HasValue) {
				redraw |= editor.ModelIdDiffers(this.ModelId.Value);
				editor.SetModelId(this.ModelId.Value, false);
			}
			editor.UpdateCustomizeData(redraw);
		}
	}
}
