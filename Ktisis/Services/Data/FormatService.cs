// Decompiled with JetBrains decompiler
// Type: Ktisis.Services.Data.FormatService
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Ktisis.Core.Attributes;

namespace Ktisis.Services.Data;

[Singleton]
public class FormatService {
	private readonly IClientState _client;
	private readonly IDataManager _data;
	private readonly List<string> ReplacerKeys;

	public FormatService(IClientState client, IDataManager data) {
		var capacity = 9;
		var stringList = new List<string>(capacity);
		CollectionsMarshal.SetCount<string>(stringList, capacity);
		Span<string> span = CollectionsMarshal.AsSpan<string>(stringList);
		var num1 = 0;
		span[num1] = "%Date%";
		var num2 = num1 + 1;
		span[num2] = "%Year%";
		var num3 = num2 + 1;
		span[num3] = "%Month%";
		var num4 = num3 + 1;
		span[num4] = "%Day%";
		var num5 = num4 + 1;
		span[num5] = "%Time%";
		var num6 = num5 + 1;
		span[num6] = "%PlayerName%";
		var num7 = num6 + 1;
		span[num7] = "%CurrentWorld%";
		var num8 = num7 + 1;
		span[num8] = "%HomeWorld%";
		var num9 = num8 + 1;
		span[num9] = "%Zone%";
		this.ReplacerKeys = stringList;
		// ISSUE: explicit constructor call
		base.\u002Ector();
		this._client = client;
		this._data = data;
	}

	public string Replace(string path) {
		using (var enumerator = this.ReplacerKeys.Where(path.Contains).GetEnumerator()) {
			label_4:
			if (enumerator.MoveNext()) {
				var current = enumerator.Current;
				var keyReplacement = this.GetKeyReplacement(current);
				do {
					path = path.Replace(current, keyReplacement, true, (CultureInfo)null);
				} while (path.Contains(current));
				goto label_4;
			}
		}
		return path;
	}

	public Dictionary<string, string> GetReplacements() {
		return this.ReplacerKeys.ToDictionary(key => key, this.GetKeyReplacement);
	}

	private string GetKeyReplacement(string key) {
		string keyReplacement;
		if (key != null) {
			switch (key.Length) {
				case 5:
					if (key == "%Day%") {
						keyReplacement = DateTime.Now.ToString("dd");
						goto label_22;
					}
					break;
				case 6:
					switch (key[1]) {
						case 'D':
							if (key == "%Date%") {
								keyReplacement = DateTime.Now.ToString("yyyy-MM-dd");
								goto label_22;
							}
							break;
						case 'T':
							if (key == "%Time%") {
								keyReplacement = DateTime.Now.ToString("hh-mm-ss");
								goto label_22;
							}
							break;
						case 'Y':
							if (key == "%Year%") {
								keyReplacement = DateTime.Now.ToString("yyyy");
								goto label_22;
							}
							break;
						case 'Z':
							if (key == "%Zone%") {
								keyReplacement = this.GetZone();
								goto label_22;
							}
							break;
					}
					break;
				case 7:
					if (key == "%Month%") {
						keyReplacement = DateTime.Now.ToString("MM");
						goto label_22;
					}
					break;
				case 11:
					if (key == "%HomeWorld%") {
						keyReplacement = this.GetHomeWorld();
						goto label_22;
					}
					break;
				case 12:
					if (key == "%PlayerName%") {
						keyReplacement = this.GetPlayerName();
						goto label_22;
					}
					break;
				case 14:
					if (key == "%CurrentWorld%") {
						keyReplacement = this.GetCurrentWorld();
						goto label_22;
					}
					break;
			}
		}
		keyReplacement = string.Empty;
		label_22:
		return keyReplacement;
	}

	private string GetPlayerName() => ((IGameObject)this._client.LocalPlayer)?.Name.ToString() ?? "Unknown";

	private string GetCurrentWorld() {
		IPlayerCharacter localPlayer = this._client.LocalPlayer;
		string str;
		if (localPlayer == null) {
			str = null;
		} else {
			World world = localPlayer.CurrentWorld.Value;
			str = ((World) ref world).Name.ToString();
		}
		return str ?? "Unknown";
	}

	private string GetHomeWorld() {
		IPlayerCharacter localPlayer = this._client.LocalPlayer;
		string str;
		if (localPlayer == null) {
			str = null;
		} else {
			World world = localPlayer.HomeWorld.Value;
			str = ((World) ref world).Name.ToString();
		}
		return str ?? "Unknown";
	}

	private string GetZone() {
		ushort territoryType = this._client.TerritoryType;
		ExcelSheet<TerritoryType> excelSheet = this._data.GetExcelSheet<TerritoryType>(new ClientLanguage?(), (string)null);
		if (excelSheet.HasRow((uint)territoryType)) {
			TerritoryType row = excelSheet.GetRow((uint)territoryType);
			RowRef<PlaceName> placeName1 = ((TerritoryType) ref row ).PlaceName;
			if (placeName1.IsValid) {
				PlaceName placeName2 = placeName1.Value;
				ReadOnlySeString name = ((PlaceName) ref placeName2 ).Name;
				return ((ReadOnlySeString) ref name ).ExtractText();
			}
		}
		return "Unknown";
	}
}
