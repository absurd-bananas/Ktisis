// Decompiled with JetBrains decompiler
// Type: Ktisis.Services.Plugin.CommandService
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;

using Ktisis.Core.Attributes;
using Ktisis.Editor.Context;
using Ktisis.Interface;

namespace Ktisis.Services.Plugin;

[Singleton]
public class CommandService : IDisposable {
	private readonly IChatGui _chat;
	private readonly ICommandManager _cmd;
	private readonly ContextManager _ctx;
	private readonly GuiManager _gui;
	private readonly HashSet<string> _register = new HashSet<string>();

	public CommandService(ICommandManager cmd, IChatGui chat, ContextManager ctx, GuiManager gui) {
		this._cmd = cmd;
		this._chat = chat;
		this._ctx = ctx;
		this._gui = gui;
	}

	public void Dispose() {
		foreach (var str in this._register)
			this._cmd.RemoveHandler(str);
	}

	public void RegisterHandlers() {
		// ISSUE: method pointer
		this.BuildCommand("/ktisispyon", new IReadOnlyCommandInfo.HandlerDelegate((object)this, __methodptr(OnMainCommand))).AddAlias("/ktisis").SetMessage("Toggle the main KtisisPyon window.").Create();
	}

	private void Add(string name, CommandInfo info) {
		if (!this._register.Add(name))
			return;
		this._cmd.AddHandler(name, info);
	}

	private CommandFactory BuildCommand(
		string name,
		IReadOnlyCommandInfo.HandlerDelegate handler
	) => new CommandFactory(this, name, handler);

	private void OnMainCommand(string command, string arguments) {
		Ktisis.Ktisis.Log.Info("Main command used", Array.Empty<object>());
		var current = this._ctx.Current;
		if (current == null)
			this._chat.PrintError("Cannot open KtisisPyon workspace outside of GPose.", (string)null, new ushort?());
		else
			current.Interface.ToggleWorkspaceWindow();
	}

	private class CommandFactory {
		private readonly CommandService _cmd;
		private readonly List<string> Alias = new List<string>();
		private readonly IReadOnlyCommandInfo.HandlerDelegate Handler;
		private readonly string Name;
		private string HelpMessage = string.Empty;
		private bool ShowInHelp;

		public CommandFactory(
			CommandService cmd,
			string name,
			IReadOnlyCommandInfo.HandlerDelegate handler
		) {
			this._cmd = cmd;
			this.Name = name;
			this.Handler = handler;
		}

		public CommandFactory SetMessage(string message) {
			this.ShowInHelp = true;
			this.HelpMessage = message;
			return this;
		}

		public CommandFactory AddAlias(string alias) {
			this.Alias.Add(alias);
			return this;
		}

		public CommandFactory AddAliases(params string[] aliases) {
			this.Alias.AddRange(aliases);
			return this;
		}

		public void Create() {
			this._cmd.Add(this.Name, this.BuildCommandInfo());
			this.Alias.ForEach(this.CreateAlias);
		}

		private void CreateAlias(string alias) {
			this._cmd.Add(alias, new CommandInfo(this.Handler) {
				ShowInHelp = false
			});
		}

		private CommandInfo BuildCommandInfo() {
			var helpMessage = this.HelpMessage;
			if (this.HelpMessage != string.Empty && this.Alias.Count > 0) {
				var str = new string(' ', this.Name.Length * 2);
				helpMessage += $"\n{str} (Aliases: {string.Join(", ", this.Alias)})";
			}
			return new CommandInfo(this.Handler) {
				ShowInHelp = this.ShowInHelp,
				HelpMessage = helpMessage
			};
		}
	}
}
