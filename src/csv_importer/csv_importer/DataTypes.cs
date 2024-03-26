using System;
using System.Text.RegularExpressions;

namespace csv_importer
{
	public enum MySQL_Type
	{
		INVALID = -1,
		NULL = 0,
		INT,
		DOUBLE,
		DATE,
		DATETIME,
		TIMESTAMP,
		TIME,
		TEXT
	};

	// This can probably be cleaned up with templates, but I don't know how to use those :^)
	public class DataTypes
	{
		// Lazy part 1
		private static string DATE_Regex = "^([0-9]{2,4})-([0-1][0-9])-([0-3][0-9])$";
		private static string DATETIME_Regex = "^([0-9]{2,4})-([0-1][0-9])-([0-3][0-9])(?:( [0-2][0-9]):([0-5][0-9]):([0-5][0-9]))$";
		private static string TIMESTAMP_Regex = "^([0-9]{2,4})([0-1][0-9])([0-3][0-9])(?:([0-2][0-9])([0-5][0-9])([0-5][0-9]))$";
		private static string TIME_Regex = "^([0-2][0-9]):([0-5][0-9]):([0-5][0-9])$";

		public DataTypes()
		{

		}

		// INT
		// TINYINT, SMALLINT, MEDIUMINT, BIGINT are unused
		public MySQL_Type TryInt(string Block)
		{
			Int32 INT_Output;
			if (Int32.TryParse(Block, out INT_Output))
				return MySQL_Type.INT;

			return MySQL_Type.INVALID;
		}

		// DOUBLE, DECIMAL
		// FLOAT is unused
		public MySQL_Type TryDouble(string Block)
		{
			Double DOUBLE_Output;
			if (Double.TryParse(Block, out DOUBLE_Output))
				return MySQL_Type.DOUBLE;

			return MySQL_Type.INVALID;
		}

		// DATE, DATETIME, TIMESTAMP, TIME
		public MySQL_Type TryTime(string Block)
		{
			if (Regex.IsMatch(Block, DATE_Regex)) return MySQL_Type.DATE;
			if (Regex.IsMatch(Block, DATETIME_Regex)) return MySQL_Type.DATETIME;
			if (Regex.IsMatch(Block, TIMESTAMP_Regex)) return MySQL_Type.TIMESTAMP;
			if (Regex.IsMatch(Block, TIME_Regex)) return MySQL_Type.TIME;

			// Lazy part 2
			// Cheeky bastards
			Block = Block.Replace("/", "-");
			if (Regex.IsMatch(Block, DATE_Regex)) return MySQL_Type.DATE;
			if (Regex.IsMatch(Block, DATETIME_Regex)) return MySQL_Type.DATETIME;

			return MySQL_Type.INVALID;
		}

		public MySQL_Type GetTypeOf(string Block)
		{
			MySQL_Type Output = MySQL_Type.INVALID;

			if ((Output = this.TryInt(Block)) != MySQL_Type.INVALID) return Output;
			if ((Output = this.TryDouble(Block)) != MySQL_Type.INVALID) return Output;
			if ((Output = this.TryTime(Block)) != MySQL_Type.INVALID) return Output;

			// CHAR, VARCHAR, TINYTEXT, BLOB, MEDIUMTEXT, MEDIUMBLOB are unused
			// LONGTEXT, LONGBLOB are unsupported
			return MySQL_Type.TEXT;
		}
	}
}
