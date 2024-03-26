using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace csv_importer
{
	public class CSVParser
	{
		private CSVData CSV;

		public CSVParser()
		{

		}

		private void ParseTopRow(string[] Row)
		{
			foreach (string Column in Row)
				this.CSV.AddColumn(Column);
		}

		private void LoadBasicData(string FilePath)
		{
			using (TextFieldParser Parser = new TextFieldParser(FilePath))
			{
				Parser.TextFieldType = FieldType.Delimited;
				Parser.SetDelimiters(",");

				while (!Parser.EndOfData)
				{
					string[] Row = Parser.ReadFields();

					// Initialize columns
					if (this.CSV.ColumnNames.Count < 1)
					{
						this.ParseTopRow(Row);
						continue;
					}

					// Handle rows
					this.CSV.AddRow(Row);
				}
			}
		}

		// <<List of Column Names, List of Column Types>, List of <Column Name, Data>>
		public CSVData HandleFile(string FilePath, string TableName)
		{
			if (!File.Exists(FilePath))
				return null;

			this.CSV = new CSVData(TableName);

			this.LoadBasicData(FilePath);

			this.CSV.AverageColumnTypes();

			return this.CSV;
		}
	}
}
