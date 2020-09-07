using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Data.SqlClient;
//using System.ComponentModel;
//using System.Collections;
//using System.Windows.Forms;

using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using acd = Autodesk.AutoCAD.ApplicationServices.Application;

namespace AutoCadLibrary
{
    public class StartForm
    {
        Editor ed = acd.DocumentManager.MdiActiveDocument.Editor;

        // Запуск формы
        [CommandMethod("FSTART", CommandFlags.UsePickSet | CommandFlags.Redraw | CommandFlags.Modal)]
        public void Start()
        {
            Form1.FormShow();
        }
    }
    public class CancSel
    {
        Form1 fr1 = new Form1();
        Editor ed = acd.DocumentManager.MdiActiveDocument.Editor;
        Database db = HostApplicationServices.WorkingDatabase;

        // Снятие выделения на виде
        [CommandMethod("CANSEL", CommandFlags.UsePickSet | CommandFlags.Redraw | CommandFlags.Modal)]
        public void CancelSelection()
        {
            ed.Command("SELECTIONCYCLING", 0);
        }

        // Выделение вида на чертеже
        [CommandMethod("SELVIEW", CommandFlags.Redraw | CommandFlags.UsePickSet | CommandFlags.Modal)]
        public void selector()
        {
            PromptSelectionOptions pso = new PromptSelectionOptions();
            PromptSelectionResult selres = ed.GetSelection();
            if (selres.Status != PromptStatus.OK)
                ed.WriteMessage("\nНичего не выбрано!");
            else 
            {
                string view = "";
                List<string> viewarr = new List<string>();
                ed.WriteMessage("\nВыбрано объектов: {0} шт.\n", selres.Value.Count);
                using(Transaction tr = db.TransactionManager.StartTransaction())
                {
                    foreach(SelectedObject so in selres.Value)
                    {
                        try
                        {
                            DBText dt = (DBText)tr.GetObject(so.ObjectId, OpenMode.ForRead);
                            int i = new int();
                            if (dt.Layer == "20_TEXT" && dt.ColorIndex == 7)
                            {
                                if (dt.TextString.Length > view.Length)
                                    view = dt.TextString;
                            }
                            else if (dt.Layer == "20_POZ" && (dt.ColorIndex == 4 || dt.ColorIndex == 256) && int.TryParse(dt.TextString, out i))
                            {
                                viewarr.Add(dt.TextString);
                                //ed.WriteMessage("\nПОЗИЦИЯ!!! --- {0}  ", dt.TextString);
                            }
                        }
                        catch
                        { }
                    }
                    tr.Commit();
                    //ed.WriteMessage("\nОно!!!    {0}", view);
                }
                fr1.dataGridPull(view, viewarr.ToArray());
                ed.Command("SELECTIONCYCLING", 0);
            }
        }
        // Определение цвета
        [CommandMethod("CCOLOR", CommandFlags.Redraw | CommandFlags.UsePickSet | CommandFlags.Modal)]
        public void checkcolor()
        {
            PromptSelectionOptions pso = new PromptSelectionOptions();
            PromptSelectionResult selres = ed.GetSelection();
            if (selres.Status != PromptStatus.OK)
                ed.WriteMessage("\nНичего не выбрано!");
            else
            {
                ed.WriteMessage("\nВыбрано объектов: {0} шт.\n", selres.Value.Count);
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    foreach (SelectedObject so in selres.Value)
                    {
                        try
                        {
                            DBText dt = (DBText)tr.GetObject(so.ObjectId, OpenMode.ForRead);
                            ed.WriteMessage(dt.ColorIndex.ToString());
                        }
                        catch
                        { }
                    }
                    tr.Commit();
                }
                ed.Command("SELECTIONCYCLING", 0);
            }
        }
    }
}
