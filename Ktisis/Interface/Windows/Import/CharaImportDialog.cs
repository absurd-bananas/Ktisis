// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Windows.Import.CharaImportDialog
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Editor.Context.Types;
using Ktisis.Interface.Components.Chara;
using Ktisis.Interface.Types;
using Ktisis.Scene.Entities.Game;

namespace Ktisis.Interface.Windows.Import;

public class CharaImportDialog : EntityEditWindow<ActorEntity> {
	private readonly IEditorContext _ctx;
	private readonly CharaImportUI _import;

	public CharaImportDialog(IEditorContext ctx, CharaImportUI import)
		: base("Import Appearance", ctx, (ImGuiWindowFlags)64 /*0x40*/) {
		this._ctx = ctx;
		this._import = import;
		this._import.Context = ctx;
		this._import.OnNpcSelected += this.OnNpcSelected;
	}

	public virtual void OnOpen() => this._import.Initialize();

	private void OnNpcSelected(CharaImportUI sender) => sender.ApplyTo(this.Target);

	public virtual void Draw() {
		this.UpdateTarget();
		ImU8String imU8String;
		// ISSUE: explicit constructor call
		((ImU8String) ref imU8String).\u002Ector(25, 1);
		((ImU8String) ref imU8String).AppendLiteral("Importing appearance for ");
		((ImU8String) ref imU8String).AppendFormatted<string>(this.Target.Name);
		Dalamud.Bindings.ImGui.ImGui.Text(imU8String);
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		this._import.DrawLoadMethods();
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		this._import.DrawImport();
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		this.DrawCharaApplication();
	}

	private void DrawCharaApplication() {
		this._import.DrawModesSelect();
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		using (ImRaii.Disabled(!this._import.HasSelection)) {
			if (Dalamud.Bindings.ImGui.ImGui.Button(ImU8String.op_Implicit("Apply"), new Vector2()))
				this._import.ApplyTo(this.Target);
			Dalamud.Bindings.ImGui.ImGui.Spacing();
		}
	}
}
