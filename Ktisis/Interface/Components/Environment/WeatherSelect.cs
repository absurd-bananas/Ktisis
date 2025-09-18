// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Environment.WeatherSelect
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Ktisis.Core.Attributes;
using Ktisis.Services.Environment;
using Ktisis.Structs.Env;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Ktisis.Interface.Components.Environment;

[Transient]
public class WeatherSelect
{
  private static readonly Vector2 WeatherIconSize = new Vector2(28f, 28f);
  private readonly IClientState _clientState;
  private readonly WeatherService _weather;
  private readonly WeatherSelect.WeatherResource _resource;

  public WeatherSelect(IClientState clientState, WeatherService weather)
  {
    this._clientState = clientState;
    this._weather = weather;
    this._resource = new WeatherSelect.WeatherResource(weather);
  }

  public unsafe bool Draw(EnvManagerEx* env, out WeatherInfo? selected)
  {
    selected = (WeatherInfo) null;
    if ((IntPtr) env == IntPtr.Zero)
      return false;
    IEnumerable<WeatherInfo> weathers = this._resource.Get((uint) this._clientState.TerritoryType);
    byte activeWeather = env->_base.ActiveWeather;
    WeatherInfo weatherInfo = this._resource.Find((int) activeWeather);
    ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
    float num1 = ((ImGuiStylePtr) ref style).FramePadding.Y + WeatherSelect.WeatherIconSize.Y - Dalamud.Bindings.ImGui.ImGui.GetFrameHeight();
    using (ImRaii.PushStyle((ImGuiStyleVar) 10, ((ImGuiStylePtr) ref style).FramePadding with
    {
      Y = num1
    }, true))
    {
      int num2 = this.DrawWeatherCombo(activeWeather, weatherInfo, weathers, out selected) ? 1 : 0;
      if (weatherInfo != null)
        this.DrawWeatherLabel(weatherInfo, false);
      return num2 != 0;
    }
  }

  private bool DrawWeatherCombo(
    byte id,
    WeatherInfo? current,
    IEnumerable<WeatherInfo> weathers,
    out WeatherInfo? selected)
  {
    selected = (WeatherInfo) null;
    Dalamud.Bindings.ImGui.ImGui.SetNextItemWidth(Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail().X);
    using (ImRaii.IEndObject iendObject = ImRaii.Combo(ImU8String.op_Implicit("##WeatherCombo"), ImU8String.op_Implicit(current != null ? "##" : "Unknown")))
    {
      if (!iendObject.Success)
        return false;
      bool flag1 = false;
      foreach (WeatherInfo weather in weathers)
      {
        ImU8String imU8String = new ImU8String(13, 1);
        ((ImU8String) ref imU8String).AppendLiteral("##EnvWeather_");
        ((ImU8String) ref imU8String).AppendFormatted<uint>(weather.RowId);
        bool flag2 = Dalamud.Bindings.ImGui.ImGui.Selectable(imU8String, (int) weather.RowId == (int) id, (ImGuiSelectableFlags) 0, new Vector2());
        this.DrawWeatherLabel(weather, true);
        if (flag2)
          selected = weather;
        flag1 |= flag2;
      }
      return flag1;
    }
  }

  private void DrawWeatherLabel(WeatherInfo weather, bool adjustPad)
  {
    ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
    float frameHeight = Dalamud.Bindings.ImGui.ImGui.GetFrameHeight();
    if (weather.Icon != null)
    {
      Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, 0.0f);
      Dalamud.Bindings.ImGui.ImGui.SetCursorPosX(Dalamud.Bindings.ImGui.ImGui.GetCursorStartPos().X + ((ImGuiStylePtr) ref style).ItemInnerSpacing.X);
      float num = (float) ((double) Dalamud.Bindings.ImGui.ImGui.GetCursorPosY() + (double) frameHeight / 2.0 - (double) WeatherSelect.WeatherIconSize.Y / 2.0);
      if (adjustPad)
        num -= ((ImGuiStylePtr) ref style).FramePadding.Y;
      Dalamud.Bindings.ImGui.ImGui.SetCursorPosY(num);
      Dalamud.Bindings.ImGui.ImGui.Image(weather.Icon.GetWrapOrEmpty().Handle, WeatherSelect.WeatherIconSize);
      Dalamud.Bindings.ImGui.ImGui.SameLine();
    }
    Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(weather.Name));
  }

  private class WeatherResource(WeatherService service)
  {
    private uint TerritoryId;
    private readonly List<WeatherInfo> Cached = new List<WeatherInfo>();
    private CancellationTokenSource? TokenSource;

    public IEnumerable<WeatherInfo> Get(uint territory)
    {
      if ((int) territory != (int) this.TerritoryId)
      {
        this.TerritoryId = territory;
        this.Fetch();
      }
      lock (this.Cached)
        return (IEnumerable<WeatherInfo>) this.Cached;
    }

    public WeatherInfo? Find(int rowId)
    {
      lock (this.Cached)
        return this.Cached.Find((Predicate<WeatherInfo>) (row => (long) row.RowId == (long) rowId));
    }

    private void Fetch()
    {
      this.TokenSource?.Dispose();
      this.TokenSource = new CancellationTokenSource();
      service.GetWeatherTypes(this.TokenSource.Token).ContinueWith((Action<Task<IEnumerable<WeatherInfo>>>) (task =>
      {
        if (((Task) task).Exception != null)
        {
          Ktisis.Ktisis.Log.Error($"Failed to fetch weather:\n{((Task) task).Exception}", Array.Empty<object>());
        }
        else
        {
          lock (this.Cached)
          {
            this.Cached.Clear();
            this.Cached.AddRange(task.Result);
          }
        }
      }));
    }
  }
}
