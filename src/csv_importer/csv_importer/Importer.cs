using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security;

namespace csv_importer
{
	public class Importer
	{
		public Dictionary<string, string> Files { get; protected set; } // <PathToFile, SQL Table Name>

		private List<CSVData> CSVFiles;

		private StringCleaner Cleaner;

		MySqlConnection connection;

		public Importer()
		{
			this.Files = new Dictionary<string, string>();
			this.CSVFiles = List<CSVData>();

			this.Cleaner = new StringCleaner();
		}

		private string GetFileNameWithoutExtension(string FilePath)
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

		public void LoadFiles()
		{
			CSVParser Parser = new CSVParser();

			foreach (KeyValuePair<string, string> Pair in this.Files)
			{
				CSVData CSV = Parser.HandleFile(Pair.Key, Pair.Value);

				this.CSVFiles.Add(CSV);
			}
		}

		public void Import()
		{

		}

		public MySqlErrorCode ConnectToDatabase()
		{
			string ServerName = "localhost";
			string Database = Program.GetUserInput("Enter Destination DataBase Name: ");
			string Username = "root";
			SecureString Password = Program.GetUserInput("Enter MySql Password: ", true);

			string PlainPassword = new NetworkCredential(string.Empty, Password).Password; // Kind of defeats the purpose of using a SecureString, but I'm unsure of how else to transfer this to MySql

			string ConnectQuery = string.Format("SERVER={0}; DATABASE={1}; UID={2}; PASSWORD={3};", ServerName, Database, Username, PlainPassword);
			this.connection = new MySqlConnection(ConnectQuery);

			try
			{
				connection.Open();
				Console.WriteLine("Connected to MySql.");

				return MySqlErrorCode.Yes;
			}
			catch (MySqlException Exception)
			{
				return (MySqlErrorCode)Exception.Number;
			}
		}

		public void DisconnectFromDatabase()
		{
			if (this.connection != null)
			{
				Console.WriteLine("Disconnecting from MySql.");

				try
				{
					this.connection.Close();
					this.connection.Dispose();
				}
				catch (Exception) { }
			}
		}
	}
}
