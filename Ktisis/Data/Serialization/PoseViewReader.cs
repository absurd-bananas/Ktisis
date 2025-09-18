// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Serialization.PoseViewReader
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Data.Config.Pose2D;
using System.IO;
using System.Numerics;
using System.Xml;

#nullable enable
namespace Ktisis.Data.Serialization;

public static class PoseViewReader
{
  private const string ViewTag = "View";
  private const string ImageTag = "Image";
  private const string BoneTag = "Bone";

  public static PoseViewSchema ReadStream(Stream stream)
  {
    PoseViewSchema schema = new PoseViewSchema();
    using (XmlReader reader = XmlReader.Create(stream))
    {
      while (reader.Read())
      {
        if (reader.NodeType == XmlNodeType.Element && !(reader.Name != "View"))
        {
          PoseViewEntry poseViewEntry = PoseViewReader.ReadView(reader, schema);
          schema.Views.Add(poseViewEntry.Name, poseViewEntry);
        }
      }
      return schema;
    }
  }

  private static PoseViewEntry ReadView(XmlReader reader, PoseViewSchema schema)
  {
    PoseViewEntry poseViewEntry = new PoseViewEntry()
    {
      Name = reader.GetAttribute("name") ?? "INVALID"
    };
    while (reader.Read())
    {
      switch (reader.NodeType)
      {
        case XmlNodeType.Element:
          if (reader.Name == "Image")
          {
            string attribute = reader.GetAttribute("file");
            if (attribute != null)
            {
              poseViewEntry.Images.Add(attribute);
              continue;
            }
            continue;
          }
          if (reader.Name == "Bone")
          {
            float result1;
            if (!float.TryParse(reader.GetAttribute("x"), out result1))
              result1 = 0.0f;
            float result2;
            if (!float.TryParse(reader.GetAttribute("y"), out result2))
              result2 = 0.0f;
            PoseViewBone poseViewBone = new PoseViewBone()
            {
              Label = reader.GetAttribute("label") ?? string.Empty,
              Name = reader.GetAttribute("name") ?? string.Empty,
              Position = new Vector2(result1, result2)
            };
            poseViewEntry.Bones.Add(poseViewBone);
            continue;
          }
          continue;
        case XmlNodeType.EndElement:
          if (reader.Name == "View")
            return poseViewEntry;
          continue;
        default:
          continue;
      }
    }
    return poseViewEntry;
  }
}
