// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Config.Sections.PyonConfig
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System.Collections.Generic;
using System.Drawing;

#nullable enable
namespace Ktisis.Data.Config.Sections;

public class PyonConfig
{
  public int DefaultStyle;
  public Point DefaultPosition;
  public Size DefaultSize;
  public Size DefaultDeviceSize;
  public Size HiResSize;
  public List<Size> Resolutions = new List<Size>();
}
