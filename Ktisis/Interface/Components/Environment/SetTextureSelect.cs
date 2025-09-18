// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Environment.SetTextureSelect
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Ktisis.Core.Attributes;
using Ktisis.Interface.Widgets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Ktisis.Interface.Components.Environment;

[Transient]
public class SetTextureSelect
{
  private readonly ITextureProvider _texture;
  private static readonly Vector2 ButtonSize = new Vector2(48f, 48f);
  private static readonly Vector2 OptionSize = new Vector2(64f, 64f);
  private bool _opening;
  private SetTextureSelect.OptionsPopupResource? Options;

  public SetTextureSelect(ITextureProvider texture) => this._texture = texture;

  public bool Draw(string name, ref uint value, SetTextureSelect.ResolvePathHandler resolve)
  {
    ImU8String imU8String;
    // ISSUE: explicit constructor call
    ((ImU8String) ref imU8String).\u002Ector(12, 1);
    ((ImU8String) ref imU8String).AppendLiteral("##TexSelect_");
    ((ImU8String) ref imU8String).AppendFormatted<string>(name);
    using (ImRaii.PushId(imU8String, true))
    {
      bool flag1 = false;
      ISharedImmediateTexture fromGame = this._texture.GetFromGame(resolve(value));
      if (this.DrawButton(value, fromGame, SetTextureSelect.ButtonSize))
        this.OpenPopup(name, resolve);
      bool flag2 = flag1 | this.DrawPopup(name, ref value);
      Dalamud.Bindings.ImGui.ImGui.SameLine();
      using (ImRaii.Group())
      {
        Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(name));
        Dalamud.Bindings.ImGui.ImGui.SetNextItemWidth(Dalamud.Bindings.ImGui.ImGui.CalcItemWidth() - Dalamud.Bindings.ImGui.ImGui.GetCursorPosX());
        return flag2 | InputUInt.Draw("##" + name, ref value);
      }
    }
  }

  private bool DrawButton(uint value, ISharedImmediateTexture? image, Vector2 size)
  {
    using (ImRaii.PushColor((ImGuiCol) 21, 0U, true))
    {
      using (ImRaii.PushColor((ImGuiCol) 22, 1815755322U, true))
      {
        using (ImRaii.PushColor((ImGuiCol) 23, 2621061690U, true))
        {
          using (ImRaii.PushStyle((ImGuiStyleVar) 10, Vector2.Zero, true))
          {
            if (image != null)
              return Dalamud.Bindings.ImGui.ImGui.ImageButton(image.GetWrapOrEmpty().Handle, size);
            ImU8String imU8String = new ImU8String(0, 1);
            ((ImU8String) ref imU8String).AppendFormatted<uint>(value, "D3");
            return Dalamud.Bindings.ImGui.ImGui.Button(imU8String, size);
          }
        }
      }
    }
  }

  private void OpenPopup(string name, SetTextureSelect.ResolvePathHandler resolve)
  {
    this.Options?.Dispose();
    this.Options = (SetTextureSelect.OptionsPopupResource) null;
    this.Options = new SetTextureSelect.OptionsPopupResource();
    this.Options.Load(this._texture, resolve);
    ImU8String imU8String;
    // ISSUE: explicit constructor call
    ((ImU8String) ref imU8String).\u002Ector(6, 1);
    ((ImU8String) ref imU8String).AppendFormatted<string>(name);
    ((ImU8String) ref imU8String).AppendLiteral("_Popup");
    Dalamud.Bindings.ImGui.ImGui.OpenPopup(imU8String, (ImGuiPopupFlags) 0);
    this._opening = true;
  }

  private bool DrawPopup(string name, ref uint value)
  {
    if (this.Options == null)
      return false;
    ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
    Dalamud.Bindings.ImGui.ImGui.SetNextWindowSizeConstraints(Vector2.Zero, new Vector2((float) (((double) SetTextureSelect.OptionSize.X + (double) ((ImGuiStylePtr) ref style).ItemSpacing.X) * 6.0) + ((ImGuiStylePtr) ref style).ItemInnerSpacing.X + ((ImGuiStylePtr) ref style).ScrollbarSize, (float) (((double) SetTextureSelect.OptionSize.Y + (double) ((ImGuiStylePtr) ref style).ItemSpacing.Y) * 4.0) + ((ImGuiStylePtr) ref style).WindowPadding.Y));
    ImU8String imU8String;
    // ISSUE: explicit constructor call
    ((ImU8String) ref imU8String).\u002Ector(6, 1);
    ((ImU8String) ref imU8String).AppendFormatted<string>(name);
    ((ImU8String) ref imU8String).AppendLiteral("_Popup");
    using (ImRaii.IEndObject iendObject = ImRaii.Popup(imU8String, (ImGuiWindowFlags) 64 /*0x40*/))
    {
      if (!iendObject.Success)
      {
        if (this._opening)
          return false;
        this.Options?.Dispose();
        this.Options = (SetTextureSelect.OptionsPopupResource) null;
        return false;
      }
      this._opening = false;
      int num = 0;
      bool flag = false;
      foreach (SetTextureSelect.Option option in this.Options.Get())
      {
        if (num++ % 6 != 0 && num > 1)
          Dalamud.Bindings.ImGui.ImGui.SameLine();
        if (this.DrawButton(option.Value, option.Texture, SetTextureSelect.OptionSize))
        {
          value = option.Value;
          flag = true;
        }
      }
      return flag;
    }
  }

  public delegate string ResolvePathHandler(uint id);

  private class OptionsPopupResource : IDisposable
  {
    private readonly CancellationTokenSource Source = new CancellationTokenSource();
    private readonly List<SetTextureSelect.Option> List = new List<SetTextureSelect.Option>();

    public IEnumerable<SetTextureSelect.Option> Get()
    {
      lock (this.List)
        return (IEnumerable<SetTextureSelect.Option>) this.List.ToList<SetTextureSelect.Option>();
    }

    public void Load(ITextureProvider tex, SetTextureSelect.ResolvePathHandler resolve)
    {
      this.LoadAsync(Enumerable.Range(0, 1000).Select<int, uint>((Func<int, uint>) (i => (uint) i)).Select<uint, SetTextureSelect.Option>((Func<uint, SetTextureSelect.Option>) (i =>
      {
        ISharedImmediateTexture fromGame = tex.GetFromGame(resolve(i));
        if (fromGame == null && i > 0U)
          return (SetTextureSelect.Option) null;
        return new SetTextureSelect.Option()
        {
          Value = i,
          Texture = fromGame
        };
      })).Where<SetTextureSelect.Option>((Func<SetTextureSelect.Option, bool>) (opt => opt != null)).Cast<SetTextureSelect.Option>(), this.Source.Token).ContinueWith((Action<Task>) (task =>
      {
        if (task.Exception == null)
          return;
        Ktisis.Ktisis.Log.Error(task.Exception.ToString(), Array.Empty<object>());
      }));
    }

    private async Task LoadAsync(
      IEnumerable<SetTextureSelect.Option> values,
      CancellationToken token)
    {
      await Task.Yield();
      Stopwatch t = new Stopwatch();
      t.Start();
      foreach (SetTextureSelect.Option[] collection in Enumerable.Chunk<SetTextureSelect.Option>(values, 5))
      {
        double totalMilliseconds = t.Elapsed.TotalMilliseconds;
        lock (this.List)
        {
          if (!token.IsCancellationRequested)
            this.List.AddRange((IEnumerable<SetTextureSelect.Option>) collection);
          else
            break;
        }
        await Task.Delay(Math.Min((int) totalMilliseconds, 100), token);
        t.Restart();
      }
      token.ThrowIfCancellationRequested();
      t = (Stopwatch) null;
    }

    public void Dispose()
    {
      lock (this.List)
      {
        this.Source.Cancel();
        this.List.Clear();
      }
      this.Source.Dispose();
    }
  }

  private class Option
  {
    public required uint Value;
    public required ISharedImmediateTexture? Texture;
  }
}
