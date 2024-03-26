using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Reflection;
using System.Security;

namespace csv_importer
{
	public class Program
	{
		public static string GetUserInput(string Prompt)
		{
			Console.Write(Prompt);

			return Console.ReadLine();
		}

		public static SecureString GetUserInput(string Prompt, bool Secure)
		{
			Console.Write(Prompt);

			SecureString Data = new SecureString();

			while (true)
			{
				ConsoleKeyInfo KeyInfo = Console.ReadKey(true);

				if (KeyInfo.Key == ConsoleKey.Enter)
					break;

				if (KeyInfo.Key == ConsoleKey.Backspace)
				{
					if (Data.Length > 0)
					{
						Data.RemoveAt(Data.Length - 1);
						Console.Write("\b \b");
					}

					continue;
				}

				if (KeyInfo.KeyChar != '\u0000')
				{
					Data.AppendChar(KeyInfo.KeyChar);
					Console.Write("*");
				}
			}

			Console.WriteLine();

			return Data;
		}

		public static void Pause()
		{
			Console.WriteLine();
			Console.WriteLine("Press any key to continue...");

			Console.ReadKey();
		}

		private static bool HandleConnection(MySqlErrorCode ConnectionCode)
		{
			if (ConnectionCode == MySqlErrorCode.Yes)
			{
				return true;
			}
			else
			{
				switch (ConnectionCode)
				{
					case MySqlErrorCode.None:
						Console.WriteLine("Failed to connect to Database. Make sure MySql is running.");
						break;

					case MySqlErrorCode.AccessDenied:
						Console.WriteLine("Invalid password supplied.");
						break;
				}

				Pause();

				return false;
			}
		}

		private static string GetCurrentDirectory()
		{
			string Location = Assembly.GetExecutingAssembly().Location;

			return Path.GetDirectoryName(Location);
		}

		static void Main(string[] args)
		{
			Importer CSVImporter = new Importer();

			if (!HandleConnection(CSVImporter.ConnectToDatabase()))
				return;

			CSVImporter.AddFromDirectory(GetCurrentDirectory());

			CSVImporter.LoadFiles();
			CSVImporter.Import();

			CSVImporter.DisconnectFromDatabase();

			Pause();
		}
	}
}
