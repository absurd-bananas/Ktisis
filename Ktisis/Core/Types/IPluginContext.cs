// Decompiled with JetBrains decompiler
// Type: Ktisis.Core.Types.IPluginContext
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Actions;
using Ktisis.Data.Config;
using Ktisis.Editor.Context.Types;
using Ktisis.Interface;
using Ktisis.Interop.Ipc;

#nullable enable
namespace Ktisis.Core.Types;

public interface IPluginContext
{
  ActionService Actions { get; }

  ConfigManager Config { get; }

  GuiManager Gui { get; }

  IpcManager Ipc { get; }

  IEditorContext? Editor { get; }
}
