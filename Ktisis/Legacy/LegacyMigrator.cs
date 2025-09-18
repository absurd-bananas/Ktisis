// Decompiled with JetBrains decompiler
// Type: Ktisis.Legacy.LegacyMigrator
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Core.Attributes;
using Ktisis.Interface;
using Ktisis.Legacy.Interface;
using Ktisis.Services.Game;
using System;

#nullable enable
namespace Ktisis.Legacy;

[Singleton]
public class LegacyMigrator
{
  private readonly GPoseService _gpose;
  private readonly GuiManager _gui;
  private bool _confirmed;

  public event Action? OnConfirmed;

  public LegacyMigrator(GPoseService gpose, GuiManager gui)
  {
    this._gpose = gpose;
    this._gui = gui;
  }

  public void Setup()
  {
    Ktisis.Ktisis.Log.Warning("User is migrating from Ktisis v0.2, activating legacy mode.", Array.Empty<object>());
    this._gpose.StateChanged += new GPoseStateHandler(this.OnGPoseStateChanged);
    this._gpose.Subscribe();
  }

  private void OnGPoseStateChanged(object sender, bool state)
  {
    if (!state || this._confirmed)
      return;
    this._gui.GetOrCreate<MigratorWindow>((object) this).Open();
  }

  public void Begin()
  {
    if (this._confirmed)
      return;
    this._confirmed = true;
    this._gpose.StateChanged -= new GPoseStateHandler(this.OnGPoseStateChanged);
    this._gpose.Reset();
    Action onConfirmed = this.OnConfirmed;
    if (onConfirmed == null)
      return;
    onConfirmed();
  }
}
