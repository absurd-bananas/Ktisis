// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Env.EnvManagerEx
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.FFXIV.Client.Graphics.Environment;
using System.Runtime.InteropServices;

#nullable disable
namespace Ktisis.Structs.Env;

[StructLayout(LayoutKind.Explicit, Size = 2320)]
public struct EnvManagerEx
{
  [FieldOffset(0)]
  public EnvManager _base;
  [FieldOffset(88)]
  public EnvState EnvState;
  [FieldOffset(1248)]
  public EnvSimulator EnvSimulator;

  public static unsafe EnvManagerEx* Instance() => (EnvManagerEx*) EnvManager.Instance();
}
