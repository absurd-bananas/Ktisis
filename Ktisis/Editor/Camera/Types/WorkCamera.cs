// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Camera.Types.WorkCamera
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game.ClientState.Keys;
using Ktisis.Editor.Context.Types;
using Ktisis.Structs.Input;
using System;
using System.Numerics;

#nullable enable
namespace Ktisis.Editor.Camera.Types;

public class WorkCamera : KtisisCamera
{
  private readonly IEditorContext _ctx;
  private static readonly Vector3 UpVector = Vector3.UnitY;
  private const float ClampY = 1.57072f;
  public Vector3 Position;
  public Vector3 Rotation;
  private float MoveSpeed;
  private float MoveSpeedModifier = 1f;
  private Vector3 Velocity;
  private Vector2 MouseDelta;
  private Vector3 InterpPos;
  private DateTime LastTime;

  public WorkCamera(IEditorContext ctx, ICameraManager manager)
    : base(manager)
  {
    this._ctx = ctx;
  }

  public void SetInitialPosition(Vector3 pos, Vector3 rot)
  {
    this.Position = pos;
    this.InterpPos = pos;
    this.Rotation = rot;
    this.MoveSpeedModifier = 1f;
  }

  public unsafe void UpdateControl(MouseDeviceData* mouseData, KeyboardDeviceData* keyData)
  {
    bool leftHeld = false;
    bool rightHeld = false;
    int scrollDelta = 0;
    if ((IntPtr) mouseData != IntPtr.Zero)
      this.UpdateMouse(mouseData, out leftHeld, out rightHeld, out scrollDelta);
    if ((IntPtr) keyData == IntPtr.Zero)
      return;
    this.UpdateKeyboard(keyData, leftHeld, rightHeld, scrollDelta);
  }

  private unsafe void UpdateMouse(
    MouseDeviceData* mouseData,
    out bool leftHeld,
    out bool rightHeld,
    out int scrollDelta)
  {
    Vector2 delta = mouseData->GetDelta(false);
    leftHeld = mouseData->IsButtonHeld(MouseButton.Left);
    rightHeld = mouseData->IsButtonHeld(MouseButton.Right);
    if (rightHeld)
      this.MouseDelta += delta;
    scrollDelta = mouseData->ScrollDelta;
    if (!this._ctx.Config.Camera.ScrollWheelAdjustsSpeed || this._ctx.Cameras.Current == null || !(this._ctx.Cameras.Current is WorkCamera))
      return;
    mouseData->ScrollDelta = 0;
  }

  private unsafe void UpdateKeyboard(
    KeyboardDeviceData* keyData,
    bool leftHeld,
    bool rightHeld,
    int scrollDelta)
  {
    this.MoveSpeed = this._ctx.Config.Camera.DefaultWorkCamSpeed;
    if (keyData->IsKeyDown((VirtualKey) 16 /*0x10*/, false))
      this.MoveSpeed *= 5f;
    else if (keyData->IsKeyDown((VirtualKey) 17, false))
      this.MoveSpeed *= 0.25f;
    if (this._ctx.Config.Camera.ScrollWheelAdjustsSpeed)
    {
      if (scrollDelta != 0)
        this.MoveSpeedModifier = Math.Clamp((float) ((double) this.MoveSpeedModifier * 1.0 + 0.039999999105930328 * (double) scrollDelta), 0.01f, 5f);
      this.MoveSpeed *= this.MoveSpeedModifier;
    }
    int num1 = 0;
    bool flag = leftHeld & rightHeld;
    if (WorkCamera.IsKeyDown(keyData, (VirtualKey) 87) | flag)
      --num1;
    if (WorkCamera.IsKeyDown(keyData, (VirtualKey) 83))
      ++num1;
    int num2 = 0;
    if (WorkCamera.IsKeyDown(keyData, (VirtualKey) 65))
      --num2;
    if (WorkCamera.IsKeyDown(keyData, (VirtualKey) 68))
      ++num2;
    this.Velocity.X = (float) ((double) num1 * (double) MathF.Sin(this.Rotation.X) * (double) MathF.Cos(this.Rotation.Y) + (double) num2 * (double) MathF.Cos(this.Rotation.X));
    this.Velocity.Y = (float) num1 * MathF.Sin(this.Rotation.Y);
    this.Velocity.Z = (float) ((double) num1 * (double) MathF.Cos(this.Rotation.X) * (double) MathF.Cos(this.Rotation.Y) + (double) -num2 * (double) MathF.Sin(this.Rotation.X));
    if (WorkCamera.IsKeyDown(keyData, (VirtualKey) 32 /*0x20*/))
      ++this.Velocity.Y;
    if (!WorkCamera.IsKeyDown(keyData, (VirtualKey) 81))
      return;
    --this.Velocity.Y;
  }

