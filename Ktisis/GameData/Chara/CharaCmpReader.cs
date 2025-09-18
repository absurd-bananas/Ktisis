// Decompiled with JetBrains decompiler
// Type: Ktisis.GameData.Chara.CharaCmpReader
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Plugin.Services;
using Ktisis.Structs.Characters;
using System;
using System.IO;

#nullable enable
namespace Ktisis.GameData.Chara;

public class CharaCmpReader(BinaryReader br)
{
  private const string HumanCmpPath = "chara/xls/charamake/human.cmp";
  private const int BlockLength = 256 /*0x0100*/;
  private const int DataLength = 192 /*0xC0*/;
  private const int AlphaLength = 128 /*0x80*/;
  private const int CommonBlockCount = 5;
  private const int CommonBlockSize = 10;
  private const int TribeBlockSkipCount = 3;
  private const int TribeBlockCount = 2;
  private const int GenderBlockSize = 5120;
  private const int TribeBlockSize = 10240;
  private const int CommonSeekTo = 5120;
  private const int TribesSeekTo = 13312;
  private const uint ExtendedDataLength = 208 /*0xD0*/;

  public static CharaCmpReader Open(IDataManager data)
  {
    return new CharaCmpReader(new BinaryReader((Stream) new MemoryStream((data.GetFile("chara/xls/charamake/human.cmp") ?? throw new Exception("Failed to open human.cmp")).Data)));
  }

  public CommonColors ReadCommon()
  {
    this.SeekTo(5120U);
    uint[] numArray1 = this.ReadArray(192U /*0xC0*/);
    this.SeekNextBlock();
    uint[] numArray2 = this.ReadArray(208U /*0xD0*/);
    this.SeekNextBlock();
    uint[] numArray3 = this.ReadArray(128U /*0x80*/);
    this.SeekNextBlock();
    uint[] numArray4 = this.ReadArray(208U /*0xD0*/);
    this.SeekNextBlock();
    uint[] numArray5 = this.ReadArray(128U /*0x80*/);
    this.SeekNextBlock();
    return new CommonColors()
    {
      EyeColors = numArray1,
      HighlightColors = numArray2,
      LipColors = numArray3,
      FaceFeatureColors = numArray4,
      FacepaintColors = numArray5
    };
  }

  public TribeColors ReadTribeData(Tribe tribe, Gender gender)
  {
    this.SeekTo((uint) (13312 + 10240 * (int) (tribe - (byte) 1) + 5120 * (int) gender));
    uint[] numArray1 = this.ReadArray(192U /*0xC0*/);
    this.SeekNextBlock();
    bool flag;
    switch (tribe)
    {
      case Tribe.Helion:
      case Tribe.Lost:
        flag = true;
        break;
      default:
        flag = false;
        break;
    }
    uint[] numArray2 = this.ReadArray(!flag ? 208U /*0xD0*/ : 192U /*0xC0*/);
    return new TribeColors()
    {
      SkinColors = numArray1,
      HairColors = numArray2
    };
  }

  private void SeekTo(uint offset) => br.BaseStream.Seek((long) offset, (SeekOrigin) 0);

  private uint[] ReadArray(uint length)
  {
    uint[] numArray = new uint[(int) length];
    for (int index = 0; (long) index < (long) length; ++index)
      numArray[index] = br.ReadUInt32();
    return numArray;
  }

  private void SeekNextBlock()
  {
    br.BaseStream.Seek(1024L /*0x0400*/ - br.BaseStream.Position % 1024L /*0x0400*/, (SeekOrigin) 1);
  }
}
