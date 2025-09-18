// Decompiled with JetBrains decompiler
// Type: Ktisis.Services.Game.CameraService
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System;

namespace Ktisis.Services.Game;

public static class CameraService {
	public unsafe static Camera* GetGameCamera() {
		CameraManager* cameraManagerPtr = CameraManager.Instance();
		return (IntPtr)cameraManagerPtr == IntPtr.Zero ? (Camera*)null : ((CameraManager)(IntPtr)cameraManagerPtr).GetActiveCamera();
	}

	public unsafe static Camera* GetSceneCamera() {
		Camera* gameCamera = GetGameCamera();
		return (IntPtr)gameCamera == IntPtr.Zero ? (Camera*)null : &gameCamera->CameraBase.SceneCamera;
	}

	public unsafe static Camera* GetRenderCamera() {
		Camera* sceneCamera = GetSceneCamera();
		return (IntPtr)sceneCamera == IntPtr.Zero ? (Camera*)null : sceneCamera->RenderCamera;
	}

	public unsafe static Matrix4x4? GetProjectionMatrix() {
		Camera* renderCamera = GetRenderCamera();
		if ((IntPtr)renderCamera == IntPtr.Zero)
			return new Matrix4x4?();
		Matrix4x4 projectionMatrix = renderCamera->ProjectionMatrix;
		float farPlane = renderCamera->FarPlane;
		float nearPlane = renderCamera->NearPlane;
		var num = farPlane / (farPlane - nearPlane);
		projectionMatrix.M33 = (float)-((farPlane + (double)nearPlane) / (farPlane - (double)nearPlane));
		projectionMatrix.M43 = (float)-(num * (double)nearPlane);
		return new Matrix4x4?(Matrix4x4.op_Implicit(projectionMatrix));
	}

	public unsafe static Matrix4x4? GetViewMatrix() {
		Camera* sceneCamera = GetSceneCamera();
		if ((IntPtr)sceneCamera == IntPtr.Zero)
			return new Matrix4x4?();
		Matrix4x4 viewMatrix = sceneCamera->ViewMatrix;
		viewMatrix.M44 = 1f;
		return new Matrix4x4?(Matrix4x4.op_Implicit(viewMatrix));
	}

	public unsafe static bool WorldToScreen(Camera* camera, Vector3 worldPos, out Vector2 screenPos) {
		Matrix4x4 matrix4x4_1 = camera->ViewMatrix;
		if (camera->RenderCamera->IsOrtho) {
			Matrix4x4 matrix4x4_2 = matrix4x4_1;
			matrix4x4_2.M44 = 1f;
			matrix4x4_1 = matrix4x4_2;
		}
		Vector3 screenPos1;
		var num = WorldToScreenDepth(Matrix4x4.op_Implicit(Matrix4x4.op_Multiply(matrix4x4_1, camera->RenderCamera->ProjectionMatrix)), worldPos, out screenPos1) ? 1 : 0;
		screenPos = new Vector2(screenPos1.X, screenPos1.Y);
		return num != 0;
	}

	private static bool WorldToScreenDepth(Matrix4x4 m, Vector3 v, out Vector3 screenPos) {
		var num1 = (float)((double)m.M11 * (double)v.X + (double)m.M21 * (double)v.Y + (double)m.M31 * (double)v.Z) + m.M41;
		var num2 = (float)((double)m.M12 * (double)v.X + (double)m.M22 * (double)v.Y + (double)m.M32 * (double)v.Z) + m.M42;
		var z = (float)((double)m.M14 * (double)v.X + (double)m.M24 * (double)v.Y + (double)m.M34 * (double)v.Z) + m.M44;
		ImGuiViewportPtr mainViewport = ImGuiHelpers.MainViewport;
		float num3 = ((ImGuiViewportPtr) ref mainViewport ).Size.X / 2f;
		float num4 = ((ImGuiViewportPtr) ref mainViewport ).Size.Y / 2f;
		screenPos = new Vector3(num3 + num3 * num1 / z + ((ImGuiViewportPtr) ref mainViewport).Pos.X, num4 - num4 * num2 / z + ((ImGuiViewportPtr) ref mainViewport).Pos.Y, z);
		return z > 1.0 / 1000.0;
	}
}
