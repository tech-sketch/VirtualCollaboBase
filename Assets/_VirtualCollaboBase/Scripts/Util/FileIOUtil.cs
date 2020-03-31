using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class FileIOUtil
{
    public readonly static string NO_IMAGE_RESOUCE_STRING = "none";

    public static string GetIncrimentalUniqueFilePath(string dir_path, string file_name, string signature)
    {
        var di = new DirectoryInfo(dir_path);

        var max = di.GetFiles(file_name + "_???" + signature)                       // パターンに一致するファイルを取得する
            .Select(fi => Regex.Match(fi.Name, @"(?i)_(\d{3})\.rec"))      // ファイルの中で数値のものを探す
            .Where(m => m.Success)                                          // 該当するファイルだけに絞り込む
            .Select(m => Int32.Parse(m.Groups[1].Value))                    // 数値を取得する
            .DefaultIfEmpty(0)                                              // １つも該当しなかった場合は 0 とする
            .Max();                                                         // 最大値を取得する

        var fileName = String.Format("{0}_{1:d3}" + signature, file_name, max + 1);

        //string unique_path = dir_path + fileName;
        string unique_path = Regex.Replace((dir_path + fileName), @"\\", @"/");

        Debug.Log(unique_path);

        return unique_path;
    }

    public static DirectoryInfo SafeCreateDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            return null;
        }
        return Directory.CreateDirectory(path);
    }

    public static byte[] ReadPngFile(string path)
    {

        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        BinaryReader bin = new BinaryReader(fileStream);
        byte[] values = bin.ReadBytes((int)bin.BaseStream.Length);

        bin.Close();

        return values;
    }

    public static Texture2D ReadTexture(string path, int width, int height)
    {
        byte[] readBinary = ReadPngFile(path);

        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(readBinary);

        return texture;
    }

    public static Texture2D CreateTexturePngBinary(int width,int height,byte[] textureData)
    {
        var texture = new Texture2D(
    (int)width,
    (int)height,
    TextureFormat.RGBA32,
    false,
    false
);
        texture.LoadRawTextureData(textureData);
        texture.Apply();

        return texture;
    }
}

