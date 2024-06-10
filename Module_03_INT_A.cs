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
            List<XYZ> pointListHoriz = new List<XYZ>();
            List<XYZ> pointListVert = new List<XYZ>();

            //List<Curve> gridCurve = new List<Curve>();

            // 4. Loop through Line Curve 
            foreach (Element ele in gridCollector)
            {
                Grid grid = ele as Grid;
                if (grid != null)
                {
                    Curve lineCurve = grid.Curve;
                    //gridCurve.Add(lineCurve);
                    XYZ pointInt = lineCurve.Evaluate(0.99,true);


                    if (IsLineVertical(lineCurve))
                    {
                        dimRefArrayHoriz.Append(new Reference(grid));
                        pointListHoriz.Add(pointInt);
                    }
                    else 
                    {
                        dimRefArrayVert.Append(new Reference(grid));
                        pointListVert.Add(pointInt);
                    }
                }
            }

            //  Create line for dimension
            XYZ pointH1 = pointListHoriz.First();
            XYZ pointH2 = pointListHoriz.Last();
            //XYZ pointH1 = GetOffsetOrientation(pointListHoriz.First(),gridCurve.Orientation, 3);
            //XYZ pointH2 = GetOffsetOrientation(pointListHoriz.Last(), gridCurve.Orientation, 3);

            XYZ pointV1 = pointListVert.First();
            XYZ pointV2 = pointListVert.Last();

            Line dimGridLineH = Line.CreateBound(pointH1, pointH2);
            Line dimGridLineV = Line.CreateBound(pointV1, pointV2);



            using (Transaction t = new Transaction(doc))
            {
                t.Start("Dimension Grids");
                Dimension gridDimHor = doc.Create.NewDimension(doc.ActiveView, dimGridLineH, dimRefArrayHoriz); 
                Dimension gridDimVert = doc.Create.NewDimension(doc.ActiveView, dimGridLineV, dimRefArrayVert);
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
        private XYZ GetOffsetOrientation(XYZ point, XYZ orientation, int value)
        {
            XYZ newVector = orientation.Multiply(value);
            XYZ returnPoint = point.Add(newVector);

            return returnPoint;
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
