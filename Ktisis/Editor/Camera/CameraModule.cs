// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Camera.CameraModule
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using FFXIVClientStructs.Interop;
using Ktisis.Editor.Actions.Input;
using Ktisis.Editor.Camera.Types;
using Ktisis.Interop.Hooking;
using Ktisis.Structs.Camera;
using Ktisis.Structs.Input;
using System;
using System.Numerics;

#nullable enable
namespace Ktisis.Editor.Camera;

public class CameraModule : HookModule
{
  private readonly CameraManager Manager;
  private readonly ISigScanner _sigScanner;
  private readonly IGameInteropProvider _interop;
  private readonly IObjectTable _objectTable;
  [Signature("E8 ?? ?? ?? ?? 48 8B 17 48 8D 4D E0")]
  private CameraModule.LoadMatrixDelegate _loadMatrix;
  [Signature("E8 ?? ?? ?? ?? 48 83 3D ?? ?? ?? ?? ?? 74 0C", DetourName = "CameraControlDetour")]
  private Hook<CameraModule.CameraControlDelegate> CameraControlHook;
  [Signature("48 83 EC 28 8B 41 48", DetourName = "CameraPreUpdateDetour")]
  private Hook<CameraModule.CameraPreUpdateDelegate> CameraPreUpdateHook;
  [Signature("48 89 5C 24 ?? 57 48 81 EC ?? ?? ?? ?? F6 81 ?? ?? ?? ?? ?? 48 8B D9 48 89 B4 24 ?? ?? ?? ??", DetourName = "CalcViewMatrixDetour")]
  private Hook<CameraModule.CalcViewMatrixDelegate> CalcViewMatrixHook;
  [Signature("E8 ?? ?? ?? ?? 83 7B 58 00", DetourName = "UpdateInputDetour")]
  private Hook<CameraModule.UpdateInputDelegate> UpdateInputHook;
  [Signature("E8 ?? ?? ?? ?? 4C 8D 45 97 89 83 ?? ?? ?? ??", DetourName = "CameraCollideDetour")]
  private Hook<CameraModule.CameraCollideDelegate> CameraCollideHook;
  [Signature("E8 ?? ?? ?? ?? 45 32 FF 40 32 F6", DetourName = "ActiveCameraDetour")]
  private Hook<CameraModule.ActiveCameraDelegate> ActiveCameraHook;
  [Signature("E8 ?? ?? ?? ?? 0F B6 F8 EB 34", DetourName = "CameraEventDetour")]
  private Hook<CameraModule.CameraEventDelegate> CameraEventHook;
  [Signature("E8 ?? ?? ?? ?? 80 BB ?? ?? ?? ?? ?? 74 0D 8B 53 28", DetourName = "CameraUiDetour")]
  private Hook<CameraModule.CameraUiDelegate> CameraUiHook;
  private Hook<CameraModule.CameraTargetDelegate>? CameraTargetHook;

  public CameraModule(
    IHookMediator hook,
    CameraManager manager,
    ISigScanner sigScanner,
    IGameInteropProvider interop,
    IObjectTable objectTable)
    : base(hook)
  {
    this.Manager = manager;
    this._sigScanner = sigScanner;
    this._interop = interop;
    this._objectTable = objectTable;
  }

  public override bool Initialize()
  {
    this.InitVfHook();
    return base.Initialize();
  }

  private unsafe void InitVfHook()
  {
    IntPtr num;
    if (!this._sigScanner.TryGetStaticAddressFromSig("48 8D 05 ?? ?? ?? ?? C7 83 ?? ?? ?? ?? ?? ?? ?? ?? 48 89 03 0F 57 C0 33 C0 48 C7 83 ?? ?? ?? ?? ?? ?? ?? ??", ref num, 0))
      Ktisis.Ktisis.Log.Warning("Failed to find signature for CameraTarget hook!", Array.Empty<object>());
    else
      this.CameraTargetHook = this._interop.HookFromAddress<CameraModule.CameraTargetDelegate>(*(IntPtr*) (num + new IntPtr(17) * sizeof (IntPtr)), new CameraModule.CameraTargetDelegate(this.CameraTargetDetour), (IGameInteropProvider.HookBackend) 0);
  }

  public void Setup()
  {
    if (!this.IsInit)
      return;
    this.CameraControlHook.Enable();
    this.CameraCollideHook.Enable();
    this.CameraTargetHook?.Enable();
  }

