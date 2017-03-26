This project aims to accurately compile common programming languages and I/O features into Microsoft Excel (2015) files.

# How to Use

From the latest release version: use the command `Excel_VM myFile.file -outputExcelFile` (or `Excel_VM myFile.file` to get a Windows compatible executable file), replacing `myFile.file` with the name of your file.


The compiled Excel file can be executed by triggering iterative calculation (pressing F9 repeatedly) in Microsoft Excel. The project currently supports C and F# (incomplete). I/O operations are supported with cell A2 acting as standard input, and cell B2 acting as standard output.

### Note

By default, 500 iterations are made each time. To ensure accuracy, do not disturb the spreadsheet before it is finished recalculating.


Download the latest version [here](Excel_VM.zip).

# Examples

..for now, are included in the download. They'll be here shortly.

# Try It

coming soon