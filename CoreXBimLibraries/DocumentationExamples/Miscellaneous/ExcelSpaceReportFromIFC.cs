using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;

namespace DocumentationExamples
{
    class ExcelSpaceReportFromIFC
    {
        static void Main(string[] args)
        {
            // initialize NPOI workbook from the template
            var workBook = new XSSFWorkbook("D:\\Trainings\\XBimToolKitLibrary\\template.xlsx");
            var sheet = workBook.GetSheet("Spaces");

            //Create nice numeric formats with units. Units would need a LOT MORE care in real world. 
            //We just know that our current model has space areas in square meters and space volumes in cubic meters
            //Please note that the original data exported from Revit were wrong because volumes were 1000x bigger than they should be.
            //Data were fixed using xbim for this example.

            var areaFormat = workBook.CreateDataFormat();
            var areaFormatId = areaFormat.GetFormat("# ##0.00 [$m²]");
            var areaStyle = workBook.CreateCellStyle();
            areaStyle.DataFormat = areaFormatId;

            var volumeFormat = workBook.CreateDataFormat();
            var volumeFormatId = volumeFormat.GetFormat("# ##0.00 [$m³]");
            var volumeStyle = workBook.CreateCellStyle();
            volumeStyle.DataFormat = volumeFormatId;


            //Open IFC model. We are not going to change anything in the model so we can leave editor credentials out.
            using (var model = IfcStore.Open("D:\\Trainings\\XBimToolKitLibrary\\SampleHouse.ifc"))
            {
                //Get all spaces in the model. 
                //We use ToList() here to avoid multiple enumeration with Count() and foreach(){}
                var spaces = model.Instances.OfType<IIfcSpace>().ToList();
                //Set header content
                sheet.GetRow(0).GetCell(0)
                    .SetCellValue($"Space Report ({spaces.Count} spaces)");
                foreach (var space in spaces)
                {
                    //write report data
                    WriteSpaceRow(space, sheet, areaStyle, volumeStyle);
                }
            }

            //save report
            using (var stream = File.Create("spaces.xlsx"))
            {
                workBook.Write(stream);
                stream.Close();
            }

            //see the result if you have some SW associated with the *.xlsx
            Process.Start("spaces.xlsx");
        }

        private static void WriteSpaceRow(IIfcSpace space, ISheet sheet, object areaStyle, object volumeStyle)
        {
            var row = sheet.CreateRow(sheet.LastRowNum + 1);

            var name = space.Name;
            row.CreateCell(0).SetCellValue(name);

            var floor = GetFloor(space);
            row.CreateCell(1).SetCellValue(floor?.Name);

            var area = GetArea(space);
            if (area != null)
            {
                var cell = row.CreateCell(2);
                cell.CellStyle = (ICellStyle)areaStyle;

                //there is no guarantee it is a number if it came from property and not from a quantity
                if (area.UnderlyingSystemType == typeof(double))
                    cell.SetCellValue((double)(area.Value));
                else
                    cell.SetCellValue(area.ToString());
            }

            var volume = GetVolume(space);
            if (volume != null)
            {
                var cell = row.CreateCell(3);
                cell.CellStyle = (ICellStyle)volumeStyle;

                //there is no guarantee it is a number if it came from property and not from a quantity
                if (volume.UnderlyingSystemType == typeof(double))
                    cell.SetCellValue((double)(volume.Value));
                else
                    cell.SetCellValue(volume.ToString());
            }
        }

        private static IIfcBuildingStorey GetFloor(IIfcSpace space)
        {
            return
                //get all objectified relations which model decomposition by this space
                space.Decomposes

                    //select decomposed objects (these might be either other space or building storey)
                    .Select(r => r.RelatingObject)

                    //get only storeys
                    .OfType<IIfcBuildingStorey>()

                    //get the first one
                    .FirstOrDefault();
        }

        private static IIfcValue GetArea(IIfcProduct product)
        {
            //try to get the value from quantities first
            var area =
                //get all relations which can define property and quantity sets
                product.IsDefinedBy

                    //Search across all property and quantity sets. 
                    //You might also want to search in a specific quantity set by name
                    .SelectMany(r => r.RelatingPropertyDefinition.PropertySetDefinitions)

                    //Only consider quantity sets in this case.
                    .OfType<IIfcElementQuantity>()

                    //Get all quantities from all quantity sets
                    .SelectMany(qset => qset.Quantities)

                    //We are only interested in areas 
                    .OfType<IIfcQuantityArea>()

                    //We will take the first one. There might obviously be more than one area properties
                    //so you might want to check the name. But we will keep it simple for this example.
                    .FirstOrDefault()?
                    .AreaValue;

            if (area != null)
                return area;

            //try to get the value from properties
            return GetProperty(product, "Area");
        }

        private static IIfcValue GetVolume(IIfcProduct product)
        {
            var volume = product.IsDefinedBy
                .SelectMany(r => r.RelatingPropertyDefinition.PropertySetDefinitions)
                .OfType<IIfcElementQuantity>()
                .SelectMany(qset => qset.Quantities)
                .OfType<IIfcQuantityVolume>()
                .FirstOrDefault()?.VolumeValue;
            if (volume != null)
                return volume;
            return GetProperty(product, "Volume");
        }

        private static IIfcValue GetProperty(IIfcProduct product, string name)
        {
            return
                //get all relations which can define property and quantity sets
                product.IsDefinedBy

                    //Search across all property and quantity sets. You might also want to search in a specific property set
                    .SelectMany(r => r.RelatingPropertyDefinition.PropertySetDefinitions)

                    //Only consider property sets in this case.
                    .OfType<IIfcPropertySet>()

                    //Get all properties from all property sets
                    .SelectMany(pset => pset.HasProperties)

                    //lets only consider single value properties. There are also enumerated properties, 
                    //table properties, reference properties, complex properties and other
                    .OfType<IIfcPropertySingleValue>()

                    //lets make the name comparison more fuzzy. This might not be the best practise
                    .Where(p =>
                        string.Equals(p.Name, name, System.StringComparison.OrdinalIgnoreCase) ||
                        p.Name.ToString().ToLower().Contains(name.ToLower()))

                    //only take the first. In reality you should handle this more carefully.
                    .FirstOrDefault()?.NominalValue;
        }
    }
}
