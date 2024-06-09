#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

#endregion

namespace Module_03_INT_Challange
{
    [Transaction(TransactionMode.Manual)]
    public class Module_03_INT_A : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            // Your code goes here
            FilteredElementCollector gridCollector = new FilteredElementCollector(doc).OfClass(typeof(Grid));

            List<Curve> gridCurve = new List<Curve>();

            // 2. Create reference array and point list
            ReferenceArray dimRefArray = new ReferenceArray();
            List<XYZ> pointList = new List<XYZ>();

            // 4. Loop through Line Curve 
            foreach (Element ele in gridCollector)
            {
                Curve lineCurve = ele.GetCurve();


                if (IsLineVertical(lineCurve))
            }




            using (Transaction t = new Transaction(doc))
            {
                t.Start("Dimension Grids");
                Dimension gridDimHor = doc.Create.NewDimension(doc.ActiveView, dimGridLine, dimRefArray); 
                Dimension gridDimVert = doc.Create.NewDimension(doc.ActiveView, dimGridLine, dimRefArray);
                t.Commit();
            }
            return Result.Succeeded;
        }

        private bool IsLineVertical(Line line)
        {
            if (line.Direction.IsAlmostEqualTo(XYZ.BasisZ) || line.Direction.IsAlmostEqualTo(-XYZ.BasisZ))
                return true;
            else
                return false;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
