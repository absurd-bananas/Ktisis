// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Characters.State.CustomizeState
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game.ClientState.Objects.Enums;
using Ktisis.Structs.Characters;

#nullable enable
namespace Ktisis.Editor.Characters.State;

public class CustomizeState
{
  private CustomizeContainer _container;
  private readonly bool[] _state = new bool[26];

  public byte this[CustomizeIndex index]
  {
    get => this._container[(uint) index];
    set
    {
      this._container[(uint) index] = value;
      this._state[index] = true;
    }
  }

  public void SetIfActive(CustomizeIndex index, byte value)
  {
    if (!this.IsSet(index))
      return;
    this[index] = value;
  }

  public bool IsSet(CustomizeIndex index) => this._state[index];

  public void Unset(CustomizeIndex index) => this._state[index] = false;
}