  private static unsafe bool IsKeyDown(KeyboardDeviceData* keyData, VirtualKey key)
  {
    return keyData->IsKeyDown(key, true);
  }

  public unsafe void Update()
  {
    DateTime now = DateTime.Now;
    float num1 = Math.Max((float) (now - this.LastTime).TotalMilliseconds, 1f);
    this.LastTime = now;
    float num2 = Math.Abs(this.Camera->RenderEx->FoV);
    this.MouseDelta = this.MouseDelta * num2 * (7f / 400f) * 0.2f * this._ctx.Config.Camera.PanSensitivityModifier;
    this.Rotation.X -= this.MouseDelta.X;
    this.Rotation.Y = Math.Clamp(this.Rotation.Y + this.MouseDelta.Y, -1.57072f, 1.57072f);
    this.MouseDelta = Vector2.Zero;
    this.Position += this.Velocity * this.MoveSpeed * num2 * 0.2f;
    this.InterpPos = Vector3.Lerp(this.InterpPos, this.Position, MathF.Pow(0.5f, num1 * 0.05f));
  }

  public Matrix4x4 CalculateViewMatrix()
  {
    Vector3 interpPos = this.InterpPos;
    Vector3 lookDirection = this.CalculateLookDirection();
    Vector3 upVector = WorkCamera.UpVector;
    float num1 = MathF.Sqrt((float) ((double) lookDirection.X * (double) lookDirection.X + (double) lookDirection.Y * (double) lookDirection.Y + (double) lookDirection.Z * (double) lookDirection.Z));
    Vector3 vector3_1 = lookDirection / num1;
    Vector3 vector3_2 = new Vector3((float) ((double) upVector.Y * (double) vector3_1.Z - (double) upVector.Z * (double) vector3_1.Y), (float) ((double) upVector.Z * (double) vector3_1.X - (double) upVector.X * (double) vector3_1.Z), (float) ((double) upVector.X * (double) vector3_1.Y - (double) upVector.Y * (double) vector3_1.X));
    float num2 = MathF.Sqrt((float) ((double) vector3_2.X * (double) vector3_2.X + (double) vector3_2.Y * (double) vector3_2.Y + (double) vector3_2.Z * (double) vector3_2.Z));
    Vector3 vector3_3 = vector3_2 / num2;
    Vector3 vector3_4 = new Vector3((float) ((double) vector3_1.Y * (double) vector3_3.Z - (double) vector3_1.Z * (double) vector3_3.Y), (float) ((double) vector3_1.Z * (double) vector3_3.X - (double) vector3_1.X * (double) vector3_3.Z), (float) ((double) vector3_1.X * (double) vector3_3.Y - (double) vector3_1.Y * (double) vector3_3.X));
    Vector3 vector3_5 = new Vector3((float) (-(double) interpPos.X * (double) vector3_3.X - (double) interpPos.Y * (double) vector3_3.Y - (double) interpPos.Z * (double) vector3_3.Z), (float) (-(double) interpPos.X * (double) vector3_4.X - (double) interpPos.Y * (double) vector3_4.Y - (double) interpPos.Z * (double) vector3_4.Z), (float) (-(double) interpPos.X * (double) vector3_1.X - (double) interpPos.Y * (double) vector3_1.Y - (double) interpPos.Z * (double) vector3_1.Z));
    return new Matrix4x4(vector3_3.X, vector3_4.X, vector3_1.X, 0.0f, vector3_3.Y, vector3_4.Y, vector3_1.Y, 0.0f, vector3_3.Z, vector3_4.Z, vector3_1.Z, 0.0f, vector3_5.X, vector3_5.Y, vector3_5.Z, 1f);
  }

  private Vector3 CalculateLookDirection()
  {
    return new Vector3(MathF.Sin(this.Rotation.X) * MathF.Cos(this.Rotation.Y), MathF.Sin(this.Rotation.Y), MathF.Cos(this.Rotation.X) * MathF.Cos(this.Rotation.Y));
  }
}
