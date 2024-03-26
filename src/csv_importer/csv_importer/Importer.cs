using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace csv_importer
{
	public class Importer
	{
		public Dictionary<string, string> Files { get; protected set; } // <PathToFile, SQL Table Name>
		private Regex Remover;

		public Importer()
		{
			this.Files = new Dictionary<string, string>();
			this.Remover = new Regex("[^a-zA-Z_]");
		}

		public string CleanString(string Target)
		{
			Target = Target.Trim();
			Target = this.Remover.Replace(Target, "");

			return Target;
		}

		public string GetFileNameWithoutExtension(string FilePath)
		{
			FilePath = Path.GetFileName(FilePath);
			FilePath = FilePath.ToLower();

			if (!FilePath.EndsWith(".csv"))
				return string.Empty;

			FilePath = Path.GetFileNameWithoutExtension(FilePath);

			return this.CleanString(FilePath);
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

		public void DropFiles()
		{
			Files.Clear();
		}
	}
}
