using UnityEngine;
using System;
using FileIOUtility;


public static class JsonHelper<T>
{
	public static T Read( string path )
	{


        string text = FileIOHelper.ReadFile(path);


        return JsonUtility.FromJson<T>( text );
	}


    public static void Write( string path, T data )
	{
		string file_path = path;
		string json = JsonUtility.ToJson( data, true );
		FileIOHelper.WriteFile( file_path, json );
	}


}