  public void ChangeCamera(EditorCamera camera)
  {
    if (!this.IsInit)
      return;
    bool flag = !camera.IsDefault;
    Ktisis.Ktisis.Log.Verbose($"Updating redirect hooks: {flag}", Array.Empty<object>());
    if (flag)
    {
      this.ActiveCameraHook.Enable();
      this.CameraEventHook.Enable();
      this.CameraUiHook.Enable();
      this.CameraPreUpdateHook.Enable();
    }
    else
    {
      this.ActiveCameraHook.Disable();
      this.CameraEventHook.Disable();
      this.CameraUiHook.Disable();
      this.CameraPreUpdateHook.Disable();
    }
    if (camera is WorkCamera)
    {
      this.CalcViewMatrixHook.Enable();
      this.UpdateInputHook.Enable();
    }
    else
    {
      this.CalcViewMatrixHook.Disable();
      this.UpdateInputHook.Disable();
    }
    CameraModule.SetSceneCamera(camera);
    camera.SetActive();
  }

  public unsafe IGameObject? ResolveOrbitTarget(EditorCamera camera)
  {
    if (camera.OrbitTarget.HasValue)
    {
      IGameObject igameObject = this._objectTable[(int) camera.OrbitTarget.Value];
      if (igameObject != null)
        return igameObject;
    }
    return this._objectTable.CreateObjectReference((IntPtr) TargetSystem.Instance()->GPoseTarget);
  }

  private IntPtr CameraControlDetour(IntPtr a1)
  {
    IntPtr num;
    using (this.Redirect())
      num = this.CameraControlHook.Original(a1);
    try
    {
      EditorCamera current = this.Manager.Current;
      if (current != null)
      {
        if (current.IsValid)
          current.WritePosition();
      }
    }
    catch (Exception ex)
    {
      Ktisis.Ktisis.Log.Error($"Failed to handle camera control:\n{ex}", Array.Empty<object>());
      this.DisableAll();
    }
    return num;
  }

  private IntPtr CameraPreUpdateDetour(IntPtr a1)
  {
    using (this.Redirect())
      return this.CameraPreUpdateHook.Original(a1);
  }

  private unsafe IntPtr CalcViewMatrixDetour(FFXIVClientStructs.FFXIV.Client.Graphics.Scene.Camera* camera)
  {
    IntPtr num = this.CalcViewMatrixHook.Original(camera);
    try
    {
      if (this.Manager.Current is WorkCamera current)
      {
        current.Update();
        Matrix4x4* matrix = (Matrix4x4*) &camera->ViewMatrix;
        *matrix = current.CalculateViewMatrix();
        Matrix4x4* matrix4x4Ptr = this._loadMatrix(current.Camera->RenderEx, matrix, 0, 0);
      }
    }
    catch (Exception ex)
    {
      Ktisis.Ktisis.Log.Error($"Failed to handle work camera:\n{ex}", Array.Empty<object>());
    }
    return num;
  }

  private unsafe void UpdateInputDetour(
    InputDeviceManager* mgr,
    IntPtr a2,
    void* controller,
    MouseDeviceData* mouseData,
    KeyboardDeviceData* keyData)
  {
    this.UpdateInputHook.Original(mgr, a2, controller, mouseData, keyData);
    try
    {
      if (!(this.Manager.Current is WorkCamera current) || InputManager.IsChatInputActive())
        return;
      current.UpdateControl(mouseData, keyData);
    }
    catch (Exception ex)
    {
      Ktisis.Ktisis.Log.Error($"Failed to handle work camera input:\n{ex}", Array.Empty<object>());
    }
  }

  private unsafe IntPtr CameraCollideDetour(
    FFXIVClientStructs.FFXIV.Client.Game.Camera* a1,
    Vector3* a2,
    Vector3* a3,
    float a4,
    IntPtr a5,
    float a6)
  {
    EditorCamera current = this.Manager.Current;
    if (current == null || !current.IsNoCollide || (IntPtr) current.Camera == IntPtr.Zero)
      return this.CameraCollideHook.Original(a1, a2, a3, a4, a5, a6);
    float num = a4 + 1f / 1000f;
    current.Camera->DistanceCollide.X = num;
    current.Camera->DistanceCollide.Y = num;
    return IntPtr.Zero;
  }

  private unsafe FFXIVClientStructs.FFXIV.Client.Game.Camera* ActiveCameraDetour(IntPtr a1)
  {
    EditorCamera current = this.Manager.Current;
    return current != null && current.IsValid ? current.GameCamera : this.ActiveCameraHook.Original(a1);
  }

  private char CameraEventDetour(IntPtr a1, IntPtr a2, int a3)
  {
    using (this.Redirect(a3 == 5))
      return this.CameraEventHook.Original(a1, a2, a3);
  }

  private void CameraUiDetour(IntPtr a1)
  {
    using (this.Redirect())
      this.CameraUiHook.Original(a1);
  }

