// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Serialization.CategoryReader
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Utility;
using Ktisis.Data.Config.Bones;
using Ktisis.Data.Config.Sections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

#nullable enable
namespace Ktisis.Data.Serialization;

public static class CategoryReader
{
  private const string BonesTag = "Bones";
  private const string CategoryTag = "Category";
  private const string TwoJointsIkTag = "TwoJointsIK";
  private const string CcdIkTag = "CcdIK";

  public static CategoryConfig ReadStream(Stream stream)
  {
    CategoryConfig categories = new CategoryConfig();
    using (XmlReader reader = XmlReader.Create(stream))
    {
      while (reader.Read())
      {
        if (reader.NodeType == XmlNodeType.Element && !(reader.Name != "Category"))
          CategoryReader.ReadCategory(reader, categories);
      }
      return categories;
    }
  }

  private static BoneCategory ReadCategory(XmlReader reader, CategoryConfig categories)
  {
    BoneCategory category = new BoneCategory(reader.GetAttribute("Id") ?? "Unknown")
    {
      IsNsfw = reader.GetAttribute("IsNsfw") == "true",
      IsDefault = reader.GetAttribute("IsDefault") == "true"
    };
    categories.AddCategory(category);
    while (reader.Read())
    {
      switch (reader.NodeType)
      {
        case XmlNodeType.Element:
          if (reader.Name == "Category")
          {
            CategoryReader.ReadCategory(reader, categories).ParentCategory = category.Name;
            continue;
          }
          if (reader.Name == "Bones")
          {
            CategoryReader.ReadBone(reader, category);
            continue;
          }
          if (reader.Name == "TwoJointsIK")
          {
            category.TwoJointsGroup = CategoryReader.ReadTwoJointsIkGroup(reader);
            continue;
          }
          if (reader.Name == "CcdIK")
          {
            category.CcdGroup = CategoryReader.ReadCcdIkGroup(reader);
            continue;
          }
          continue;
        case XmlNodeType.EndElement:
          if (reader.Name == "Category")
            return category;
          continue;
        default:
          continue;
      }
    }
    return category;
  }

  private static void ReadBone(XmlReader reader, BoneCategory category)
  {
    reader.Read();
    if (reader.NodeType != XmlNodeType.Text)
      return;
    IEnumerable<CategoryBone> collection = ((IEnumerable<string>) reader.Value.Split((char[]) null)).Select<string, string>((Func<string, string>) (ln => ln.Trim())).Where<string>((Func<string, bool>) (ln => !StringExtensions.IsNullOrEmpty(ln))).Select<string, CategoryBone>((Func<string, CategoryBone>) (bone => new CategoryBone(bone)));
    category.Bones.AddRange(collection);
  }

  private static TwoJointsGroupParams ReadTwoJointsIkGroup(XmlReader reader)
  {
    TwoJointsGroupParams jointsGroupParams1 = new TwoJointsGroupParams();
    TwoJointsGroupParams jointsGroupParams2 = jointsGroupParams1;
    TwoJointsType twoJointsType;
    switch (reader.GetAttribute("Type"))
    {
      case "Arm":
        twoJointsType = TwoJointsType.Arm;
        break;
      case "Leg":
        twoJointsType = TwoJointsType.Leg;
        break;
      default:
        twoJointsType = TwoJointsType.None;
        break;
    }
    jointsGroupParams2.Type = twoJointsType;
    TwoJointsGroupParams jointsGroupParams3 = jointsGroupParams1;
    while (reader.Read() && (reader == null || reader.NodeType != XmlNodeType.EndElement || !(reader.Name == "TwoJointsIK")))
    {
      if (reader.NodeType == XmlNodeType.Element)
      {
        string name = reader.Name;
        reader.Read();
        if (reader.NodeType == XmlNodeType.Text)
        {
          List<string> stringList;
          switch (name)
          {
            case "FirstBone":
              stringList = jointsGroupParams3.FirstBone;
              break;
            case "FirstTwist":
              stringList = jointsGroupParams3.FirstTwist;
              break;
            case "SecondBone":
              stringList = jointsGroupParams3.SecondBone;
              break;
            case "SecondTwist":
              stringList = jointsGroupParams3.SecondTwist;
              break;
            case "EndBone":
              stringList = jointsGroupParams3.EndBone;
              break;
            default:
              throw new Exception("Encountered invalid IK bone parameter: " + name);
          }
          stringList.Add(reader.Value);
        }
      }
    }
    return jointsGroupParams3;
  }

  private static CcdGroupParams ReadCcdIkGroup(XmlReader reader)
  {
    CcdGroupParams ccdGroupParams = new CcdGroupParams();
    while (reader.Read() && (reader == null || reader.NodeType != XmlNodeType.EndElement || !(reader.Name == "CcdIK")))
    {
      if (reader.NodeType == XmlNodeType.Element)
      {
        string name = reader.Name;
        reader.Read();
        if (reader.NodeType == XmlNodeType.Text)
        {
          List<string> stringList;
          switch (name)
          {
            case "StartBone":
              stringList = ccdGroupParams.StartBone;
              break;
            case "EndBone":
              stringList = ccdGroupParams.EndBone;
              break;
            default:
              throw new Exception("Encountered invalid IK bone parameter: " + name);
          }
          stringList.Add(reader.Value);
        }
      }
    }
    return ccdGroupParams;
  }
}
