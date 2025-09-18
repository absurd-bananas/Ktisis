// Decompiled with JetBrains decompiler
// Type: Ktisis.Common.Utility.PlayerNameUtil
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;

#nullable enable
namespace Ktisis.Common.Utility;

public static class PlayerNameUtil
{
  private static readonly string[] Single = new string[16 /*0x10*/]
  {
    "Zero",
    "One",
    "Two",
    "Three",
    "Four",
    "Five",
    "Six",
    "Seven",
    "Eight",
    "Nine",
    "Ten",
    "Eleven",
    "Twelve",
    "Thirteen",
    "Fourteen",
    "Fifteen"
  };
  private static readonly string[] Tens = new string[8]
  {
    "Twenty",
    "Thirty",
    "Forty",
    "Fifty",
    "Sixty",
    "Seventy",
    "Eighty",
    "Ninety"
  };

  public static string CalcActorName(ushort index, string firstName = "Actor")
  {
    return $"{firstName} {PlayerNameUtil.CalcActorNameWords(index)}";
  }

  private static string CalcActorNameWords(ushort index)
  {
    if (index >= (ushort) 200)
      index -= (ushort) 200;
    if ((int) index < PlayerNameUtil.Single.Length)
      return PlayerNameUtil.Single[(int) index];
    if (index < (ushort) 20)
    {
      int index1 = (int) index - 10;
      string str1 = PlayerNameUtil.Single[index1];
      if (str1.EndsWith('t'))
      {
        string str2 = str1;
        str1 = str2.Substring(0, str2.Length - 1);
      }
      return str1 + "teen";
    }
    int index2 = (int) Math.Floor((Decimal) ((int) index - 20) / 10M);
    int index3 = (int) index % 10;
    string ten = PlayerNameUtil.Tens[index2];
    return index3 == 0 ? ten : $"{ten}-{PlayerNameUtil.Single[index3].ToLowerInvariant()}";
  }
}
