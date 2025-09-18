// Decompiled with JetBrains decompiler
// Type: Ktisis.Actions.Binds.KeybindInfo
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Data.Config.Actions;

#nullable enable
namespace Ktisis.Actions.Binds;

public class KeybindInfo
{
  public KeybindTrigger Trigger;
  public ActionKeybind Default = new ActionKeybind();
}
