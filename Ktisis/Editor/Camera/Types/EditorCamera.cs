// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Camera.Types.EditorCamera
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using FFXIVClientStructs.FFXIV.Common.Math;
using Ktisis.Common.Utility;
using Ktisis.Editor.Context.Types;
using Ktisis.Scene.Entities;
using Ktisis.Scene.Entities.Game;
using Ktisis.Scene.Entities.Skeleton;
using Ktisis.Structs.Camera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Ktisis.Editor.Camera.Types;

public class EditorCamera
{
  protected readonly ICameraManager Manager;
  public string Name = string.Empty;
  public CameraFlags Flags;
  public ushort? OrbitTarget;
  public Vector3? FixedPosition;
  public Vector3 RelativeOffset = Vector3.Zero;
  public float OrthographicZoom = 10f;

  public virtual IntPtr Address { get; set; } = IntPtr.Zero;

  public EditorCamera(ICameraManager manager) => this.Manager = manager;

  public bool IsValid => this.Manager.IsValid && this.Address != IntPtr.Zero;

  public bool IsDefault => this.Flags.HasFlag((Enum) CameraFlags.DefaultCamera);

  public bool IsNoCollide => this.Flags.HasFlag((Enum) CameraFlags.NoCollide);

  public bool IsOrthographic => this.Flags.HasFlag((Enum) CameraFlags.Orthographic);

  public unsafe FFXIVClientStructs.FFXIV.Client.Game.Camera* GameCamera => (FFXIVClientStructs.FFXIV.Client.Game.Camera*) this.Address;

  public unsafe GameCameraEx* Camera => (GameCameraEx*) this.Address;

  public void SetActive() => this.SetOrthographic(this.IsOrthographic);

  public virtual unsafe Vector3? GetPosition()
  {
    FFXIVClientStructs.FFXIV.Client.Game.Camera* gameCamera = this.GameCamera;
    return (IntPtr) gameCamera == IntPtr.Zero ? new Vector3?() : new Vector3?(this.FixedPosition ?? Vector3.op_Implicit(gameCamera->CameraBase.SceneCamera.Object.Position));
  }

  public virtual unsafe void WritePosition()
  {
    FFXIVClientStructs.FFXIV.Client.Game.Camera* gameCamera = this.GameCamera;
    if ((IntPtr) gameCamera == IntPtr.Zero)
      return;
    FFXIVClientStructs.FFXIV.Client.Graphics.Scene.Camera* cameraPtr = &gameCamera->CameraBase.SceneCamera;
    Vector3 position = cameraPtr->Object.Position;
    Vector3 vector3 = Vector3.op_Implicit(this.GetPosition().Value + this.RelativeOffset);
    cameraPtr->Object.Position = vector3;
    ref Vector3 local = ref cameraPtr->LookAtVector;
    local = Vector3.op_Addition(local, Vector3.op_Subtraction(vector3, position));
    RenderCameraEx* renderEx = this.Camera->RenderEx;
    if ((IntPtr) renderEx == IntPtr.Zero || !this.IsOrthographic)
      return;
    renderEx->OrthographicZoom = this.OrthographicZoom;
  }

  public unsafe void SetDelimited(bool delimit)
  {
    if (delimit)
      this.Flags |= CameraFlags.Delimit;
    else
      this.Flags &= ~CameraFlags.Delimit;
    GameCameraEx* camera = this.Camera;
    if ((IntPtr) camera == IntPtr.Zero)
      return;
    float num = delimit ? 350f : 20f;
    camera->DistanceMax = num;
    camera->DistanceMin = delimit ? 0.0f : 1.5f;
    camera->Distance = System.Math.Clamp(camera->Distance, 0.0f, num);
    camera->YMin = delimit ? 1.5f : 1.25f;
    camera->YMax = delimit ? -1.5f : -1.4f;
  }

  public unsafe void SetOrthographic(bool enabled)
  {
    if (enabled)
      this.Flags |= CameraFlags.Orthographic;
    else
      this.Flags &= ~CameraFlags.Orthographic;
    RenderCameraEx* renderEx = this.Camera->RenderEx;
    if ((IntPtr) renderEx == IntPtr.Zero)
      return;
    renderEx->OrthographicEnabled = enabled;
    renderEx->OrthographicZoom = enabled ? this.OrthographicZoom : 10f;
  }

  public unsafe void ResetState()
  {
    if ((IntPtr) this.Camera == IntPtr.Zero)
      return;
    this.SetDelimited(false);
    this.SetOrthographic(false);
  }

  public unsafe void SetOffsetPositionToTarget(IEditorContext ctx, bool toPosePosition)
  {
    IGameObject target = ctx.Cameras.ResolveOrbitTarget(this);
    if (target == null)
      return;
    GameObject* address = (GameObject*) target.Address;
    DrawObject* drawObject = address->DrawObject;
    if ((IntPtr) drawObject == IntPtr.Zero)
      return;
    Vector3 vector3_1 = Vector3.op_Implicit(Vector3.op_Subtraction(drawObject->Object.Position, address->Position));
    if (toPosePosition)
    {
      IEnumerable<SceneEntity> source = ctx.Scene.Children.Where<SceneEntity>((Func<SceneEntity, bool>) (x => x is ActorEntity actorEntity1 && GameObjectId.op_Equality(((GameObject) (IntPtr) actorEntity1.CsGameObject).GetGameObjectId(), GameObjectId.op_Implicit(target.GameObjectId))));
      if (source.Any<SceneEntity>())
      {
        if (!(source.First<SceneEntity>() is ActorEntity actorEntity2))
        {
          this.RelativeOffset = vector3_1;
          return;
        }
        BoneNode boneByName1 = actorEntity2.Pose?.FindBoneByName("j_kosi");
        BoneNode boneByName2 = actorEntity2.Pose?.FindBoneByName("j_kao");
        BoneNode boneByName3 = actorEntity2.Pose?.FindBoneByName("j_asi_d_l");
        BoneNode boneByName4 = actorEntity2.Pose?.FindBoneByName("j_asi_d_r");
        if (boneByName1 == null || boneByName2 == null || boneByName3 == null || boneByName4 == null)
        {
          this.RelativeOffset = vector3_1;
          return;
        }
        Vector3 vector3_2 = this.GetPosition().Value;
        Transform transform1 = boneByName1.GetTransform();
        Vector3 vector3_3 = transform1 != null ? transform1.Position : new Vector3();
        Transform transform2 = boneByName2.GetTransform();
        Vector3 vector3_4 = transform2 != null ? transform2.Position : new Vector3();
        Transform transform3 = boneByName3.GetTransform();
        Vector3 vector3_5 = transform3 != null ? transform3.Position : new Vector3();
        Transform transform4 = boneByName4.GetTransform();
        Vector3 vector3_6 = transform4 != null ? transform4.Position : new Vector3();
        Vector3 vector3_7 = (vector3_5 + vector3_6) / 2f;
        double num1 = (double) System.Math.Abs(vector3_3.Y - vector3_7.Y);
        float num2 = System.Math.Abs(vector3_4.Y - vector3_7.Y);
        float num3 = System.Math.Abs(vector3_4.Y - vector3_3.Y);
        double num4 = (double) num2;
        float num5 = System.Math.Clamp(System.Math.Min((float) num1, (float) num4) + System.Math.Min(num2, num3), 0.7f, 1.5f);
        vector3_1 = new Vector3(vector3_3.X - address->Position.X, vector3_3.Y - address->Position.Y - num5, vector3_3.Z - address->Position.Z);
      }
    }
    this.RelativeOffset = vector3_1;
  }
}
