using System;
using System.IO;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

namespace SCS
{
    class ExcelHelper : IDisposable
    {
        private Excel.Application excel;
        private Excel.Workbook workbook;
        private string filePath;

        public ExcelHelper()
        {
            excel = new Excel.Application();
        }
        internal bool Open(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    workbook = excel.Workbooks.Add();
                    this.filePath = filePath;
                }
                else
                {
                    workbook = excel.Workbooks.Add();
                    this.filePath = filePath;
                }

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return false;
        }
        internal void Save()
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                workbook.SaveAs(filePath);
                filePath = null;
            }
            else
            {
                workbook.Save();
            }
        }

        internal bool Set(string column, int row, object data)
        {
            try
            {
                ((Excel.Worksheet)excel.ActiveSheet).Cells[row, column] = data;
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return false;
        }

        public void Dispose()
        {
            try
            {
                workbook.Close();
                excel.Quit();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

    }
}

