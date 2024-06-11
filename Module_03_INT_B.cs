#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Media3D;

#endregion

namespace Module_03_INT_Challange
{
    [Transaction(TransactionMode.Manual)]
    public class Module_03_INT_B : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            // Collect Rooms
            FilteredElementCollector roomCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms);

            int dimenCount = 0;

            foreach (SpatialElement room in roomCollector)
            {
                //Reference curRef = 
                Room curRoom = room as Room;

                // Create reference array and point list
                ReferenceArray referenceArrayH = new ReferenceArray();
                ReferenceArray referenceArrayV = new ReferenceArray();
                List<XYZ> pointListH = new List<XYZ>();
                List<XYZ> pointListV = new List<XYZ>();

                // Set options and get room boundaries
                SpatialElementBoundaryOptions options = new SpatialElementBoundaryOptions();
                options.SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish;

                IList<IList<BoundarySegment>> boundSegList = curRoom.GetBoundarySegments(options);

                //  Loop through room boundaries
                foreach (IList<BoundarySegment> boundarySegments in boundSegList)
                {
                    foreach (BoundarySegment curSeg in boundarySegments)
                    {
                        // Get boundary geometry
                        Curve boundCurve = curSeg.GetCurve();
                        XYZ midPoint = boundCurve.Evaluate(0.25, true);

                        // Get boundary wall
                        Element curWall = doc.GetElement(curSeg.ElementId);
                        if (curWall == null) 
                            continue;

                        // Check if line is vertical
                        if (IsLineVertical(boundCurve))
                        {
                            // Add to ref and point array
                            referenceArrayH.Append(new Reference(curWall));
                            pointListH.Add(midPoint);
                        }
                        // Check if line is horizontal 
                        else
                        {
                            // Add to ref and point array
                            referenceArrayV.Append(new Reference(curWall));
                            pointListV.Add(midPoint);
                        }

                    }
                }
                //  Create line for dimension
                XYZ pointH1 = pointListH.First();
                XYZ pointH2 = pointListH.Last();
                Line dimLineH = Line.CreateBound(pointH1, new XYZ(pointH2.X, pointH1.Y, 0));
                //Line dimLine = Line.CreateBound(point1, new XYZ(point1.X, point2.Y, 0));
                XYZ pointV1 = pointListV.First();
                XYZ pointV2 = pointListV.Last();
                Line dimLineV = Line.CreateBound(pointV1, new XYZ(pointV1.X, pointV2.Y, 0));

                using (Transaction t = new Transaction(doc))
                {
                    t.Start("Dimension Room Walls");
                    Dimension newDim1 = doc.Create.NewDimension(doc.ActiveView, dimLineH, referenceArrayH);
                    dimenCount++;
                    Dimension newDim2 = doc.Create.NewDimension(doc.ActiveView, dimLineV, referenceArrayV);
                    dimenCount++;
                    t.Commit();
                }
            }
            TaskDialog.Show("Dimension Count", $"{dimenCount} Dimenstions added");

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

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
