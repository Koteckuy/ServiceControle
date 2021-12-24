using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Word = Microsoft.Office.Interop.Word;

namespace SCS
{
    class WordHelper
    {
        Word.Application wordApp;
        Word.Document document;
        object objMiss = Missing.Value;
        object TmpFile = Path.GetTempPath() + "Act.doc";
        object file = @"C:\Users\MSI\Desktop\Act.doc";

        public WordHelper(Orders order, Dictionary<int, object> services)
        {
            try
            {
                wordApp = new Word.Application();
                document = wordApp.Documents.Open(ref file, ref objMiss, ref objMiss, ref objMiss, ref objMiss, ref objMiss, ref objMiss, ref objMiss, ref objMiss, ref objMiss, ref objMiss, ref objMiss, ref objMiss, ref objMiss, ref objMiss, ref objMiss);

                Dictionary<string, string> items = new Dictionary<string, string>
                {
                    { "<OrderID>", order.ID.ToString()},
                    { "<OrderDate>", order.OrderDate.ToString()},
                    { "<Master>", order.Master.ToString()},
                    { "<MasterPhone>", order.Master.Phone},
                    { "<Client>", order.Device.Client.ToString()},
                    { "<ClientPhone>", order.Device.Client.Phone},
                    { "<OrderPriceRuble>", ((int)order.Price).ToString()},
                    { "<OrderPriceMonet>", (order.Price - (int)order.Price).ToString().Replace("0,","") + "0"},
                    { "<OrderVATRuble>", ((int)order.Price * 0.2).ToString()},
                    { "<OrderVATMonet>", (order.Price * 0.2 - (int)(order.Price * 0.2)).ToString().Replace("0,","") + "0"}
                };
                FindAndReplace(items);

                Word.Table table = document.Tables[1];

                for (int i = 2; i < services.Count+2; i++)
                {
                    table.Rows.Add(ref objMiss);
                    table.Cell(i, 1).Range.Text = $"{i-1}";
                    table.Cell(i, 2).Range.Text = (services[i - 2] as ServiceOrder).Service.Name;
                    table.Cell(i, 3).Range.Text = (services[i - 2] as ServiceOrder).Service.Description;
                    table.Cell(i, 4).Range.Text = $"{(services[i - 2] as ServiceOrder).Service.Price}";
                }

                document.SaveAs2(@"C:\Users\MSI\Desktop\Act " + order.ID + ".doc");
            } 
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                document.Close(WdSaveOptions.wdDoNotSaveChanges, WdOriginalFormat.wdOriginalDocumentFormat, false);
                wordApp.Quit(WdSaveOptions.wdDoNotSaveChanges);
            }
        }

        void FindAndReplace(Dictionary<string, string> items)
        {
            foreach (var item in items)
            {
                Word.Find find = wordApp.Selection.Find;
                find.Text = item.Key;
                find.Replacement.Text = item.Value;

                Object wrap = Word.WdFindWrap.wdFindContinue;
                Object replace = Word.WdReplace.wdReplaceAll;

                find.Execute(FindText: Type.Missing,
                    MatchCase: false,
                    MatchWholeWord: false,
                    MatchWildcards: false,
                    MatchSoundsLike: objMiss,
                    MatchAllWordForms: false,
                    Forward: true,
                    Wrap: wrap,
                    Format: false,
                    ReplaceWith: objMiss, 
                    Replace: replace);
            }
        }
    }
}