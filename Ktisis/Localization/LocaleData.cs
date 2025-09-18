// Decompiled with JetBrains decompiler
// Type: Ktisis.Localization.LocaleData
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System.Collections.Generic;
using System.Text;

namespace Ktisis.Localization;

public class LocaleData {
	private readonly Dictionary<string, string> _translationData;
	private readonly HashSet<string> warnedKeys = new HashSet<string>();

	public LocaleData(LocaleMetaData metaData, Dictionary<string, string> translationData) {
		this._translationData = translationData;
		this.MetaData = metaData;
	}

	public LocaleMetaData MetaData { get; }

	public string Translate(string key, Dictionary<string, string>? parameters = null) {
		string translationString;
		if (this._translationData.TryGetValue(key, out translationString))
			return this.ReplaceParameters(key, translationString, parameters);
		if (this.warnedKeys.Add(key))
			Ktisis.Ktisis.Log.Warning("Unassigned translation key '{0}' for locale '{1}'", new object[2] {
				key,
				this.MetaData.TechnicalName
			});
		return key;
	}

	public bool HasTranslationFor(string key) => this._translationData.ContainsKey(key);

	private string ReplaceParameters(
		string handle,
		string translationString,
		Dictionary<string, string>? parameters
	) {
		var stringBuilder1 = new StringBuilder(translationString.Length);
		var stringBuilder2 = new StringBuilder(16 /*0x10*/);
		var flag = false;
		foreach (var ch in translationString) {
			if (!flag) {
				if (ch == '%')
					flag = true;
				else
					stringBuilder1.Append(ch);
			} else if (ch == '%') {
				if (stringBuilder2.Length == 0) {
					stringBuilder1.Append('%');
				} else {
					var key = stringBuilder2.ToString();
					var str = (string)null;
					parameters?.TryGetValue(key, out str);
					if (str == null) {
						Ktisis.Ktisis.Log.Warning("Unassigned parameter '{0}' in value for '{1}' in locale '{2}'", new object[3] {
							key,
							handle,
							this.MetaData.TechnicalName
						});
						str = $"%{key}%";
					}
					stringBuilder1.Append(str);
					stringBuilder2.Clear();
				}
				flag = false;
			} else
				stringBuilder2.Append(ch);
		}
		return stringBuilder1.ToString();
	}
}
