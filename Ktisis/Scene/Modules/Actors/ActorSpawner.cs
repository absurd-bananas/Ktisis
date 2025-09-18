// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Modules.Actors.ActorSpawner
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Common.Math;
using Ktisis.Interop.Hooking;
using Ktisis.Structs.Events;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Ktisis.Scene.Modules.Actors;

public class ActorSpawner : HookModule
{
  private const ushort Start = 200;
  private const ushort SoftCap = 30;
  private const ushort HardCap = 38;
  private readonly IObjectTable _objectTable;
  private readonly IFramework _framework;
  private const int VfSize = 9;
  [Signature("48 8D 05 ?? ?? ?? ?? 48 89 4A 20")]
  private unsafe IntPtr* _eventVfTable = (IntPtr*) null;
  [Signature("80 61 0C FC 48 8D 05 ?? ?? ?? ?? 4C 8B C9")]
  private ActorSpawner.GPoseActorEventCtorDelegate _gPoseActorEventCtor;
  [Signature("48 89 5C 24 ?? 48 89 54 24 ?? 57 48 83 EC 20 48 8B 02")]
  private ActorSpawner.DispatchEventDelegate _dispatchEvent;
  private unsafe IntPtr* _hookVfTable = (IntPtr*) null;
  private static ActorSpawner.FinalizeDelegate _finalizeOriginal;

  public ActorSpawner(IHookMediator hook, IObjectTable objectTable, IFramework framework)
    : base(hook)
  {
    this._objectTable = objectTable;
    this._framework = framework;
  }

  public void TryInitialize()
  {
    try
    {
      this.Initialize();
    }
    catch (Exception ex)
    {
      Ktisis.Ktisis.Log.Error($"Failed to initialize actor spawner:\n{ex}", Array.Empty<object>());
    }
  }

  protected override bool OnInitialize()
  {
    this.Setup();
    return true;
  }

  private unsafe void Setup()
  {
    IntPtr* numPtr = (IntPtr*) Marshal.AllocHGlobal(sizeof (IntPtr) * 9);
    for (int index = 0; index < 9; ++index)
    {
      IntPtr ptr = this._eventVfTable[index];
      if (index == 2)
      {
        ActorSpawner._finalizeOriginal = Marshal.GetDelegateForFunctionPointer<ActorSpawner.FinalizeDelegate>(ptr);
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        numPtr[index] = Marshal.GetFunctionPointerForDelegate<ActorSpawner.FinalizeDelegate>(ActorSpawner.\u003C\u003EO.\u003C0\u003E__FinalizeHook ?? (ActorSpawner.\u003C\u003EO.\u003C0\u003E__FinalizeHook = new ActorSpawner.FinalizeDelegate(ActorSpawner.FinalizeHook)));
      }
      else
        numPtr[index] = ptr;
    }
    this._hookVfTable = numPtr;
  }

  public async Task<IntPtr> CreateActor(IGameObject original)
  {
    IntPtr actor;
    using (CancellationTokenSource source = new CancellationTokenSource())
    {
      source.CancelAfter(10000);
      actor = await this.CreateActor(original, source.Token);
    }
    return actor;
  }

  private async Task<IntPtr> CreateActor(IGameObject original, CancellationToken token)
  {
    uint index = await this._framework.RunOnFrameworkThread<uint>((Func<uint>) (() =>
    {
      uint index1;
      if (!this.TryDispatch(original, out index1))
        throw new Exception("Object table is full.");
      return index1;
    }));
    while (!token.IsCancellationRequested)
    {
      IntPtr actor = await this._framework.RunOnFrameworkThread<IntPtr>((Func<IntPtr>) (() =>
      {
        IGameObject igameObject = this._objectTable[(int) index];
        return igameObject == null || !igameObject.IsValid() ? IntPtr.Zero : igameObject.Address;
      }));
      if (actor != IntPtr.Zero)
        return actor;
      await Task.Delay(10, CancellationToken.None);
    }
    throw new TaskCanceledException($"Actor spawn at index {index} timed out.");
  }

  private bool TryDispatch(IGameObject original, out uint index)
  {
    index = (uint) this.CalculateNextIndex();
    if (index == (uint) ushort.MaxValue)
      return false;
    Ktisis.Ktisis.Log.Info($"Dispatching, expecting spawn on {index}", Array.Empty<object>());
    this.DispatchSpawn(original);
    return true;
  }

  private unsafe void DispatchSpawn(IGameObject original)
  {
    if ((IntPtr) this._hookVfTable == IntPtr.Zero)
      throw new Exception("Hook vtable is not initialized!");
    FFXIVClientStructs.FFXIV.Client.Game.Character.Character* address = (FFXIVClientStructs.FFXIV.Client.Game.Character.Character*) original.Address;
    if ((IntPtr) address == IntPtr.Zero || !((GameObject) ref address->GameObject).IsCharacter())
      throw new Exception($"Original object '{original.Name}' ({original.ObjectIndex}) is invalid.");
    GPoseActorEvent* gposeActorEventPtr = (GPoseActorEvent*) ((IMemorySpace) (IntPtr) IMemorySpace.GetDefaultSpace()).Malloc<GPoseActorEvent>(8UL);
    IntPtr num1 = this._gPoseActorEventCtor(gposeActorEventPtr, address, &address->GameObject.Position, 64U /*0x40*/, 30, 0, 4294934523U, true);
    gposeActorEventPtr->__vfTable = this._hookVfTable;
    IntPtr num2 = this._dispatchEvent((IntPtr) EventFramework.Instance() + new IntPtr(432) + new IntPtr(152), gposeActorEventPtr);
  }

  private ushort CalculateNextIndex()
  {
    for (ushort nextIndex = 200; nextIndex <= (ushort) 238; ++nextIndex)
    {
      if (this._objectTable[(int) nextIndex] == null)
        return nextIndex;
    }
    return ushort.MaxValue;
  }

  private static unsafe void FinalizeHook(GPoseActorEvent* self, IntPtr a2, IntPtr a3)
  {
    if ((IntPtr) self->Character != IntPtr.Zero)
      self->EntityID = 3758096384UL /*0xE0000000*/;
    ActorSpawner._finalizeOriginal(self, a2, a3);
  }

  public override unsafe void Dispose()
  {
    base.Dispose();
    Ktisis.Ktisis.Log.Verbose("Disposing actor spawn manager...", Array.Empty<object>());
    if ((IntPtr) this._hookVfTable != IntPtr.Zero)
    {
      Ktisis.Ktisis.Log.Verbose("Freeing hookVfTable from spawn manager", Array.Empty<object>());
      Marshal.FreeHGlobal((IntPtr) this._hookVfTable);
      this._hookVfTable = (IntPtr*) null;
    }
    GC.SuppressFinalize((object) this);
  }

  private unsafe delegate IntPtr GPoseActorEventCtorDelegate(
    GPoseActorEvent* self,
    FFXIVClientStructs.FFXIV.Client.Game.Character.Character* target,
    Vector3* position,
    uint a4,
    int a5,
    int a6,
    uint a7,
    bool a8);

  private unsafe delegate IntPtr DispatchEventDelegate(IntPtr handler, GPoseActorEvent* task);

  private unsafe delegate void FinalizeDelegate(GPoseActorEvent* a1, IntPtr a2, IntPtr a3);
}
