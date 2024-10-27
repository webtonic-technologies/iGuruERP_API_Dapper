using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using OfficeOpenXml;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;
using Student_API.Services.Interfaces;
using System.Reflection.Metadata;
namespace Student_API.Services.Implementations
{
    public class CommonService : ICommonService
    {
        public async Task<ServiceResponse<string>> ExportDataToFile<T>(List<T> data, List<string> headers, int exportFormat, string fileName)
        {
            try
            {
                //if (data == null || !data.Any())
                //{
                //    return new ServiceResponse<string>(false, "No data found", null, 404);
                //}

                switch (exportFormat)
                {
                    case 1: // Excel format
                        return await ExportToExcel(data, headers, fileName);

                    case 2: // CSV format
                        return await ExportToCsv(data, headers, fileName);

                    case 3: // PDF format
                        return await ExportToPdf(data, headers, fileName);

                    default:
                        return new ServiceResponse<string>(false, "Invalid export format", null, 400);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }

        private async Task<ServiceResponse<string>> ExportToExcel<T>(List<T> data, List<string> headers, string fileName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Add headers dynamically, with Serial Number as the first column
                worksheet.Cells[1, 1].Value = "Serial Number";
                for (int i = 0; i < headers.Count; i++)
                {
                    worksheet.Cells[1, i + 2].Value = headers[i];
                }

                // Add data rows dynamically, with Serial Number as the first column
                var rowIndex = 2;
                if (data != null)
                {
                    int serialNumber = 1;
                    foreach (var item in data)
                    {
                        worksheet.Cells[rowIndex, 1].Value = serialNumber++; // Serial Number
                        var properties = typeof(T).GetProperties();
                        for (int i = 0; i < properties.Length; i++)
                        {
                            worksheet.Cells[rowIndex, i + 2].Value = properties[i].GetValue(item);
                        }
                        rowIndex++;
                    }
                }

                worksheet.Cells.AutoFitColumns();
                var excelFile = package.GetAsByteArray();
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports", $"{fileName}.xlsx");

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                await File.WriteAllBytesAsync(filePath, excelFile);

                return new ServiceResponse<string>(true, "Excel file generated successfully", filePath, 200);
            }
        }

        private async Task<ServiceResponse<string>> ExportToCsv<T>(List<T> data, List<string> headers, string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports", $"{fileName}.csv");
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            // Add "Serial Number" as the first header
            var csvLines = new List<string> { "Serial Number," + string.Join(",", headers) };

            if (data != null)
            {
                int serialNumber = 1;
                foreach (var item in data)
                {
                    var properties = typeof(T).GetProperties();
                    var values = properties.Select(p => p.GetValue(item)?.ToString().Replace(",", " "));

                    // Prepend the serial number to each row
                    csvLines.Add(serialNumber++ + "," + string.Join(",", values));
                }
            }

            await File.WriteAllLinesAsync(filePath, csvLines);

            return new ServiceResponse<string>(true, "CSV file generated successfully", filePath, 200);
        }



        private async Task<ServiceResponse<string>> ExportToPdf1<T>(List<T> data, List<string> headers, string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports", $"{fileName}.pdf");
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            // Create a PDF writer instance
            using (var writer = new PdfWriter(filePath))
            {
                using (var pdf = new PdfDocument(writer))
                {
                    var document = new iText.Layout.Document(pdf);

                    // Add table with column count equal to the headers
                    var table = new Table(headers.Count, true);

                    // Add header row
                    foreach (var header in headers)
                    {
                        table.AddCell(new Cell().Add(new Paragraph(header))
                                                .SetBold()
                                                .SetTextAlignment(TextAlignment.CENTER));
                    }

                    // Add data rows
                    if (data != null)
                    {
                        foreach (var item in data)
                        {
                            var properties = typeof(T).GetProperties();
                            foreach (var prop in properties)
                            {
                                var value = prop.GetValue(item)?.ToString() ?? "";
                                table.AddCell(new Cell().Add(new Paragraph(value))
                                                        .SetTextAlignment(TextAlignment.LEFT));
                            }
                        }
                    }

                    // Add the table to the document
                    document.Add(table);

                    // Close the document
                    document.Close();
                }
            }

            return new ServiceResponse<string>(true, "PDF file generated successfully", filePath, 200);
        }

        private async Task<ServiceResponse<string>> ExportToPdf<T>(List<T> data, List<string> headers, string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports", $"{fileName}.pdf");

            try
            {
                // Ensure the directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                // Create a new PDF document
                PdfSharp.Pdf.PdfDocument pdfDocument = new PdfSharp.Pdf.PdfDocument();
                PdfSharp.Pdf.PdfPage page = pdfDocument.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                // Set up font and other settings
                XFont font = new XFont("Verdana", 9);
                double margin = 40;
                double yPos = margin;
                double lineHeight = font.GetHeight(gfx) + 5;
                double tableWidth = page.Width - 2 * margin;
                double[] columnWidths = new double[headers.Count];
                double maxColumnWidth = tableWidth / headers.Count;

                // Measure column widths dynamically or set fixed sizes
                for (int i = 0; i < headers.Count; i++)
                {
                    columnWidths[i] = maxColumnWidth;
                }

                // Draw table headers
                DrawTableRow(gfx, headers.ToArray(), columnWidths, yPos, font, isHeader: true);
                yPos += lineHeight;

                // Draw data rows with text wrapping
                foreach (var item in data)
                {
                    var properties = typeof(T).GetProperties();
                    List<string> rowData = new List<string>();

                    foreach (var prop in properties)
                    {
                        string value = prop.GetValue(item)?.ToString() ?? "";
                        rowData.Add(value);
                    }

                    // Draw each row and update the Y position for the next row
                    yPos = DrawTableRow(gfx, rowData.ToArray(), columnWidths, yPos, font, isHeader: false);

                    // Check if the content goes beyond the page height
                    if (yPos + lineHeight > page.Height - margin)
                    {
                        page = pdfDocument.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        yPos = margin;
                    }
                }

                // Save the PDF document
                pdfDocument.Save(filePath);

                // Return the file path as a response
                return new ServiceResponse<string>(true, "PDF file generated successfully", filePath, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }

        // Helper method to draw a table row
        private double DrawTableRow(XGraphics gfx, string[] columns, double[] columnWidths, double yPos, XFont font, bool isHeader)
        {
            XBrush brush = XBrushes.Black;
            XBrush headerBrush = XBrushes.LightGray;
            double xPos = 40;  // Starting x position (left margin)
            double lineHeight = font.GetHeight(gfx) + 5;
            XPen pen = new XPen(XColors.Black, 0.5);

            for (int i = 0; i < columns.Length; i++)
            {
                string text = columns[i];
                double columnWidth = columnWidths[i];
                var rect = new XRect(xPos, yPos, columnWidth, lineHeight);

                // Fill background color for headers
                if (isHeader)
                {
                    gfx.DrawRectangle(headerBrush, rect);
                    gfx.DrawString(text, font, brush, rect, XStringFormats.Center);
                }
                else
                {
                    // Word wrapping logic
                    XTextFormatter tf = new XTextFormatter(gfx);
                    tf.Alignment = XParagraphAlignment.Left;
                    tf.DrawString(text, font, brush, new XRect(xPos, yPos, columnWidth, lineHeight * 2)); // Allow for 2 lines of text per cell
                }

                // Draw cell borders
                gfx.DrawRectangle(pen, rect);
                xPos += columnWidth; // Move to the next column
            }

            return yPos + lineHeight;  // Return updated y position for the next row
        }





    }
}
