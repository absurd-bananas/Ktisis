// Decompiled with JetBrains decompiler
// Type: Ktisis.Localization.LocaleDataLoader
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Data.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;

#nullable enable
namespace Ktisis.Localization;

public class LocaleDataLoader
{
  private static readonly JsonReaderOptions readerOptions;

  private Stream GetResourceStream(string technicalName)
  {
    return Assembly.GetExecutingAssembly().GetManifestResourceStream("KtisisPyon.Localization.Data.en_US.json") ?? throw new Exception($"Cannot find data file '{technicalName}'");
  }

  public LocaleMetaData LoadMeta(string technicalName)
  {
    using (Stream resourceStream = this.GetResourceStream(technicalName))
    {
      BlockBufferJsonReader reader = new BlockBufferJsonReader(resourceStream, stackalloc byte[4096 /*0x1000*/], LocaleDataLoader.readerOptions);
      reader.Read();
      if (((Utf8JsonReader) ref reader.Reader).TokenType != 1)
        throw new Exception($"Locale Data file '{technicalName}.json' does not contain a top-level object.");
      while (reader.Read())
      {
        JsonTokenType tokenType = ((Utf8JsonReader) ref reader.Reader).TokenType;
        if (tokenType == 2)
          throw new Exception($"Locale Data file '{technicalName}' is is missing the top-level '$meta' object.");
        if (tokenType != 5)
          throw new Exception("Should not reach this point.");
        if (((Utf8JsonReader) ref reader.Reader).GetString() == "$meta")
          return this.ReadMetaObject(technicalName, ref reader);
        reader.SkipIt();
      }
      throw new Exception($"Locale Data file '{technicalName}.json' is missing its meta data (top-level '$meta' key not found)");
    }
  }

  private LocaleMetaData ReadMetaObject(string technicalName, ref BlockBufferJsonReader reader)
  {
    reader.Read();
    if (((Utf8JsonReader) ref reader.Reader).TokenType != 1)
      throw new Exception($"Locale Data file '{technicalName}.json' has a non-object at the top-level '$meta' key.");
    string displayName = (string) null;
    string selfName = (string) null;
    string[] maintainers = (string[]) null;
    while (true)
    {
      JsonTokenType tokenType1;
      do
      {
        ((Utf8JsonReader) ref reader.Reader).Read();
        tokenType1 = ((Utf8JsonReader) ref reader.Reader).TokenType;
        if (tokenType1 == 2)
          goto label_25;
      }
      while (tokenType1 != 5);
      string str = ((Utf8JsonReader) ref reader.Reader).GetString();
      reader.Read();
      switch (str)
      {
        case "__comment":
          continue;
        case "displayName":
          if (((Utf8JsonReader) ref reader.Reader).TokenType == 7)
          {
            displayName = ((Utf8JsonReader) ref reader.Reader).GetString();
            continue;
          }
          goto label_7;
        case "selfName":
          if (((Utf8JsonReader) ref reader.Reader).TokenType == 7)
          {
            selfName = ((Utf8JsonReader) ref reader.Reader).GetString();
            continue;
          }
          goto label_10;
        case "maintainers":
          if (((Utf8JsonReader) ref reader.Reader).TokenType == 3)
          {
            List<string> stringList = new List<string>();
            int num = 0;
            while (reader.Read())
            {
              JsonTokenType tokenType2 = ((Utf8JsonReader) ref reader.Reader).TokenType;
              if (tokenType2 != 4)
              {
                if (tokenType2 != 7)
                {
                  if (tokenType2 == 11)
                    stringList.Add((string) null);
                  else
                    throw new Exception($"Locale data file '{technicalName}' has an invalid value at '%.$meta.maintainers.{num}' (not a string or null).");
                }
                else
                  stringList.Add(((Utf8JsonReader) ref reader.Reader).GetString());
                ++num;
              }
              else
                break;
            }
            maintainers = stringList.ToArray();
            continue;
          }
          goto label_13;
        default:
          Ktisis.Ktisis.Log.Warning($"Locale data file '{technicalName}.json' has unknown meta key at '%.$meta.{((Utf8JsonReader) ref reader.Reader).GetString()}'", Array.Empty<object>());
          reader.SkipIt();
          continue;
      }
    }
label_7:
    throw new Exception($"Locale data file '{technicalName}.json' has an invalid '%.$meta.displayName' value (not a string).");
label_10:
    throw new Exception($"Locale data file '{technicalName}.json' has an invalid '%.$meta.selfName' value (not a string).");
label_13:
    throw new Exception($"Locale data file '{technicalName}.json' has an invalid '%.$meta.maintainers' value (not an array).");
label_25:
    if (displayName == null)
      throw new Exception($"Locale data file '{technicalName}.json' is missing the '%.$meta.displayName' value.");
    if (selfName == null)
      throw new Exception($"Locale data file '{technicalName}.json' is missing the '%.$meta.selfName' value.");
    if (maintainers == null)
      maintainers = new string[1];
    return new LocaleMetaData(technicalName, displayName, selfName, maintainers);
  }

