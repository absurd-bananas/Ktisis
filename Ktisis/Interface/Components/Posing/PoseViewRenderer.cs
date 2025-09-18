// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Posing.PoseViewRenderer
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Plugin.Services;
using Ktisis.Common.Extensions;
using Ktisis.Common.Utility;
using Ktisis.Data.Config;
using Ktisis.Data.Config.Pose2D;
using Ktisis.Editor.Selection;
using Ktisis.Interface.Components.Posing.Types;
using Ktisis.Scene.Entities;
using Ktisis.Scene.Entities.Game;
using Ktisis.Scene.Entities.Skeleton;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable
namespace Ktisis.Interface.Components.Posing;

public class PoseViewRenderer
{
  private readonly Configuration _cfg;
  private readonly ITextureProvider _tex;
  private readonly Dictionary<string, ISharedImmediateTexture> Textures = new Dictionary<string, ISharedImmediateTexture>();

  public PoseViewRenderer(Configuration cfg, ITextureProvider tex)
  {
    this._cfg = cfg;
    this._tex = tex;
  }

  public IViewFrame StartFrame() => (IViewFrame) new PoseViewRenderer.ViewFrame(this);

  public IDictionary<string, string> BuildTemplate(ActorEntity actor)
  {
    Dictionary<string, string> dictionary = new Dictionary<string, string>();
    char id;
    if (actor.TryGetEarIdAsChar(out id))
      dictionary.Add("$I", id.ToString());
    return (IDictionary<string, string>) dictionary;
  }

  private ISharedImmediateTexture GetTexture(string file)
  {
    ISharedImmediateTexture texture;
    if (this.Textures.TryGetValue(file, out texture))
      return texture;
    Assembly executingAssembly = Assembly.GetExecutingAssembly();
    string name = executingAssembly.GetName().Name;
    ISharedImmediateTexture manifestResource = this._tex.GetFromManifestResource(executingAssembly, $"{name}.Data.Images.{file}");
    this.Textures.Add(file, manifestResource);
    return manifestResource;
  }

  private class ViewFrame : IViewFrame
  {
    private readonly PoseViewRenderer _render;
    private readonly List<PoseViewRenderer.ViewData> Views = new List<PoseViewRenderer.ViewData>();

    public ViewFrame(PoseViewRenderer render) => this._render = render;

    public void DrawView(
      PoseViewEntry entry,
      float width = 1f,
      float height = 1f,
      IDictionary<string, string>? templates = null)
    {
      IDalamudTextureWrap wrapOrDefault = this._render.GetTexture(entry.Images.First<string>()).GetWrapOrDefault((IDalamudTextureWrap) null);
      if (wrapOrDefault == null)
        return;
      Vector2 contentRegionMax = Dalamud.Bindings.ImGui.ImGui.GetWindowContentRegionMax();
      ref float local = ref contentRegionMax.X;
      double num1 = (double) local;
      ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
      double num2 = (double) ((ImGuiStylePtr) ref style).ItemSpacing.X * (double) (this.Views.Count + 1);
      local = (float) (num1 - num2);
      Vector2 cursorScreenPos = Dalamud.Bindings.ImGui.ImGui.GetCursorScreenPos();
      float num3 = Math.Min(contentRegionMax.X * width / wrapOrDefault.Size.X, contentRegionMax.Y * height / wrapOrDefault.Size.Y);
      Vector2 vector2 = wrapOrDefault.Size * num3;
      Dalamud.Bindings.ImGui.ImGui.Image(wrapOrDefault.Handle, vector2);
      this.Views.Add(new PoseViewRenderer.ViewData()
      {
        Entry = entry,
        ScreenPos = cursorScreenPos,
        Size = vector2,
        Templates = templates
      });
    }

