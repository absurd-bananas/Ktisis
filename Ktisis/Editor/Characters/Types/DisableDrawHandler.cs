// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Characters.Types.DisableDrawHandler
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;

#nullable enable
namespace Ktisis.Editor.Characters.Types;

public unsafe delegate void DisableDrawHandler(IGameObject gameObject, DrawObject* drawObject);