  public LocaleData LoadData(string technicalName)
  {
    return this._LoadData(technicalName, (LocaleMetaData) null);
  }

  public LocaleData LoadData(LocaleMetaData metaData)
  {
    return this._LoadData(metaData.TechnicalName, metaData);
  }

  private LocaleData _LoadData(string technicalName, LocaleMetaData? meta)
  {
    using (Stream resourceStream = this.GetResourceStream(technicalName))
    {
      BlockBufferJsonReader reader = new BlockBufferJsonReader(resourceStream, stackalloc byte[4096 /*0x1000*/], LocaleDataLoader.readerOptions);
      reader.Read();
      if (((Utf8JsonReader) ref reader.Reader).TokenType != 1)
        throw new Exception($"Locale Data file '{technicalName}' does not contain a top-level object.");
      Dictionary<string, string> translationData = new Dictionary<string, string>();
      Stack<string> stringStack = new Stack<string>();
      string str1 = (string) null;
      int num = 0;
      while (reader.Read())
      {
        switch (((Utf8JsonReader) ref reader.Reader).TokenType - 1)
        {
          case 0:
            stringStack.Push(str1);
            continue;
          case 1:
            string str2;
            if (!stringStack.TryPop(ref str2))
              goto label_20;
            continue;
          case 2:
            this.WarnUnsupported(technicalName, "array", str1);
            reader.SkipIt();
            continue;
          case 4:
            if (stringStack.Count == 0 && ((Utf8JsonReader) ref reader.Reader).GetString() == "$meta")
            {
              ++num;
              if (meta == null)
              {
                meta = this.ReadMetaObject(technicalName, ref reader);
                continue;
              }
              reader.SkipIt();
              continue;
            }
            if (((Utf8JsonReader) ref reader.Reader).GetString() == "__comment")
            {
              reader.SkipIt();
              continue;
            }
            string str3;
            stringStack.TryPeek(ref str3);
            str1 = str3 == null ? ((Utf8JsonReader) ref reader.Reader).GetString() : $"{str3}.{((Utf8JsonReader) ref reader.Reader).GetString()}";
            continue;
          case 6:
            translationData.Add(str1, ((Utf8JsonReader) ref reader.Reader).GetString());
            continue;
          case 7:
            this.WarnUnsupported(technicalName, "number", str1);
            continue;
          case 8:
          case 9:
            this.WarnUnsupported(technicalName, "boolean", str1);
            continue;
          case 10:
            this.WarnUnsupported(technicalName, "null", str1);
            continue;
          default:
            continue;
        }
      }
label_20:
      if (num <= 1)
      {
        if (num == 0)
          throw new Exception($"Locale Data file '{technicalName}.json' is is missing the top-level '$meta' object.");
      }
      else
        Ktisis.Ktisis.Log.Warning($"Locale Data file '{technicalName}.json' has {{0}} top-level '$meta' objects?!", new object[1]
        {
          (object) num
        });
      translationData.TrimExcess();
      return new LocaleData(meta, translationData);
    }
  }

  private void WarnUnsupported(string technicalName, string elementType, string currentKey)
  {
    Ktisis.Ktisis.Log.Warning("Locale Data File '{0}.json' has an unsupported {1} at '%.{2}'.", new object[3]
    {
      (object) technicalName,
      (object) elementType,
      (object) currentKey
    });
  }

  static LocaleDataLoader()
  {
    JsonReaderOptions jsonReaderOptions = new JsonReaderOptions();
    ((JsonReaderOptions) ref jsonReaderOptions).AllowTrailingCommas = true;
    ((JsonReaderOptions) ref jsonReaderOptions).CommentHandling = (JsonCommentHandling) 1;
    LocaleDataLoader.readerOptions = jsonReaderOptions;
  }
}
