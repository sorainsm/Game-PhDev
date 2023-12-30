using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;

public class FileHandler
{
	private string ConfigPath;
	private string GeneratedFile;
	private string MetaFile;


	public FileHandler(string Path, string FileName, string MetaFileName)
	{
		ConfigPath = Path;
		GeneratedFile = FileName;
		MetaFile = MetaFileName;
	}



	public void DeleteGeneratedConfig()
	{
		File.Delete(ConfigPath + GeneratedFile);

		File.Delete(ConfigPath + MetaFile);
	}

	public bool DirectoryExists(string path)
	{
		return File.Exists(path);
	}

	public void CreateDirectory(string path)
	{
		Directory.CreateDirectory(path);
	}

	public void DeleteDirectory(string path, bool and_contents)
	{
		if (DirectoryExists(path))
		{
			Directory.Delete(path, and_contents);
		}
	}

	public void WriteGenerated(string json)
	{
		Debug.Log("FileHandler::Writing Generated Config");
		var path = StandaloneFileBrowser.SaveFilePanel("Save Config File", "","","json");
        Debug.Log("FileHandler::File is: " + path);		
		if (!string.IsNullOrEmpty(path))
		{
			WriteFile(path, json);
		} else 
		{
			Debug.Log("FileHandler:: " + path + " not selected.");
		}
	}

	public void WriteFile(string path, string data)
	{
		Debug.Log("FileHandler::Writing at " + path);
		using (var file = File.Open(path, FileMode.OpenOrCreate))
		{
			using (var sr = new StreamWriter(file))
			{
				sr.Write(data);
			}
		}
	}

	public string ReadFile(string path)
	{
		string data = null;
		Debug.Log("FileHandler::Reading file " + path);
		if (File.Exists(path))
		{
			data = File.ReadAllText(path);
		}
		return data;
	}

}
