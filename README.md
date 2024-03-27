# MySQL CSV Importer

A tool for importing multiple Comma-Separated Values (CSV) files into MySQL. Written in C# \
Also comes with an HTML parser to convert HTML tables into usable CSV files.

## Usage

To use the importer, extract the `.zip` file from the Releases page and place all of your `.csv` files in the same location as the `csv_importer.exe`.
Next, simply run the `csv_importer.exe` file and enter information when prompted, the rest will be done for you.

To use the parser, extract the `.zip` file from the Releases page and run the `html_parser.exe`.
Then, copy and paste any HTML code into the displayed text box and save the generated `.csv` file wherever you like.

## Data Types

Data resembling a whole number will be treated as `INT` \
Data resembling a decimal number will be treated as `DOUBLE` 

Data in the format of `YYYY/MM/DD` will be converted to `YYYY-MM-DD` \
Data in the format of `YYYY/MM/DD HH:MM:SS` will be converted to `YYYY-MM-DD HH:MM:SS`

Data in the format of `YYYY-MM-DD` will be treated as `DATE` \
Data in the format of `YYYY-MM-DD HH:MM:SS` will be treated as `DATETIME` \
Data in the format of `YYYYMMDDHHMMSS` will be treated as `TIMESTAMP` \
Data in the format of `HH:MM:SS` will be treated as `TIME`

Data equal to `NULL` will remain as `NULL`

Any other type of data will be treated as `TEXT`

## Credits

The CSV Importer uses [MySql.Data](https://www.nuget.org/packages/MySql.Data/8.3.0/) by Oracle \
The HTML Parser uses [AngleSharp](https://www.nuget.org/packages/AngleSharp) by Florian Rappl
