using System.Text.RegularExpressions;

namespace csv_importer
{
	public class StringCleaner
	{
		private Regex FileNameCleaner;
		private Regex ColumnNameCleaner;

		public StringCleaner()
		{
			this.FileNameCleaner = new Regex("[^a-zA-Z_]");
			this.ColumnNameCleaner = new Regex("[^a-zA-Z_]");
		}

		public string CleanFileName(string FileName)
		{
			FileName = FileName.ToLower();

			return this.FileNameCleaner.Replace(FileName, "");
		}

		public string CleanColumnName(string ColumnName)
		{
			ColumnName = ColumnName.ToLower();

			return this.ColumnNameCleaner.Replace(ColumnName, "");
		}
	}
}
