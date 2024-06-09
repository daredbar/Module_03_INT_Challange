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

            // 2. Create reference array and point list
            ReferenceArray dimRefArrayHoriz = new ReferenceArray();
            ReferenceArray dimRefArrayVert = new ReferenceArray();
            List<XYZ> pointList = new List<XYZ>();

            //List<Curve> gridCurve = new List<Curve>();

            // 4. Loop through Line Curve 
            foreach (Element ele in gridCollector)
            {
                Grid grid = ele as Grid;
                if (grid != null)
                {
                    Curve lineCurve = grid.Curve;
                    //gridCurve.Add(lineCurve);
                    XYZ midPoint = lineCurve.Evaluate(1,true);


                    if (IsLineVertical(lineCurve))
                    {
                        dimRefArrayHoriz.Append(new Reference(grid));
                        pointList.Add(midPoint);
                    }
                    else if (IsLineVertical(lineCurve) == false)
                    {
                        dimRefArrayVert.Append(new Reference(grid));
                        pointList.Add(midPoint);
                    }
                }


            }
            XYZ point1 = pointList.First();
            XYZ point2 = pointList.Last();

            Line dimGridLine = Line.CreateBound(point1, point2);



            using (Transaction t = new Transaction(doc))
            {
                t.Start("Dimension Grids");
                Dimension gridDimHor = doc.Create.NewDimension(doc.ActiveView, dimGridLine, dimRefArrayHoriz); 
                //Dimension gridDimVert = doc.Create.NewDimension(doc.ActiveView, dimGridLine, dimRefArrayVert);
                t.Commit();
            }
            return Result.Succeeded;
        }

        private bool IsLineVertical(Curve curLine)
        {
            XYZ p1 = curLine.GetEndPoint(0);
            XYZ p2 = curLine.GetEndPoint(1);

            if (Math.Abs(p1.X - p2.X) < Math.Abs(p1.Y - p2.Y))
                return true;

            return false;
        }
        //private bool IsLineVertical(Line line)
        //{
        //    if (line.Direction.IsAlmostEqualTo(XYZ.BasisZ) || line.Direction.IsAlmostEqualTo(-XYZ.BasisZ))
        //        return true;
        //    else
        //        return false;
        //}

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
