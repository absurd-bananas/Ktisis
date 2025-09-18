// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Posing.PoseViewRenderer
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

using Ktisis.Common.Extensions;
using Ktisis.Common.Utility;
using Ktisis.Data.Config;
using Ktisis.Interface.Components.Posing.Types;
using Ktisis.Scene.Entities.Game;
using Ktisis.Scene.Entities.Skeleton;

namespace Ktisis.Interface.Components.Posing;

public class PoseViewRenderer {
	private readonly Configuration _cfg;
	private readonly ITextureProvider _tex;
	private readonly Dictionary<string, ISharedImmediateTexture> Textures = new Dictionary<string, ISharedImmediateTexture>();
	public required PoseViewEntry Entry;
	public required Vector2 ScreenPos;
	public required Vector2 Size;
	public IDictionary<string, string>? Templates;

	public PoseViewRenderer(Configuration cfg, ITextureProvider tex) {
		this._cfg = cfg;
		this._tex = tex;
	}

	[CompilerGenerated]
	[SetsRequiredMembers]
	protected ViewData(PoseViewRenderer.ViewData original) {
		this.Entry = original.Entry;
		this.ScreenPos = original.ScreenPos;
		this.Size = original.Size;
		this.Templates = original.Templates;
	}

	public IViewFrame StartFrame() => new ViewFrame(this);

	public IDictionary<string, string> BuildTemplate(ActorEntity actor) {
		var dictionary = new Dictionary<string, string>();
		char id;
		if (actor.TryGetEarIdAsChar(out id))
			dictionary.Add("$I", id.ToString());
		return dictionary;
	}

	private ISharedImmediateTexture GetTexture(string file) {
		ISharedImmediateTexture texture;
		if (this.Textures.TryGetValue(file, out texture))
			return texture;
		var executingAssembly = Assembly.GetExecutingAssembly();
		var name = executingAssembly.GetName().Name;
		ISharedImmediateTexture manifestResource = this._tex.GetFromManifestResource(executingAssembly, $"{name}.Data.Images.{file}");
		this.Textures.Add(file, manifestResource);
		return manifestResource;
	}

	private record ViewData() {

	[CompilerGenerated]
	protected virtual bool PrintMembers(StringBuilder builder) {
		RuntimeHelpers.EnsureSufficientExecutionStack();
		builder.Append("Entry = ");
		builder.Append((object)this.Entry);
		builder.Append(", ScreenPos = ");
		builder.Append(this.ScreenPos.ToString());
		builder.Append(", Size = ");
		builder.Append(this.Size.ToString());
		builder.Append(", Templates = ");
		builder.Append(this.Templates);
		return true;
	}

	[CompilerGenerated]
	public override int GetHashCode() =>
		(((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<PoseViewEntry>.Default.GetHashCode(this.Entry)) * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(this.ScreenPos)) *
		-1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(this.Size)) * -1521134295 + EqualityComparer<IDictionary<string, string>>.Default.GetHashCode(this.Templates);

	[CompilerGenerated]
	public virtual bool Equals(PoseViewRenderer.ViewData? other) {
		if (this == (object)other)
			return true;
		return (object)other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<PoseViewEntry>.Default.Equals(this.Entry, other.Entry) && EqualityComparer<Vector2>.Default.Equals(this.ScreenPos, other.ScreenPos) &&
			EqualityComparer<Vector2>.Default.Equals(this.Size, other.Size) && EqualityComparer<IDictionary<string, string>>.Default.Equals(this.Templates, other.Templates);
	}

	private class ViewFrame : IViewFrame {
		private readonly PoseViewRenderer _render;
		private readonly List<PoseViewRenderer.ViewData> Views = new List<PoseViewRenderer.ViewData>();

		public ViewFrame(PoseViewRenderer render) {
			this._render = render;
		}

		public void DrawView(
			PoseViewEntry entry,
			float width = 1f,
			float height = 1f,
			IDictionary<string, string>? templates = null
		) {
			IDalamudTextureWrap wrapOrDefault = this._render.GetTexture(entry.Images.First<string>()).GetWrapOrDefault((IDalamudTextureWrap)null);
			if (wrapOrDefault == null)
				return;
			Vector2 contentRegionMax = Dalamud.Bindings.ImGui.ImGui.GetWindowContentRegionMax();
			ref float local = ref contentRegionMax.X;
			var num1 = (double)local;
			ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
			var num2 = (double)((ImGuiStylePtr) ref style ).ItemSpacing.X * (double)(this.Views.Count + 1);
			local = (float)(num1 - num2);
			Vector2 cursorScreenPos = Dalamud.Bindings.ImGui.ImGui.GetCursorScreenPos();
			var num3 = Math.Min(contentRegionMax.X * width / wrapOrDefault.Size.X, contentRegionMax.Y * height / wrapOrDefault.Size.Y);
			Vector2 vector2 = wrapOrDefault.Size * num3;
			Dalamud.Bindings.ImGui.ImGui.Image(wrapOrDefault.Handle, vector2);
			this.Views.Add(new PoseViewRenderer.ViewData {
				Entry = entry,
				ScreenPos = cursorScreenPos,
				Size = vector2,
				Templates = templates
			});
		}

		public void DrawBones(EntityPose pose) {
			ImDrawListPtr windowDrawList = Dalamud.Bindings.ImGui.ImGui.GetWindowDrawList();
			bool flag1 = Dalamud.Bindings.ImGui.ImGui.IsWindowHovered();
			var boneNode = (BoneNode)null;
			foreach (PoseViewRenderer.ViewData view in this.Views) {
				var flag2 = flag1 && Dalamud.Bindings.ImGui.ImGui.IsMouseHoveringRect(view.ScreenPos, view.ScreenPos + view.Size);
				foreach (PoseViewBone bone in view.Entry.Bones) {
					string name = bone.Name;
					if (view.Templates != null) {
						foreach (var template in (IEnumerable<KeyValuePair<string, string>>)view.Templates) {
							string str1;
							string str2;
							template.Deconstruct(ref str1, ref str2);
							var oldValue = str1;
							var newValue = str2;
							name = name.Replace(oldValue, newValue);
						}
					}
					var boneByName = pose.FindBoneByName(name);
					if (boneByName != null) {
						Vector2 vector2_1 = view.Size * bone.Position;
						Vector2 vector2_2 = view.ScreenPos + vector2_1;
						float num = MathF.Max(MathF.Min(9f, view.Size.X * 0.04f), 6f);
						Vector2 vector2_3 = new Vector2(num, num);
						var flag3 = flag2 && boneNode == null && Dalamud.Bindings.ImGui.ImGui.IsMouseHoveringRect(vector2_2 - vector2_3, vector2_2 + vector2_3);
						uint rgba = this._render._cfg.GetEntityDisplay(boneByName).Color;
						if (!flag3 && !boneByName.IsSelected)
							rgba = rgba.SetAlpha(100);
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
			if (!Dalamud.Bindings.ImGui.ImGui.IsMouseClicked((ImGuiMouseButton)0))
				return;
			var selectMode = GuiHelpers.GetSelectMode();
			boneNode.Select(selectMode);
		}
	}
}

}
