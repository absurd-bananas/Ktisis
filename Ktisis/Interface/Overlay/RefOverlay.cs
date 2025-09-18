// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Overlay.RefOverlay
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Ktisis.Core.Attributes;
using Ktisis.Data.Config;
using Ktisis.Scene.Entities.Utility;
using System;
using System.Numerics;

#nullable enable
namespace Ktisis.Interface.Overlay;

[Transient]
public class RefOverlay
{
  private readonly ConfigManager _cfg;
  private readonly ITextureProvider _tex;
  private static RefOverlay.CallbackData _data = new RefOverlay.CallbackData();

  public RefOverlay(ConfigManager cfg, ITextureProvider tex)
  {
    this._cfg = cfg;
    this._tex = tex;
  }

  public void DrawInstance(ReferenceImage image)
  {
    bool visible = image.Visible;
    IDalamudTextureWrap idalamudTextureWrap;
    Exception exception;
    if (!visible || !this._tex.GetFromFile(image.Data.FilePath).TryGetWrap(ref idalamudTextureWrap, ref exception))
      return;
    bool drawReferenceTitle = this._cfg.File.Overlay.DrawReferenceTitle;
    Dalamud.Bindings.ImGui.ImGui.SetNextWindowSize(idalamudTextureWrap.Size, (ImGuiCond) 4);
    RefOverlay.HandleImageAspectRatio(idalamudTextureWrap.Size, drawReferenceTitle);
    using (ImRaii.PushStyle((ImGuiStyleVar) 1, Vector2.Zero, true))
    {
      string id = $"{image.Name}###{image.Data.Id}";
      ImGuiWindowFlags imGuiWindowFlags = (ImGuiWindowFlags) 128 /*0x80*/;
      if (!drawReferenceTitle)
        imGuiWindowFlags = (ImGuiWindowFlags) (imGuiWindowFlags | 1);
      try
      {
        if (!Dalamud.Bindings.ImGui.ImGui.Begin(ImU8String.op_Implicit(id), ref visible, imGuiWindowFlags))
          return;
        Vector2 contentRegionAvail = Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail();
        Vector4 one = Vector4.One with
        {
          W = image.Data.Opacity
        };
        Dalamud.Bindings.ImGui.ImGui.Image(idalamudTextureWrap.Handle, contentRegionAvail, Vector2.Zero, Vector2.One, one);
        this.HandlePopup(id, contentRegionAvail, image);
      }
      finally
      {
        Dalamud.Bindings.ImGui.ImGui.End();
      }
      if (visible)
        return;
      image.Visible = false;
    }
  }

  private void HandlePopup(string id, Vector2 avail, ReferenceImage image)
  {
    string str = id + "##popup";
    if ((Dalamud.Bindings.ImGui.ImGui.IsItemClicked((ImGuiMouseButton) 1) ? 1 : (!Dalamud.Bindings.ImGui.ImGui.IsItemClicked((ImGuiMouseButton) 0) ? 0 : (Dalamud.Bindings.ImGui.ImGui.IsMouseDoubleClicked((ImGuiMouseButton) 0) ? 1 : 0))) != 0)
    {
      Dalamud.Bindings.ImGui.ImGui.OpenPopup(ImU8String.op_Implicit(str), (ImGuiPopupFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.SetNextWindowPos(Dalamud.Bindings.ImGui.ImGui.GetCursorScreenPos());
    }
    using (ImRaii.IEndObject iendObject = ImRaii.Popup(ImU8String.op_Implicit(str)))
    {
      if (!iendObject.Success)
        return;
      Dalamud.Bindings.ImGui.ImGui.SetNextItemWidth(avail.X);
      Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("##ref_opacity"), ref image.Data.Opacity, 0.0f, 1f, new ImU8String(), (ImGuiSliderFlags) 0);
    }
  }

  private static unsafe void HandleImageAspectRatio(Vector2 size, bool title)
  {
    if ((double) size.X == 0.0 || (double) size.Y == 0.0)
      return;
    float num = size.X / size.Y;
    ImGuiIOPtr io = Dalamud.Bindings.ImGui.ImGui.GetIO();
    Vector2 vector2_1 = ((ImGuiIOPtr) ref io).DisplaySize * 0.9f;
    Vector2 vector2_2 = new Vector2(vector2_1.Y * num, vector2_1.X / num);
    Vector2 vector2_3 = size * 0.1f;
    RefOverlay._data.Ratio = num;
    RefOverlay._data.Height = title ? Dalamud.Bindings.ImGui.ImGui.GetFrameHeight() : 0.0f;
    fixed (RefOverlay.CallbackData* callbackDataPtr1 = &RefOverlay._data)
    {
      Vector2 vector2_4 = vector2_2;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      ImGuiSizeCallback imGuiSizeCallback = RefOverlay.\u003C\u003EO.\u003C0\u003E__SetSizeCallback ?? (RefOverlay.\u003C\u003EO.\u003C0\u003E__SetSizeCallback = new ImGuiSizeCallback(RefOverlay.SetSizeCallback));
      RefOverlay.CallbackData* callbackDataPtr2 = callbackDataPtr1;
      Dalamud.Bindings.ImGui.ImGui.SetNextWindowSizeConstraints(vector2_3, vector2_4, imGuiSizeCallback, (void*) callbackDataPtr2);
    }
  }

  private static unsafe void SetSizeCallback(ImGuiSizeCallbackData* data)
  {
    if ((IntPtr) data == IntPtr.Zero)
      return;
    RefOverlay.CallbackData* userData = (RefOverlay.CallbackData*) data->UserData;
    if ((IntPtr) userData == IntPtr.Zero)
      return;
    data->DesiredSize.Y = userData->Height + data->DesiredSize.X / userData->Ratio;
  }

  private struct CallbackData
  {
    public float Ratio;
    public float Height;

    public CallbackData()
    {
      this.Ratio = 1f;
      this.Height = 0.0f;
    }
  }
}
