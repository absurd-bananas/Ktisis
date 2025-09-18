// Decompiled with JetBrains decompiler
// Type: Ktisis.Services.Environment.WeatherInfo
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
namespace Ktisis.Services.Environment;

public class WeatherInfo {
	public readonly ISharedImmediateTexture? Icon;
	public readonly string Name;
	public readonly uint RowId;

	public WeatherInfo(Weather row, ISharedImmediateTexture? icon) {
		ReadOnlySeString name = ((Weather) ref row ).
		this.Name;
		string str = ((ReadOnlySeString) ref name ).ExtractText();
		if (StringExtensions.IsNullOrEmpty(str))
			str = $"Weather #{((Weather) ref
		row).
		this.RowId}";
		this.Name = str;
		this.RowId = ((Weather) ref row).
		this.RowId;
		this.Icon = icon;
	}
}
