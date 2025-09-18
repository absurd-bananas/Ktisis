// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Chara.AnimationEditorTab
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

using GLib.Popups;
using GLib.Popups.Decorators;
using GLib.Widgets;

using Ktisis.Core.Attributes;
using Ktisis.Data.Config;
using Ktisis.Editor.Animation.Game;
using Ktisis.Editor.Animation.Types;
using Ktisis.Structs.Actors;

namespace Ktisis.Interface.Components.Chara;

[Transient]
public class AnimationEditorTab {
	private readonly static PoseModeEnum[] Modes = new PoseModeEnum[4] {
		PoseModeEnum.Idle,
		PoseModeEnum.SitGround,
		PoseModeEnum.SitChair,
		PoseModeEnum.Sleeping
	};
	private readonly GameAnimationData _animData;
	private readonly AnimationFilter _animFilter = new AnimationFilter();
	private readonly PopupList<GameAnimation> _animList;
	private readonly ConfigManager _cfg;
	private readonly ITextureProvider _tex;
	private bool _isSetup;
	private bool _openAnimList;
	private uint TimelineId;

	public AnimationEditorTab(ConfigManager cfg, IDataManager data, ITextureProvider tex) {
		this._cfg = cfg;
		this._tex = tex;
		this._animData = new GameAnimationData(data);
		// ISSUE: reference to a compiler-generated field
		// ISSUE: reference to a compiler-generated field
		this._animList = new PopupList<GameAnimation>("##AnimEmoteList", this.DrawAnimationSelect)
			.WithSearch(AnimationEditorTab.\u003C\u003EO.\u003C0\u003E__AnimSearchPredicate ?? (AnimationEditorTab.\u003C\u003EO.\u003C0\u003E__AnimSearchPredicate = new PopupList<GameAnimation>.SearchPredicate(AnimSearchPredicate)))
			.WithFilter(this._animFilter);
	}

	public IAnimationEditor Editor { set; private get; }

	private Configuration Config => this._cfg.File;

	private ref bool PlayEmoteStart => ref this.Config.Editor.PlayEmoteStart;

	private ref bool ForceLoop => ref this.Config.Editor.ForceLoop;

	public void Setup() {
		if (this._isSetup)
			return;
		this._isSetup = true;
		this._animData.Build().ContinueWith(task => {
			if (task.Exception == null)
				return;
			Ktisis.Ktisis.Log.Error($"Failed to fetch animations:\n{task.Exception}", Array.Empty<object>());
		});
	}

	public void Draw() => this.DrawAnimation();

	private static float CalcItemHeight() {
		var textLineHeight = (double)Dalamud.Bindings.ImGui.ImGui.GetTextLineHeight();
		ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		var y = (double)((ImGuiStylePtr) ref style ).ItemInnerSpacing.Y;
		return (float)((textLineHeight + y) * 2.0);
	}

