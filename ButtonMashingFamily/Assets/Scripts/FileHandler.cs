using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		return File.Exists(ConfigPath + GeneratedFile);
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
		WriteFile(ConfigPath, GeneratedFile, json);
		Debug.Log("Wrote Generated Config");
	}

	public void WriteFile(string path, string filename, string text)
	{
		if (DirectoryExists(path))
		{
			File.WriteAllText(path+filename, text);
		}
	}

}
