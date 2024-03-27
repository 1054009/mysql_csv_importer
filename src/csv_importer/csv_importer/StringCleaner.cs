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

		public string FixRowBlock(string Block)
		{
			MySQL_Type Type = (new DataTypes()).GetTypeOf(Block);

			if (Type != MySQL_Type.TEXT)
				Block = Block.Trim();

			// Fix empty spots causing rows to not be imported at all
			if (Block.Length < 1)
				return "NULL";

			return Block;
		}

		public string FixSQLString(string Block)
		{
			if (Block.Equals("NULL")) // Let nulls chill
				return Block;

			MySQL_Type Type = (new DataTypes()).GetTypeOf(Block);

			if (Type == MySQL_Type.DATE || Type == MySQL_Type.DATETIME)
				Block = Block.Replace("/", "-");

			if (Type == MySQL_Type.TEXT || Type == MySQL_Type.DATE || Type == MySQL_Type.DATETIME || Type == MySQL_Type.TIMESTAMP || Type == MySQL_Type.TIME)
				return string.Format("'{0}'", Block);

			return Block;
		}
	}
}
