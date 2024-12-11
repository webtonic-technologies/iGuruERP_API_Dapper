using System.Data;
using System.Text;
using OfficeOpenXml; // Ensure EPPlus NuGet package is added

namespace FeesManagement_API.Utilities
{
    public static class FileExportHelper
    {
        public static byte[] ExportToCsv(DataTable dataTable)
        {
            var sb = new StringBuilder();

            // Add headers
            foreach (DataColumn column in dataTable.Columns)
            {
                sb.Append(column.ColumnName + ",");
            }
            sb.Length--; // Remove trailing comma
            sb.AppendLine();

            // Add rows
            foreach (DataRow row in dataTable.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    sb.Append(item + ",");
                }
                sb.Length--; // Remove trailing comma
                sb.AppendLine();
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public static byte[] ExportToExcel(DataTable dataTable)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("ChequeTracking");

            // Add headers
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                worksheet.Cells[1, i + 1].Value = dataTable.Columns[i].ColumnName;
            }

            // Add rows
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    worksheet.Cells[i + 2, j + 1].Value = dataTable.Rows[i][j];
                }
            }

            return package.GetAsByteArray();
        }
    }
}
