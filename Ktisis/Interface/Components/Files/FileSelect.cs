// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Files.FileSelect`1
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System.IO;

using GLib.Widgets;

using Ktisis.Core.Attributes;

namespace Ktisis.Interface.Components.Files;

[Transient]
public class FileSelect<T> where T : notnull {
	public OpenDialogHandler
		#nullable enable
		? OnOpenDialog;

	public bool IsFileOpened => this.Selected != null;

	public FileSelectState
		#nullable enable
		? Selected { get; private set; }

	public void SetFile(string path, T file) {
		this.Selected = new FileSelectState {
			Name = Path.GetFileName(path),
			Path = path,
			File = file
		};
	}

	public void Clear() => this.Selected = null;

	public void Draw() {
		var str = this.Selected?.Name ?? "Select a file to open...";
		Dalamud.Bindings.ImGui.ImGui.InputText(ImU8String.op_Implicit("##FileSelectPath"), ref str, 256 /*0x0100*/, (ImGuiInputTextFlags)16384 /*0x4000*/, (Dalamud.Bindings.ImGui.ImGui.ImGuiInputTextCallbackDelegate)null);
		Dalamud.Bindings.ImGui.ImGui.SameLine();
		if (Buttons.IconButton((FontAwesomeIcon)62831)) {
			var onOpenDialog = this.OnOpenDialog;
			if (onOpenDialog != null)
				onOpenDialog(this);
		}
		using (ImRaii.Disabled(!this.IsFileOpened)) {
			Dalamud.Bindings.ImGui.ImGui.SameLine();
			if (!Buttons.IconButton((FontAwesomeIcon)62186))
				return;
			this.Selected = null;
		}
	}

	public delegate void OpenDialogHandler(FileSelect<T> sender);

	public class FileSelectState {
		public T File;
		public string Name;
		public string Path;
	}
}
