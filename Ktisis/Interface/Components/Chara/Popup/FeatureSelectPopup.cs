// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Chara.Popup.FeatureSelectPopup
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Ktisis.Editor.Characters.Make;
using Ktisis.Editor.Characters.Types;
using System.Numerics;

#nullable enable
namespace Ktisis.Interface.Components.Chara.Popup;

public class FeatureSelectPopup
{
  private readonly ITextureProvider _tex;
  private MakeTypeFeature? Feature;
  private const int MaxColumns = 6;
  private const int MaxRows = 3;
  private static readonly Vector2 ButtonSize = new Vector2(64f, 64f);
  private bool _isOpening;
  private bool _isOpen;

  private string PopupId => $"##FeatureSelect_{this.GetHashCode():X}";

  public FeatureSelectPopup(ITextureProvider tex) => this._tex = tex;

  public void Open(MakeTypeFeature feature)
  {
    this.Feature = feature;
    this._isOpening = true;
  }

  public void Draw(ICustomizeEditor editor)
  {
    if (this._isOpening)
    {
      this._isOpening = false;
      Dalamud.Bindings.ImGui.ImGui.OpenPopup(ImU8String.op_Implicit(this.PopupId), (ImGuiPopupFlags) 0);
    }
    if (!Dalamud.Bindings.ImGui.ImGui.IsPopupOpen(ImU8String.op_Implicit(this.PopupId), (ImGuiPopupFlags) 0))
      return;
    ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
    Vector2 zero = Vector2.Zero;
    double x = ((double) FeatureSelectPopup.ButtonSize.X + (double) ((ImGuiStylePtr) ref style).FramePadding.X * 2.0 + (double) ((ImGuiStylePtr) ref style).ItemSpacing.X) * 6.0 + (double) ((ImGuiStylePtr) ref style).ItemSpacing.X + (double) ((ImGuiStylePtr) ref style).ScrollbarSize;
    double num1 = (double) FeatureSelectPopup.ButtonSize.Y + ((double) ((ImGuiStylePtr) ref style).FramePadding.X + (double) ((ImGuiStylePtr) ref style).ItemSpacing.Y) * 2.0;
    ImFontPtr iconFont = UiBuilder.IconFont;
    double num2 = (double) ((ImFontPtr) ref iconFont).FontSize;
    double y = (num1 + num2) * 3.0 + (double) ((ImGuiStylePtr) ref style).WindowPadding.Y;
    Vector2 vector2 = new Vector2((float) x, (float) y);
    Dalamud.Bindings.ImGui.ImGui.SetNextWindowSizeConstraints(zero, vector2);
    using (ImRaii.IEndObject iendObject = ImRaii.Popup(ImU8String.op_Implicit(this.PopupId), (ImGuiWindowFlags) 64 /*0x40*/))
    {
      if (!iendObject.Success)
      {
        if (!this._isOpen)
          return;
        this.OnClose();
      }
      else
      {
        this._isOpen = true;
        this.DrawParams(editor);
      }
    }
  }

  private void DrawParams(ICustomizeEditor editor)
  {
    if (this.Feature == null)
      return;
    using (ImRaii.PushColor((ImGuiCol) 21, 0U, true))
    {
      bool flag1 = this.Feature.Index == 24;
      byte customization = editor.GetCustomization(this.Feature.Index);
      int num = 0;
      foreach (MakeTypeParam makeTypeParam in this.Feature.Params)
      {
        if (num++ % 6 != 0 && num > 1)
          Dalamud.Bindings.ImGui.ImGui.SameLine();
        using (ImRaii.Group())
        {
          ImU8String imU8String = new ImU8String(11, 2);
          ((ImU8String) ref imU8String).AppendLiteral("##Feature_");
          ((ImU8String) ref imU8String).AppendFormatted<byte>(makeTypeParam.Value);
          ((ImU8String) ref imU8String).AppendLiteral("_");
          ((ImU8String) ref imU8String).AppendFormatted<int>(num);
          using (ImRaii.PushId(imU8String, true))
          {
            ISharedImmediateTexture immediateTexture = (ISharedImmediateTexture) null;
            if (makeTypeParam.Graphic != 0U)
            {
              ITextureProvider tex = this._tex;
              GameIconLookup gameIconLookup = GameIconLookup.op_Implicit(makeTypeParam.Graphic);
              ref GameIconLookup local1 = ref gameIconLookup;
              ref ISharedImmediateTexture local2 = ref immediateTexture;
              tex.TryGetFromGameIcon(ref local1, ref local2);
            }
            Vector2 buttonSize = FeatureSelectPopup.ButtonSize;
            ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
            Vector2 vector2_1 = ((ImGuiStylePtr) ref style).FramePadding * 2f;
            Vector2 vector2_2 = buttonSize + vector2_1;
            bool flag2;
            if (immediateTexture != null)
            {
              flag2 = Dalamud.Bindings.ImGui.ImGui.ImageButton(immediateTexture.GetWrapOrEmpty().Handle, FeatureSelectPopup.ButtonSize);
              string str = makeTypeParam.Value.ToString();
              Dalamud.Bindings.ImGui.ImGui.SetCursorPosX(Dalamud.Bindings.ImGui.ImGui.GetCursorPosX() + (float) (((double) vector2_2.X - (double) Dalamud.Bindings.ImGui.ImGui.CalcTextSize(ImU8String.op_Implicit(str), false, -1f).X) / 2.0));
              Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(str));
            }
            else
            {
              imU8String = new ImU8String(0, 1);
              ((ImU8String) ref imU8String).AppendFormatted<byte>(makeTypeParam.Value);
              flag2 = Dalamud.Bindings.ImGui.ImGui.Button(imU8String, vector2_2);
            }
            if (flag2)
              editor.SetCustomization(this.Feature.Index, flag1 ? (byte) ((uint) makeTypeParam.Value | (uint) customization & 128U /*0x80*/) : makeTypeParam.Value);
          }
        }
      }
    }
  }

  private void OnClose()
  {
    this._isOpen = false;
    this.Feature = (MakeTypeFeature) null;
  }
}