	private void DrawAnimation() {
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		Vector2 contentRegionAvail = Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail();
		using (ImRaii.Child(ImU8String.op_Implicit("##animFrame"), contentRegionAvail with {
			X = contentRegionAvail.X * 0.35f
		})) {
			Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Animation"));
			this.DrawEmote();
			Dalamud.Bindings.ImGui.ImGui.Spacing();
			Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Idle Pose"));
			this.DrawPose();
		}
		Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, 0.0f);
		using (ImRaii.Child(ImU8String.op_Implicit("##tlFrame"), contentRegionAvail with {
			X = contentRegionAvail.X * 0.65f
		}))
			this.DrawTimelines();
		if (this._openAnimList) {
			this._openAnimList = false;
			this._animList.Open();
		}
		GameAnimation selected;
		if (!this._animList.Draw(this._animData.GetAll(), this._animData.Count, out selected, CalcItemHeight()))
			return;
		if (!this._animFilter.SlotFilterActive)
			this.TimelineId = selected.TimelineId;
		this.Editor.PlayAnimation(selected, this.PlayEmoteStart);
	}

	private void DrawEmote() {
		ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		float x = ((ImGuiStylePtr) ref style ).ItemInnerSpacing.X;
		if (Buttons.IconButton((FontAwesomeIcon)61442))
			this.OpenAnimationPopup();
		Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x);
		var timelineId = (int)this.TimelineId;
		if (Dalamud.Bindings.ImGui.ImGui.InputInt(ImU8String.op_Implicit("##emote"), ref timelineId, 0, 0, new ImU8String(), (ImGuiInputTextFlags)0))
			this.TimelineId = (uint)timelineId;
		if (Dalamud.Bindings.ImGui.ImGui.Button(ImU8String.op_Implicit("Play"), new Vector2()))
			this.PlayTimeline((uint)timelineId);
		Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x);
		if (Dalamud.Bindings.ImGui.ImGui.Button(ImU8String.op_Implicit("Reset"), new Vector2()))
			this.ResetTimeline();
		Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x);
		Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Loop"), ref this.ForceLoop);
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Play emote start"), ref this.PlayEmoteStart);
	}

	private void DrawPose() {
		PoseModeEnum mode1;
		int pose1;
		if (!this.Editor.TryGetModeAndPose(out mode1, out pose1))
			return;
		var x1 = (double)Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail().X;
		ImGuiStylePtr style1 = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		var num1 = (double)((ImGuiStylePtr) ref style1 ).ItemSpacing.X * 2.0;
		Dalamud.Bindings.ImGui.ImGui.SetNextItemWidth((float)(x1 - num1));
		if (Dalamud.Bindings.ImGui.ImGui.BeginCombo(ImU8String.op_Implicit("##Mode"), ImU8String.op_Implicit(mode1.ToString()), (ImGuiComboFlags)0)) {
			foreach (var mode2 in Modes) {
				if (Dalamud.Bindings.ImGui.ImGui.Selectable(ImU8String.op_Implicit(mode2.ToString()), mode2 == mode1, (ImGuiSelectableFlags)0, new Vector2()))
					this.Editor.SetPose(mode2, 0);
			}
			Dalamud.Bindings.ImGui.ImGui.EndCombo();
		}
		var x2 = (double)Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail().X;
		ImGuiStylePtr style2 = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		var num2 = (double)((ImGuiStylePtr) ref style2 ).ItemSpacing.X * 2.0;
		Dalamud.Bindings.ImGui.ImGui.SetNextItemWidth((float)(x2 - num2));
		if (Dalamud.Bindings.ImGui.ImGui.InputInt(ImU8String.op_Implicit("##Pose"), ref pose1, 1, 0, new ImU8String(), (ImGuiInputTextFlags)0)) {
			var poseCount = this.Editor.GetPoseCount(mode1);
			var pose2 = pose1 < 0 ? poseCount - 1 : pose1 % poseCount;
			this.Editor.SetPose(mode1, (byte)pose2);
		}
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		var isWeaponDrawn = this.Editor.IsWeaponDrawn;
		if (Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Weapon drawn"), ref isWeaponDrawn))
			this.Editor.ToggleWeapon();
		var positionLockEnabled = this.Editor.PositionLockEnabled;
		if (!Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Freeze positions"), ref positionLockEnabled))
			return;
		this.Editor.PositionLockEnabled = positionLockEnabled;
	}

	private unsafe void DrawTimelines() {
		var speedControlEnabled = this.Editor.SpeedControlEnabled;
		if (Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Enable speed control"), ref speedControlEnabled))
			this.Editor.SpeedControlEnabled = speedControlEnabled;
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		float x = ((ImGuiStylePtr) ref style ).ItemInnerSpacing.X;
		var timeline = this.Editor.GetTimeline();
		foreach (TimelineSlot timelineSlot in Enum.GetValues<TimelineSlot>()) {
			ImU8String imU8String1;
			// ISSUE: explicit constructor call
			((ImU8String) ref imU8String1).\u002Ector(9, 1);
			((ImU8String) ref imU8String1).AppendLiteral("timeline_");
			((ImU8String) ref imU8String1).AppendFormatted<TimelineSlot>(timelineSlot);
			using (ImRaii.PushId(imU8String1, true)) {
				var slot = (int)timelineSlot;
				if (Buttons.IconButton((FontAwesomeIcon)61761, new Vector2?(new Vector2(Dalamud.Bindings.ImGui.ImGui.GetFrameHeight(), Dalamud.Bindings.ImGui.ImGui.GetFrameHeight()))))
					this.OpenAnimationPopup(timelineSlot);
				Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x);
				var id = timeline.TimelineIds[slot];
				ActionTimeline? timelineById = this._animData.GetTimelineById(id);
				Dalamud.Bindings.ImGui.ImGui.SetNextItemWidth(40f);
				var num1 = (int)id;
				imU8String1 = new ImU8String(4, 1);
				((ImU8String) ref imU8String1).AppendLiteral("##id");
				((ImU8String) ref imU8String1).AppendFormatted<int>(slot);
				ImU8String imU8String2 = imU8String1;
				ref var local1 = ref num1;
				imU8String1 = new ImU8String();
				ImU8String imU8String3 = imU8String1;
				Dalamud.Bindings.ImGui.ImGui.InputInt(imU8String2, ref local1, 0, 0, imU8String3, (ImGuiInputTextFlags)16384 /*0x4000*/);
				Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x);
				var num2 = (float)((double)Dalamud.Bindings.ImGui.ImGui.CalcItemWidth() - (double)Dalamud.Bindings.ImGui.ImGui.GetFrameHeight() - 40.0);
				Dalamud.Bindings.ImGui.ImGui.SetNextItemWidth(num2);
				string str1;
				if (!timelineById.HasValue) {
					str1 = null;
				} else {
					ActionTimeline valueOrDefault = timelineById.GetValueOrDefault();
					ReadOnlySeString key = ((ActionTimeline) ref valueOrDefault ).Key;
					str1 = ((ReadOnlySeString) ref key).ExtractText();
				}
				if (str1 == null)
					str1 = string.Empty;
				var str2 = str1;
				using (ImRaii.Disabled(StringExtensions.IsNullOrEmpty(str2))) {
					imU8String1 = new ImU8String(3, 1);
					((ImU8String) ref imU8String1).AppendLiteral("##s");
					((ImU8String) ref imU8String1).AppendFormatted<int>(slot);
					Dalamud.Bindings.ImGui.ImGui.InputText(imU8String1, ref str2, 256 /*0x0100*/, (ImGuiInputTextFlags)16384 /*0x4000*/, (Dalamud.Bindings.ImGui.ImGui.ImGuiInputTextCallbackDelegate)null);
				}
				Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, 0.0f);
				ImU8String imU8String4 = ImU8String.op_Implicit("{0}");
				imU8String1 = new ImU8String(0, 1);
				((ImU8String) ref imU8String1).AppendFormatted<TimelineSlot>(timelineSlot);
				ImU8String imU8String5 = imU8String1;
				Dalamud.Bindings.ImGui.ImGui.LabelText(imU8String4, imU8String5);
				using (ImRaii.Disabled(!speedControlEnabled)) {
					var speed = timeline.TimelineSpeeds[slot];
					Dalamud.Bindings.ImGui.ImGui.SetNextItemWidth((float)((double)Dalamud.Bindings.ImGui.ImGui.GetFrameHeight() + x + 40.0));
					imU8String1 = new ImU8String(9, 1);
					((ImU8String) ref imU8String1).AppendLiteral("##speed_l");
					((ImU8String) ref imU8String1).AppendFormatted<int>(slot);
					ImU8String imU8String6 = imU8String1;
					ref var local2 = ref speed;
					imU8String1 = new ImU8String();
					ImU8String imU8String7 = imU8String1;
					var num3 = Dalamud.Bindings.ImGui.ImGui.InputFloat(imU8String6, ref local2, 0.0f, 0.0f, imU8String7, (ImGuiInputTextFlags)0) ? 1 : 0;
					Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x);
					Dalamud.Bindings.ImGui.ImGui.SetNextItemWidth(num2);
					imU8String1 = new ImU8String(9, 1);
					((ImU8String) ref imU8String1).AppendLiteral("##speed_r");
					((ImU8String) ref imU8String1).AppendFormatted<int>(slot);
					var num4 = Dalamud.Bindings.ImGui.ImGui.SliderFloat(imU8String1, ref speed, 0.0f, 2f, ImU8String.op_Implicit(""), (ImGuiSliderFlags)0) ? 1 : 0;
					if ((num3 | num4) != 0)
						this.Editor.SetTimelineSpeed((uint)slot, speed);
				}
				Dalamud.Bindings.ImGui.ImGui.Spacing();
			}
		}
	}

	private bool DrawAnimationSelect(GameAnimation anim, bool isFocus) {
		var num = CalcItemHeight();
		ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		float x = ((ImGuiStylePtr) ref style ).ItemInnerSpacing.X;
		float cursorPosX = Dalamud.Bindings.ImGui.ImGui.GetCursorPosX();
		bool flag = Dalamud.Bindings.ImGui.ImGui.Button(ImU8String.op_Implicit(string.Empty), new Vector2(Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail().X, num));
		Dalamud.Bindings.ImGui.ImGui.SameLine(cursorPosX, num + x);
		Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(anim.Name));
		Dalamud.Bindings.ImGui.ImGui.SameLine(cursorPosX, num + x);
		Dalamud.Bindings.ImGui.ImGui.SetCursorPosY(Dalamud.Bindings.ImGui.ImGui.GetCursorPosY() + Dalamud.Bindings.ImGui.ImGui.GetTextLineHeight());
		using (ImRaii.PushColor((ImGuiCol)0, Dalamud.Bindings.ImGui.ImGui.GetColorU32((ImGuiCol)0).SetAlpha((byte)175), true)) {
			ImU8String imU8String = new ImU8String(0, 1);
			((ImU8String) ref imU8String).AppendFormatted<TimelineSlot>(anim.Slot);
			Dalamud.Bindings.ImGui.ImGui.Text(imU8String);
		}
		Dalamud.Bindings.ImGui.ImGui.SameLine(cursorPosX);
		Vector2 vector2 = new Vector2(num, num);
		if (anim.Icon != 0) {
			ITextureProvider tex = this._tex;
			GameIconLookup gameIconLookup = GameIconLookup.op_Implicit((uint)anim.Icon);
			ref GameIconLookup local1 = ref gameIconLookup;
			ISharedImmediateTexture immediateTexture;
			ref ISharedImmediateTexture local2 = ref immediateTexture;
			if (tex.TryGetFromGameIcon(ref local1, ref local2)) {
				Dalamud.Bindings.ImGui.ImGui.Image(immediateTexture.GetWrapOrEmpty().Handle, vector2);
				goto label_9;
			}
		}
		Dalamud.Bindings.ImGui.ImGui.Dummy(vector2);
		label_9:
		return flag;
	}

	private void OpenAnimationPopup(TimelineSlot? slot = null) {
		var hasValue = slot.HasValue;
		this._animFilter.SlotFilterActive = hasValue;
		if (hasValue)
			this._animFilter.Slot = slot.Value;
		this._openAnimList = true;
	}

	private static bool AnimSearchPredicate(GameAnimation anim, string query) => anim.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase);

	private void PlayTimeline(uint id) {
		this.Editor.PlayTimeline(id);
		if (!this.ForceLoop)
			return;
		this.Editor.SetForceTimeline((ushort)id);
	}

	private void ResetTimeline() {
		this.Editor.PlayTimeline(3U);
		this.Editor.SetForceTimeline(0);
	}

	private class AnimationFilter : IFilterProvider<GameAnimation> {
		public TimelineSlot Slot;
		public bool SlotFilterActive;
		private AnimType Type;

		public bool DrawOptions() {
			var flag = false;
			AnimType[] values = Enum.GetValues<AnimType>();
			for (var index = 0; index < values.Length; ++index) {
				if (index % 3 != 0)
					Dalamud.Bindings.ImGui.ImGui.SameLine();
				var animType = values[index];
				ImU8String imU8String;
				// ISSUE: explicit constructor call
				((ImU8String) ref imU8String).\u002Ector(0, 1);
				((ImU8String) ref imU8String).AppendFormatted<AnimType>(animType);
				if (Dalamud.Bindings.ImGui.ImGui.RadioButton(imU8String, this.Type == animType)) {
					this.Type = animType;
					flag = true;
				}
			}
			Dalamud.Bindings.ImGui.ImGui.Spacing();
			return flag;
		}

		public bool Filter(GameAnimation item) {
			var flag1 = !this.SlotFilterActive || this.Slot == item.Slot;
			if (flag1) {
				bool flag2;
				switch (item) {
					case ActionAnimation _:
						flag2 = this.Type == AnimType.Action;
						break;
					case EmoteAnimation emoteAnimation:
						flag2 = this.Type == (emoteAnimation.IsExpression ? AnimType.Expression : AnimType.Emote);
						break;
					case TimelineAnimation _:
						flag2 = this.Type == AnimType.RawTimeline;
						break;
					default:
						flag2 = false;
						break;
				}
				flag1 = flag2;
			}
			return flag1;
		}

		private enum AnimType {
			Action,
			Emote,
			Expression,
			RawTimeline
		}
	}
}