    public void DrawBones(EntityPose pose)
    {
      ImDrawListPtr windowDrawList = Dalamud.Bindings.ImGui.ImGui.GetWindowDrawList();
      bool flag1 = Dalamud.Bindings.ImGui.ImGui.IsWindowHovered();
      BoneNode boneNode = (BoneNode) null;
      foreach (PoseViewRenderer.ViewData view in this.Views)
      {
        bool flag2 = flag1 && Dalamud.Bindings.ImGui.ImGui.IsMouseHoveringRect(view.ScreenPos, view.ScreenPos + view.Size);
        foreach (PoseViewBone bone in view.Entry.Bones)
        {
          string name = bone.Name;
          if (view.Templates != null)
          {
            foreach (KeyValuePair<string, string> template in (IEnumerable<KeyValuePair<string, string>>) view.Templates)
            {
              string str1;
              string str2;
              template.Deconstruct(ref str1, ref str2);
              string oldValue = str1;
              string newValue = str2;
              name = name.Replace(oldValue, newValue);
            }
          }
          BoneNode boneByName = pose.FindBoneByName(name);
          if (boneByName != null)
          {
            Vector2 vector2_1 = view.Size * bone.Position;
            Vector2 vector2_2 = view.ScreenPos + vector2_1;
            float num = MathF.Max(MathF.Min(9f, view.Size.X * 0.04f), 6f);
            Vector2 vector2_3 = new Vector2(num, num);
            bool flag3 = flag2 && boneNode == null && Dalamud.Bindings.ImGui.ImGui.IsMouseHoveringRect(vector2_2 - vector2_3, vector2_2 + vector2_3);
            uint rgba = this._render._cfg.GetEntityDisplay((SceneEntity) boneByName).Color;
            if (!flag3 && !boneByName.IsSelected)
              rgba = rgba.SetAlpha((byte) 100);
            ((ImDrawListPtr) ref windowDrawList).AddCircleFilled(vector2_2, num, rgba, 64 /*0x40*/);
            ((ImDrawListPtr) ref windowDrawList).AddCircle(vector2_2, num, 4278190080U /*0xFF000000*/, 64 /*0x40*/, flag3 ? 2f : 1.5f);
            if (flag3)
              boneNode = boneByName;
          }
        }
      }
      if (boneNode == null)
        return;
      ImDrawListPtr foregroundDrawList = Dalamud.Bindings.ImGui.ImGui.GetForegroundDrawList();
      Vector2 vector2_4 = new Vector2(5f, 5f);
      Vector2 vector2_5 = Dalamud.Bindings.ImGui.ImGui.GetMousePos() + new Vector2(20f, 0.0f);
      ((ImDrawListPtr) ref foregroundDrawList).AddRectFilled(vector2_5 - vector2_4, vector2_5 + Dalamud.Bindings.ImGui.ImGui.CalcTextSize(ImU8String.op_Implicit(boneNode.Name), false, -1f) + vector2_4, 4278190080U /*0xFF000000*/, 5f);
      ((ImDrawListPtr) ref foregroundDrawList).AddText(vector2_5, uint.MaxValue, ImU8String.op_Implicit(boneNode.Name));
      if (!Dalamud.Bindings.ImGui.ImGui.IsMouseClicked((ImGuiMouseButton) 0))
        return;
      SelectMode selectMode = GuiHelpers.GetSelectMode();
      boneNode.Select(selectMode);
    }
  }

  private record ViewData()
  {
    public required PoseViewEntry Entry;
    public required Vector2 ScreenPos;
    public required Vector2 Size;
    public IDictionary<string, string>? Templates;

    [CompilerGenerated]
    protected virtual bool PrintMembers(StringBuilder builder)
    {
      RuntimeHelpers.EnsureSufficientExecutionStack();
      builder.Append("Entry = ");
      builder.Append((object) this.Entry);
      builder.Append(", ScreenPos = ");
      builder.Append(this.ScreenPos.ToString());
      builder.Append(", Size = ");
      builder.Append(this.Size.ToString());
      builder.Append(", Templates = ");
      builder.Append((object) this.Templates);
      return true;
    }

    [CompilerGenerated]
    public override int GetHashCode()
    {
      return (((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<PoseViewEntry>.Default.GetHashCode(this.Entry)) * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(this.ScreenPos)) * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(this.Size)) * -1521134295 + EqualityComparer<IDictionary<string, string>>.Default.GetHashCode(this.Templates);
    }

    [CompilerGenerated]
    public virtual bool Equals(PoseViewRenderer.ViewData? other)
    {
      if ((object) this == (object) other)
        return true;
      return (object) other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<PoseViewEntry>.Default.Equals(this.Entry, other.Entry) && EqualityComparer<Vector2>.Default.Equals(this.ScreenPos, other.ScreenPos) && EqualityComparer<Vector2>.Default.Equals(this.Size, other.Size) && EqualityComparer<IDictionary<string, string>>.Default.Equals(this.Templates, other.Templates);
    }

    [CompilerGenerated]
    [SetsRequiredMembers]
    protected ViewData(PoseViewRenderer.ViewData original)
    {
      this.Entry = original.Entry;
      this.ScreenPos = original.ScreenPos;
      this.Size = original.Size;
      this.Templates = original.Templates;
    }
  }
}
