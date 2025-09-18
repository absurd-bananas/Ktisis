// Decompiled with JetBrains decompiler
// Type: Ktisis.Services.Plugin.CommandService
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game.Command;
using Dalamud.Plugin.Services;
using Ktisis.Core.Attributes;
using Ktisis.Editor.Context;
using Ktisis.Editor.Context.Types;
using Ktisis.Interface;
using System;
using System.Collections.Generic;

#nullable enable
namespace Ktisis.Services.Plugin;

[Singleton]
public class CommandService : IDisposable
{
  private readonly ICommandManager _cmd;
  private readonly IChatGui _chat;
  private readonly ContextManager _ctx;
  private readonly GuiManager _gui;
  private readonly HashSet<string> _register = new HashSet<string>();

  public CommandService(ICommandManager cmd, IChatGui chat, ContextManager ctx, GuiManager gui)
  {
    this._cmd = cmd;
    this._chat = chat;
    this._ctx = ctx;
    this._gui = gui;
  }

  public void RegisterHandlers()
  {
    // ISSUE: method pointer
    this.BuildCommand("/ktisispyon", new IReadOnlyCommandInfo.HandlerDelegate((object) this, __methodptr(OnMainCommand))).AddAlias("/ktisis").SetMessage("Toggle the main KtisisPyon window.").Create();
  }

  private void Add(string name, CommandInfo info)
  {
    if (!this._register.Add(name))
      return;
    this._cmd.AddHandler(name, info);
  }

  private CommandService.CommandFactory BuildCommand(
    string name,
    IReadOnlyCommandInfo.HandlerDelegate handler)
  {
    return new CommandService.CommandFactory(this, name, handler);
  }

  private void OnMainCommand(string command, string arguments)
  {
    Ktisis.Ktisis.Log.Info("Main command used", Array.Empty<object>());
    IEditorContext current = this._ctx.Current;
    if (current == null)
      this._chat.PrintError("Cannot open KtisisPyon workspace outside of GPose.", (string) null, new ushort?());
    else
      current.Interface.ToggleWorkspaceWindow();
  }

  public void Dispose()
  {
    foreach (string str in this._register)
      this._cmd.RemoveHandler(str);
  }

  private class CommandFactory
  {
    private readonly CommandService _cmd;
    private readonly string Name;
    private readonly List<string> Alias = new List<string>();
    private readonly IReadOnlyCommandInfo.HandlerDelegate Handler;
    private bool ShowInHelp;
    private string HelpMessage = string.Empty;

    public CommandFactory(
      CommandService cmd,
      string name,
      IReadOnlyCommandInfo.HandlerDelegate handler)
    {
      this._cmd = cmd;
      this.Name = name;
      this.Handler = handler;
    }

    public CommandService.CommandFactory SetMessage(string message)
    {
      this.ShowInHelp = true;
      this.HelpMessage = message;
      return this;
    }

    public CommandService.CommandFactory AddAlias(string alias)
    {
      this.Alias.Add(alias);
      return this;
    }

    public CommandService.CommandFactory AddAliases(params string[] aliases)
    {
      this.Alias.AddRange((IEnumerable<string>) aliases);
      return this;
    }

    public void Create()
    {
      this._cmd.Add(this.Name, this.BuildCommandInfo());
      this.Alias.ForEach(new Action<string>(this.CreateAlias));
    }

    private void CreateAlias(string alias)
    {
      this._cmd.Add(alias, new CommandInfo(this.Handler)
      {
        ShowInHelp = false
      });
    }

    private CommandInfo BuildCommandInfo()
    {
      string helpMessage = this.HelpMessage;
      if (this.HelpMessage != string.Empty && this.Alias.Count > 0)
      {
        string str = new string(' ', this.Name.Length * 2);
        helpMessage += $"\n{str} (Aliases: {string.Join(", ", (IEnumerable<string>) this.Alias)})";
      }
      return new CommandInfo(this.Handler)
      {
        ShowInHelp = this.ShowInHelp,
        HelpMessage = helpMessage
      };
    }
  }
}