  private IntPtr CameraTargetDetour(IntPtr a1)
  {
    EditorCamera current = this.Manager.Current;
    if (current != null)
    {
      ushort? orbitTarget = current.OrbitTarget;
      if (orbitTarget.HasValue)
      {
        IntPtr objectAddress = this._objectTable.GetObjectAddress((int) orbitTarget.GetValueOrDefault());
        if (objectAddress != IntPtr.Zero)
          return objectAddress;
      }
    }
    return this.CameraTargetHook.Original(a1);
  }

  private unsafe CameraModule.CameraRedirect Redirect(bool condition = true)
  {
    CameraManager* cameraManagerPtr = CameraManager.Instance();
    int activeCameraIndex = cameraManagerPtr->ActiveCameraIndex;
    FFXIVClientStructs.FFXIV.Client.Game.Camera** cameraPtr = (FFXIVClientStructs.FFXIV.Client.Game.Camera**) cameraManagerPtr;
    CameraModule.CameraRedirect cameraRedirect = new CameraModule.CameraRedirect(activeCameraIndex);
    if (!this.Manager.IsValid || !condition)
      return cameraRedirect;
    EditorCamera current = this.Manager.Current;
    if (current == null || current.IsDefault || (IntPtr) current.GameCamera == IntPtr.Zero)
      return cameraRedirect;
    cameraRedirect.Value = cameraPtr[activeCameraIndex];
    cameraPtr[activeCameraIndex] = current.GameCamera;
    return cameraRedirect;
  }

  private static unsafe void SetSceneCamera(EditorCamera camera)
  {
    CameraManager* cameraManagerPtr = CameraManager.Instance();
    ((CameraManager) (IntPtr) cameraManagerPtr).Cameras[cameraManagerPtr->CameraIndex] = Pointer<FFXIVClientStructs.FFXIV.Client.Graphics.Scene.Camera>.op_Implicit(&camera.GameCamera->CameraBase.SceneCamera);
  }

  private static unsafe void ResetSceneCamera()
  {
    CameraManager* cameraManagerPtr = CameraManager.Instance();
    FFXIVClientStructs.FFXIV.Client.Game.Camera* activeCamera = ((CameraManager) (IntPtr) CameraManager.Instance()).GetActiveCamera();
    ((CameraManager) (IntPtr) cameraManagerPtr).Cameras[cameraManagerPtr->CameraIndex] = Pointer<FFXIVClientStructs.FFXIV.Client.Graphics.Scene.Camera>.op_Implicit(&activeCamera->CameraBase.SceneCamera);
  }

  public override void Dispose()
  {
    base.Dispose();
    CameraModule.ResetSceneCamera();
    GC.SuppressFinalize((object) this);
  }

  private unsafe delegate Matrix4x4* LoadMatrixDelegate(
    RenderCameraEx* camera,
    Matrix4x4* matrix,
    int a3,
    int a4);

  private delegate IntPtr CameraControlDelegate(IntPtr a1);

  private delegate IntPtr CameraPreUpdateDelegate(IntPtr a1);

  private unsafe delegate IntPtr CalcViewMatrixDelegate(FFXIVClientStructs.FFXIV.Client.Graphics.Scene.Camera* camera);

  private unsafe delegate void UpdateInputDelegate(
    InputDeviceManager* mgr,
    IntPtr a2,
    void* controller,
    MouseDeviceData* mouseData,
    KeyboardDeviceData* keyData);

  private unsafe delegate IntPtr CameraCollideDelegate(
    FFXIVClientStructs.FFXIV.Client.Game.Camera* a1,
    Vector3* a2,
    Vector3* a3,
    float a4,
    IntPtr a5,
    float a6);

  private unsafe delegate FFXIVClientStructs.FFXIV.Client.Game.Camera* ActiveCameraDelegate(
    IntPtr a1);

  private delegate char CameraEventDelegate(IntPtr a1, IntPtr a2, int a3);

  private delegate void CameraUiDelegate(IntPtr a1);

  private delegate IntPtr CameraTargetDelegate(IntPtr a1);

  private class CameraRedirect(int index) : IDisposable
  {
    public unsafe FFXIVClientStructs.FFXIV.Client.Game.Camera* Value = (FFXIVClientStructs.FFXIV.Client.Game.Camera*) null;

    public unsafe void Dispose()
    {
      if ((IntPtr) this.Value == IntPtr.Zero)
        return;
      ((FFXIVClientStructs.FFXIV.Client.Game.Camera**) CameraManager.Instance())[index] = this.Value;
      this.Value = (FFXIVClientStructs.FFXIV.Client.Game.Camera*) null;
    }
  }
}
