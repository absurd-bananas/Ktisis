// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Config.Sections.OverlayConfig
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
namespace Ktisis.Data.Config.Sections;

public class OverlayConfig {

	public enum ActiveState {
		Selection,
		Target,
		Both
	}

	public float ActiveActorOpacityMultiplier = 1f;
	public ActiveState ActiveStateType = ActiveState.Both;
	public bool ColorSelectedBoneDescendantLine = true;
	public bool ColorSelectedBoneParentLine = true;
	public uint DefaultLineColor = uint.MaxValue;
	public uint DotColor = uint.MaxValue;
	public uint DotColorSelected = uint.MaxValue;
	public float DotOpacity = 0.95f;
	public float DotOpacityUsing = 0.15f;
	public float DotOutline = 1f;
	public uint DotOutlineColor = 4278190080 /*0xFF000000*/;
	public uint DotOutlineColorSelected = 4278190080 /*0xFF000000*/;
	public float DotOutlineSelected = 2.5f;
	public float DotRadius = 7f;
	public float DotRadiusSelected = 8f;
	public bool DrawDotsGizmo = true;
	public bool DrawLines = true;
	public bool DrawLinesGizmo = true;
	public bool DrawReferenceTitle = true;
	public float InactiveActorOpacityMultiplier = 1f;
	public float LineOpacity = 0.95f;
	public float LineOpacityUsing = 0.15f;
	public float LineThickness = 2f;
	public uint SelectedBoneDescendantLineColor = 4281597747 /*0xFF33FF33*/;
	public uint SelectedBoneParentLineColor = 4281545727;
	public bool Visible = true;
}
