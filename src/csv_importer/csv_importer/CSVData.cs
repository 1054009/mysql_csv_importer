using System.Collections.Generic;
using System.Linq;

namespace csv_importer
{
	public class CSVData
	{
		public List<string> ColumnNames { get; protected set; }

		public List<MySQL_Type> ColumnDataTypes { get; protected set; }

		public List<List<string>> RowData { get; protected set; }

		private StringCleaner Cleaner;

		private DataTypes TypeManager;

		public CSVData()
		{
			this.ColumnNames = new List<string>();
			this.ColumnDataTypes = new List<MySQL_Type>();
			this.RowData = new List<List<string>>();

			this.Cleaner = new StringCleaner();
			this.TypeManager = new DataTypes();

			this.Clear(); // Redundant
		}

		public void Clear()
		{
			this.ColumnNames.Clear();
			this.ColumnDataTypes.Clear();
			this.RowData.Clear();
		}

		public int AddColumn(string Name)
		{
			this.ColumnNames.Add(Name);
			this.ColumnDataTypes.Add(MySQL_Type.TEXT);

			return this.ColumnNames.Count - 1;
		}

		public void SetColumnType(int Index, MySQL_Type Type)
		{
			if (Index < 0 || Index >= this.ColumnDataTypes.Count)
				return;

			this.ColumnDataTypes[Index] = Type;
		}

		public void AverageColumnType(int Index)
		{
			if (Index < 0 || Index >= this.ColumnNames.Count)
				return;

			// Collect all the types in the column
			List<MySQL_Type> Types = new List<MySQL_Type>();

			foreach (List<string> Row in this.RowData)
			{
				string RowColumn = Row[Index];

				Types.Add(this.TypeManager.GetTypeOf(RowColumn));
			}

			// Find the most common one
			MySQL_Type Common = Types.GroupBy(Type => Type)
				.OrderByDescending(Group => Group.Count())
				.Select(Group => Group.Key)
				.First();

			// Apply the most common type
			this.SetColumnType(Index, Common);
		}

		public void AverageColumnTypes()
		{
			for (int i = 0; i < this.ColumnNames.Count; i++)
				this.AverageColumnType(i);
		}

		public void AddRow(string[] Row)
		{
			List<string> ListRow = new List<string>(Row);

			// Prevent misalignment
			while (ListRow.Count < this.ColumnNames.Count) ListRow.Add("");
			while (ListRow.Count > this.ColumnNames.Count) ListRow.RemoveAt(ListRow.Count - 1);

			for (int i = 0; i < ListRow.Count; i++)
				ListRow[i] = this.Cleaner.FixRowBlock(ListRow[i]);

			this.RowData.Add(ListRow);
		}
	}
}
