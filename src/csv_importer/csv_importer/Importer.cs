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

		MySqlConnection SQLConnection;

		public Importer()
		{
			this.Files = new Dictionary<string, string>();
			this.CSVFiles = new List<CSVData>();

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

		public string FormatQuery(string Base, params string[] Format)
		{
			return string.Format(Base, Format);
		}

		public void SendQuery(string Query, params string[] Format)
		{
			MySqlCommand Command = new MySqlCommand(this.FormatQuery(Query, Format), this.SQLConnection);
			Command.ExecuteNonQuery();
		}

		public void Import()
		{
			Console.WriteLine("Starting import...");

			foreach (CSVData CSV in this.CSVFiles)
			{
				this.SendQuery("DROP TABLE IF EXISTS {0};", CSV.TableName);

				string TableQuery = CSV.GetTableQuery();
				Console.WriteLine("Creating table {0} with types ( {1} ) => {2}", CSV.TableName, string.Join(", ", CSV.ColumnDataTypes), TableQuery);

				this.SendQuery(TableQuery);

				for (int i = 0; i < CSV.RowData.Count; i++)
				{
					string RowQuery = CSV.GetRowQuery(i);
					Console.WriteLine("Importing row with ( {0} ) => {1}", CSV.GetQueryRows(CSV.RowData[i]), RowQuery);

					this.SendQuery(RowQuery);
				}
			}
		}

		public MySqlErrorCode ConnectToDatabase()
		{
			string ServerName = "localhost";
			string Database = Program.GetUserInput("Enter Destination DataBase Name: ");
			string Username = "root";
			SecureString Password = Program.GetUserInput("Enter MySql Password: ", true);

			string PlainPassword = new NetworkCredential(string.Empty, Password).Password; // Kind of defeats the purpose of using a SecureString, but I'm unsure of how else to transfer this to MySql

			string ConnectQuery = string.Format("SERVER={0}; UID={2}; PASSWORD={3};", ServerName, Database, Username, PlainPassword);
			this.SQLConnection = new MySqlConnection(ConnectQuery);

			try
			{
				this.SQLConnection.Open();
				Console.WriteLine("Connected to MySql.");

				this.SendQuery("CREATE DATABASE IF NOT EXISTS {0};", Database);
				this.SQLConnection.ChangeDatabase(Database);

				Console.WriteLine("Attached to database.");

				return MySqlErrorCode.Yes;
			}
			catch (MySqlException Exception)
			{
				return (MySqlErrorCode)Exception.Number;
			}
		}

		public void DisconnectFromDatabase()
		{
			if (this.SQLConnection != null)
			{
				Console.WriteLine("Disconnecting from MySql.");

				try
				{
					this.SQLConnection.Close();
					this.SQLConnection.Dispose();
				}
				catch (Exception) { }
			}
		}
	}
}
