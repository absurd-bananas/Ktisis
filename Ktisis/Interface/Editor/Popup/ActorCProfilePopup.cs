// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Editor.Popup.ActorCProfilePopup
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

using GLib.Lists;

using Ktisis.Editor.Context.Types;
using Ktisis.Interface.Types;
using Ktisis.Interop.Ipc;
using Ktisis.Scene.Entities.Game;

namespace Ktisis.Interface.Editor.Popup;

public class ActorCProfilePopup : KtisisPopup {
	private readonly IEditorContext _ctx;
	private readonly ActorEntity _entity;
	private readonly CustomizeIpcProvider _ipc;
	private readonly ListBox<(Guid UniqueId, string Name, string VirtualPath, List<(string Name, ushort WorldId, byte CharacterType, ushort CharacterSubType)> Characters, int Priority, bool IsEnabled)> _list;
	private (Guid Id, string Name) _current = (Guid.Empty, string.Empty);
	private bool _isOpening = true;
	private List<(Guid UniqueId, string Name, string VirtualPath, List<(string Name, ushort WorldId, byte CharacterType, ushort CharacterSubType)> Characters, int Priority, bool IsEnabled)> _profiles =
		new List<(Guid, string, string, List<(string, ushort, byte, ushort)>, int, bool)>();

	public ActorCProfilePopup(IEditorContext ctx, ActorEntity entity)
		: base("##ActorCProfilePopup") {
		this._ctx = ctx;
		this._entity = entity;
		this._ipc = ctx.Plugin.Ipc.GetCustomizeIpc();
		this._list = new ListBox<(Guid, string, string, List<(string, ushort, byte, ushort)>, int, bool)>("##CProfileList", this.DrawItem);
	}

	protected override void OnDraw() {
		if (!this._entity.IsValid || !this._ctx.Plugin.Ipc.IsCustomizeActive) {
			this.Close();
		} else {
			if (this._isOpening) {
				this._isOpening = false;
				this._profiles = this._ipc.GetProfileList().ToList<(Guid, string, string, List<(string, ushort, byte, ushort)>, int, bool)>();
				Ktisis.Ktisis.Log.Info($"Fetched {this._profiles.Count} profiles", Array.Empty<object>());
			}
			var id = this._ipc.GetActiveProfileId(this._entity.Actor.ObjectIndex).Id;
			if (id.HasValue) {
				foreach (var profile in this._profiles) {
					var uniqueId = profile.UniqueId;
					var nullable = id;
					if ((nullable.HasValue ? uniqueId != nullable.GetValueOrDefault() ? 1 : 0 : 1) == 0)
						this._current = (profile.UniqueId, profile.Name);
				}
			}
			ImU8String imU8String;
			// ISSUE: explicit constructor call
			((ImU8String) ref imU8String).\u002Ector(25, 1);
			((ImU8String) ref imU8String).AppendLiteral("Assigning collection for ");
			((ImU8String) ref imU8String).AppendFormatted<string>(this._entity.Name);
			Dalamud.Bindings.ImGui.ImGui.Text(imU8String);
			// ISSUE: explicit constructor call
			((ImU8String) ref imU8String).\u002Ector(18, 1);
			((ImU8String) ref imU8String).AppendLiteral("Currently set to: ");
			((ImU8String) ref imU8String).AppendFormatted<string>(this._current.Name);
			Dalamud.Bindings.ImGui.ImGui.TextDisabled(imU8String);
			(Guid UniqueId, string Name, string VirtualPath, List<(string Name, ushort WorldId, byte CharacterType, ushort CharacterSubType)> Characters, int Priority, bool IsEnabled) selected;
			if (!this._list.Draw(this._profiles, out selected))
				return;
			(int, string Data) profileByUniqueId = this._ipc.GetProfileByUniqueId(selected.UniqueId);
			if (profileByUniqueId.Data == null)
				return;
			this.SetProfile(profileByUniqueId.Data);
		}
	}

	private void SetProfile(string data) {
		ushort objectIndex = this._entity.Actor.ObjectIndex;
		this._ipc.DeleteTemporaryProfile(objectIndex);
		this._ipc.SetTemporaryProfile(objectIndex, data);
		if (this._ctx.Posing.IsEnabled)
			return;
		this._entity.Redraw();
	}

	private bool DrawItem(
		(Guid UniqueId, string Name, string VirtualPath, List<(string Name, ushort WorldId, byte CharacterType, ushort CharacterSubType)> Characters, int Priority, bool IsEnabled) item,
		bool _
	) => Dalamud.Bindings.ImGui.ImGui.Selectable(ImU8String.op_Implicit(item.Name), item.UniqueId == this._current.Id, (ImGuiSelectableFlags)0, new Vector2());
}
