// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Editor.Popup.ActorCollectionPopup
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

public class ActorCollectionPopup : KtisisPopup {
	private readonly IEditorContext _ctx;
	private readonly ActorEntity _entity;
	private readonly PenumbraIpcProvider _ipc;
	private readonly ListBox<KeyValuePair<Guid, string>> _list;
	private (Guid Id, string Name) _current = (Guid.Empty, string.Empty);

	public ActorCollectionPopup(IEditorContext ctx, ActorEntity entity)
		: base("##ActorCollectionPopup") {
		this._ctx = ctx;
		this._entity = entity;
		this._ipc = ctx.Plugin.Ipc.GetPenumbraIpc();
		this._list = new ListBox<KeyValuePair<Guid, string>>("##CollectionList", this.DrawItem);
	}

	protected override void OnDraw() {
		if (!this._entity.IsValid || !this._ctx.Plugin.Ipc.IsPenumbraActive) {
			this.Close();
		} else {
			this._current = this._ipc.GetCollectionForObject(this._entity.Actor);
			ImU8String imU8String;
			// ISSUE: explicit constructor call
			((ImU8String) ref imU8String).\u002Ector(25, 1);
			((ImU8String) ref imU8String).AppendLiteral("Assigning collection for ");
			((ImU8String) ref imU8String).AppendFormatted<string>(this._entity.Name);
			Dalamud.Bindings.ImGui.ImGui.Text(imU8String);
			// ISSUE: explicit constructor call
			((ImU8String) ref imU8String).\u002Ector(18, 1);
			((ImU8String) ref imU8String).AppendLiteral("Currently set to: ");
			((ImU8String) ref imU8String).AppendFormatted<(Guid, string)>(this._current);
			Dalamud.Bindings.ImGui.ImGui.TextDisabled(imU8String);
			KeyValuePair<Guid, string> selected;
			if (!this._list.Draw(this._ipc.GetCollections().ToList(), out selected) || !this._ipc.SetCollectionForObject(this._entity.Actor, selected.Key))
				return;
			this._entity.Redraw();
		}
	}

	private bool DrawItem(KeyValuePair<Guid, string> item, bool _) => Dalamud.Bindings.ImGui.ImGui.Selectable(ImU8String.op_Implicit(item.Value), item.Key == this._current.Id, (ImGuiSelectableFlags)0, new Vector2());
}
