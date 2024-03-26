using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace html_parser
{
	public partial class Main : Form
	{
		public Main()
		{
			InitializeComponent();
		}

		private void Main_Load(object sender, EventArgs e)
		{

		}

		private void ConvertButton_Click(object sender, EventArgs e)
		{
			string HTML = InputBox.Text;
			if (HTML.Length < 1) return;

			HtmlParser Parser = new HtmlParser();
			IHtmlDocument Document = Parser.ParseDocument(HTML);

			IElement Table = Document.QuerySelector("table");
			if (Table == null) return;

			List<string> ColumnHeaders = new List<string>();
			List<string> Rows = new List<string>();

			// Get all the DOM data
			foreach (IElement Row in Table.QuerySelectorAll("tr"))
			{
				// Assume this is the header row
				if (Row.QuerySelector("th") != null)
				{
					ColumnHeaders.Clear();

					foreach (IElement TableHeader in Row.QuerySelectorAll("th"))
						ColumnHeaders.Add(TableHeader.InnerHtml);

					continue;
				}

				// Handle rows
				List<string> CurrentRow = new List<string>();

				foreach (IElement RowData in Row.QuerySelectorAll("td"))
					CurrentRow.Add(RowData.InnerHtml);

				Rows.Add(string.Join(",", CurrentRow));
			}

			// Turn it into something useful
			string CSV = string.Format("{0}\n{1}", string.Join(",", ColumnHeaders), string.Join("\n", Rows));
			if (CSV.Length < 1)
			{
				MessageBox.Show("No data to write!");
				return;
			}

			// Save it!
			// Very organized code
			SaveFileDialog Dialog = new SaveFileDialog()
			{
				Filter = "Comma Separated File|*.csv",
				Title = "Choose CSV location"
			};

			Dialog.ShowDialog();

			if (Dialog.FileName.Length > 0)
			{
				using (FileStream CSVStream = (FileStream)Dialog.OpenFile())
				{
					if (!CSVStream.CanWrite)
					{
						MessageBox.Show("Unable to write file!");
						return;
					}

					try
					{
						byte[] Data = (new UTF8Encoding(true)).GetBytes(CSV);

						CSVStream.Write(Data, 0, Data.Length);
					}
					catch (Exception)
					{
						MessageBox.Show("Failed to write file!");
					}
				}
			}
		}
	}
}
