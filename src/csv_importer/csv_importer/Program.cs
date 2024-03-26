using System.IO;
using System.Reflection;

namespace csv_importer
{
	public class Program
	{
		static string GetCurrentDirectory()
		{
			string Location = Assembly.GetExecutingAssembly().Location;

			return Path.GetDirectoryName(Location);
		}

		static void Main(string[] args)
		{
			Importer CSVImporter = new Importer();

			CSVImporter.AddFromDirectory(GetCurrentDirectory());
			CSVImporter.LoadFiles();
		}
	}
}
