// Decompiled with JetBrains decompiler
// Type: Ktisis.Localization.LocaleManager
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;

using Ktisis.Core.Attributes;
using Ktisis.Data.Config;
using Ktisis.Data.Config.Bones;
using Ktisis.Editor.Posing.Types;

namespace Ktisis.Localization;

[Singleton]
public class LocaleManager {
	private readonly ConfigManager _cfg;
	private readonly LocaleDataLoader Loader = new LocaleDataLoader();
	private LocaleData? Data;

	public LocaleManager(ConfigManager cfg) {
		this._cfg = cfg;
	}

	public void Initialize() => this.LoadLocale(this._cfg.File.Locale.LocaleId);

	public string Translate(string handle, Dictionary<string, string>? parameters = null) => this.Data?.Translate(handle, parameters) ?? handle;

	public bool HasTranslationFor(string handle) {
		var data = this.Data;
		return data != null && data.HasTranslationFor(handle);
	}

	public void LoadLocale(string technicalName) {
		Ktisis.Ktisis.Log.Verbose($"Reading localization file for '{technicalName}'", Array.Empty<object>());
		if (this.Data != null && !(this.Data.MetaData.TechnicalName != technicalName))
			return;
		this.Data = this.Loader.LoadData(technicalName);
	}

	public string GetBoneName(PartialBoneInfo bone, bool untranslated = false) => this.GetBoneName(bone.Name, untranslated);

	public string GetBoneName(string name, bool untranslated) {
		var handle = "bone." + name;
		var friendlyBoneNames = this._cfg.File.Categories.ShowFriendlyBoneNames;
		return !(!untranslated & friendlyBoneNames) || !this.HasTranslationFor(handle) ? name : this.Translate(handle);
	}

	public string GetCategoryName(BoneCategory category) {
		var handle = "boneCategory." + category.Name;
		return !this.HasTranslationFor(handle) ? category.Name : this.Translate(handle);
	}
}
