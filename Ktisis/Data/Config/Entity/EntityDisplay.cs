// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Config.Entity.EntityDisplay
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Interface;
using Ktisis.Scene.Types;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable
namespace Ktisis.Data.Config.Entity;

public record EntityDisplay
{
  public uint Color;
  public FontAwesomeIcon Icon;
  public DisplayMode Mode;
  private const uint BoneBlue = 4294942568;
  private const uint ModelMint = 4290445234;
  private const uint LightLemon = 4285066751;

  public EntityDisplay(uint color = 4294967295 /*0xFFFFFFFF*/, FontAwesomeIcon icon = 0, DisplayMode mode = DisplayMode.Icon)
  {
    this.Color = color;
    this.Icon = icon;
    this.Mode = mode;
  }

  public static Dictionary<EntityType, EntityDisplay> GetDefaults()
  {
    return new Dictionary<EntityType, EntityDisplay>()
    {
      {
        EntityType.Invalid,
        new EntityDisplay()
      },
      {
        EntityType.Actor,
        new EntityDisplay(icon: (FontAwesomeIcon) 61870)
      },
      {
        EntityType.Armature,
        new EntityDisplay(4294942568U, (FontAwesomeIcon) 58594)
      },
      {
        EntityType.BoneGroup,
        new EntityDisplay(4294942568U, mode: DisplayMode.None)
      },
      {
        EntityType.BoneNode,
        new EntityDisplay(mode: DisplayMode.Dot)
      },
      {
        EntityType.Models,
        new EntityDisplay(4290445234U, (FontAwesomeIcon) 58598)
      },
      {
        EntityType.ModelSlot,
        new EntityDisplay(4290445234U)
      },
      {
        EntityType.Weapon,
        new EntityDisplay(icon: (FontAwesomeIcon) 61648)
      },
      {
        EntityType.Light,
        new EntityDisplay(4285066751U, (FontAwesomeIcon) 61675)
      },
      {
        EntityType.RefImage,
        new EntityDisplay(icon: (FontAwesomeIcon) 61502)
      }
    };
  }

  [CompilerGenerated]
  protected virtual bool PrintMembers(StringBuilder builder)
  {
    RuntimeHelpers.EnsureSufficientExecutionStack();
    builder.Append("Color = ");
    builder.Append(this.Color.ToString());
    builder.Append(", Icon = ");
    builder.Append(this.Icon.ToString());
    builder.Append(", Mode = ");
    builder.Append(this.Mode.ToString());
    return true;
  }

  [CompilerGenerated]
  public override int GetHashCode()
  {
    return ((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<uint>.Default.GetHashCode(this.Color)) * -1521134295 + EqualityComparer<FontAwesomeIcon>.Default.GetHashCode(this.Icon)) * -1521134295 + EqualityComparer<DisplayMode>.Default.GetHashCode(this.Mode);
  }

  [CompilerGenerated]
  public virtual bool Equals(EntityDisplay? other)
  {
    if ((object) this == (object) other)
      return true;
    return (object) other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<uint>.Default.Equals(this.Color, other.Color) && EqualityComparer<FontAwesomeIcon>.Default.Equals(this.Icon, other.Icon) && EqualityComparer<DisplayMode>.Default.Equals(this.Mode, other.Mode);
  }

  [CompilerGenerated]
  protected EntityDisplay(EntityDisplay original)
  {
    this.Color = original.Color;
    this.Icon = original.Icon;
    this.Mode = original.Mode;
  }
}
