// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Files.FileSelect`1
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using GLib.Widgets;
using Ktisis.Core.Attributes;
using System.IO;

#nullable enable
namespace Ktisis.Interface.Components.Files;

[Transient]
public class FileSelect<T> where T : notnull
{
  public FileSelect<
  #nullable disable
  T>.OpenDialogHandler
  #nullable enable
  ? OnOpenDialog;

  public bool IsFileOpened => this.Selected != null;

  public FileSelect<
  #nullable disable
  T>.FileSelectState
  #nullable enable
  ? Selected { get; private set; }

  public void SetFile(string path, T file)
  {
    this.Selected = new FileSelect<T>.FileSelectState()
    {
      Name = Path.GetFileName(path),
      Path = path,
      File = file
    };
  }

  public void Clear() => this.Selected = (FileSelect<T>.FileSelectState) null;

  public void Draw()
  {
    string str = this.Selected?.Name ?? "Select a file to open...";
    Dalamud.Bindings.ImGui.ImGui.InputText(ImU8String.op_Implicit("##FileSelectPath"), ref str, 256 /*0x0100*/, (ImGuiInputTextFlags) 16384 /*0x4000*/, (Dalamud.Bindings.ImGui.ImGui.ImGuiInputTextCallbackDelegate) null);
    Dalamud.Bindings.ImGui.ImGui.SameLine();
    if (Buttons.IconButton((FontAwesomeIcon) 62831))
    {
      FileSelect<T>.OpenDialogHandler onOpenDialog = this.OnOpenDialog;
      if (onOpenDialog != null)
        onOpenDialog(this);
    }
    using (ImRaii.Disabled(!this.IsFileOpened))
    {
      Dalamud.Bindings.ImGui.ImGui.SameLine();
      if (!Buttons.IconButton((FontAwesomeIcon) 62186))
        return;
      this.Selected = (FileSelect<T>.FileSelectState) null;
    }
  }

  public delegate void OpenDialogHandler(FileSelect<T> sender);

  public class FileSelectState
  {
    public string Name;
    public string Path;
    public T File;
  }
}
