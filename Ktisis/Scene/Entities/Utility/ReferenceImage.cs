// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Entities.Utility.ReferenceImage
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Data.Config;
using Ktisis.Scene.Decor;
using Ktisis.Scene.Types;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable
namespace Ktisis.Scene.Entities.Utility;

public class ReferenceImage : SceneEntity, IVisibility, IDeletable
{
  public readonly ReferenceImage.SetupData Data;

  private Configuration Config => this.Scene.Context.Config;

  public ReferenceImage(ISceneManager scene, ReferenceImage.SetupData data)
    : base(scene)
  {
    this.Data = data;
    this.Type = EntityType.RefImage;
  }

  public bool Visible
  {
    get => this.Data.Visible;
    set => this.Data.Visible = value;
  }

  public void Save()
  {
    List<ReferenceImage.SetupData> referenceImages = this.Config.Editor.ReferenceImages;
    this.Data.Id = $"{referenceImages.Count}-{this.Data.GetHashCode():X}";
    referenceImages.Add(this.Data);
  }

  public bool Delete()
  {
    this.Config.Editor.ReferenceImages.Remove(this.Data);
    this.Remove();
    return true;
  }

  public void SetFilePath(string newPath)
  {
    string filePath = this.Data.FilePath;
    this.Data.FilePath = newPath;
    if (!(this.Name == Path.GetFileName(filePath)))
      return;
    this.Name = Path.GetFileName(newPath);
  }

  public record SetupData()
  {
    public string Id;
    public string FilePath;
    public float Opacity;
    public bool Visible;

    [CompilerGenerated]
    protected virtual bool PrintMembers(StringBuilder builder)
    {
      RuntimeHelpers.EnsureSufficientExecutionStack();
      builder.Append("Id = ");
      builder.Append((object) this.Id);
      builder.Append(", FilePath = ");
      builder.Append((object) this.FilePath);
      builder.Append(", Opacity = ");
      builder.Append(this.Opacity.ToString());
      builder.Append(", Visible = ");
      builder.Append(this.Visible.ToString());
      return true;
    }

    [CompilerGenerated]
    public override int GetHashCode()
    {
      return (((EqualityComparer<System.Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Id)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.FilePath)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.Opacity)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.Visible);
    }

    [CompilerGenerated]
    public virtual bool Equals(ReferenceImage.SetupData? other)
    {
      if ((object) this == (object) other)
        return true;
      return (object) other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<string>.Default.Equals(this.Id, other.Id) && EqualityComparer<string>.Default.Equals(this.FilePath, other.FilePath) && EqualityComparer<float>.Default.Equals(this.Opacity, other.Opacity) && EqualityComparer<bool>.Default.Equals(this.Visible, other.Visible);
    }

    [CompilerGenerated]
    protected SetupData(ReferenceImage.SetupData original)
    {
      this.Id = original.Id;
      this.FilePath = original.FilePath;
      this.Opacity = original.Opacity;
      this.Visible = original.Visible;
    }
  }
}
