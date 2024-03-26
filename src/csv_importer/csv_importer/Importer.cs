﻿using System;
using System.Collections.Generic;
using System.IO;

namespace csv_importer
{
	public class Importer
	{
		public Dictionary<string, string> Files { get; protected set; } // <PathToFile, SQL Table Name>

		private StringCleaner Cleaner;

		public Importer()
		{
			this.Files = new Dictionary<string, string>();
			this.Cleaner = new StringCleaner();
		}

		public string GetFileNameWithoutExtension(string FilePath)
		{
			FilePath = Path.GetFileName(FilePath);
			FilePath = FilePath.ToLower();

			if (!FilePath.EndsWith(".csv"))
				return string.Empty;

			FilePath = Path.GetFileNameWithoutExtension(FilePath);

			return this.Cleaner.CleanFileName(FilePath);
		}

		public void AddFromDirectory(string SearchDirectory)
		{
			if (string.IsNullOrEmpty(SearchDirectory)) return;
			if (!Directory.Exists(SearchDirectory)) return;

			string[] DirectoryEntries = Directory.GetFiles(SearchDirectory);
			if (DirectoryEntries.Length < 1) return;

			foreach (string DirectoryEntry in DirectoryEntries)
			{
				if (!File.Exists(DirectoryEntry)) continue;

				string FileName = this.GetFileNameWithoutExtension(DirectoryEntry);

				if (string.IsNullOrEmpty(FileName)) continue;

				this.Files.Add(DirectoryEntry, FileName);
			}
		}

		public void HandleFiles()
		{
			CSVParser Parser = new CSVParser();

			foreach (string FilePath in this.Files.Keys)
			{
				CSVData CSV = Parser.HandleFile(FilePath);

				Console.WriteLine(CSV.RowData.Count);

				break;
			}
		}
	}
}
