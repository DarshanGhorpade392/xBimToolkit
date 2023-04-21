using System;
using DocumentationExamples.Utils;
using Xbim.Common;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.SharedBldgElements;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;
using Xbim.IO;

namespace DocumentationExamples
{
    public class CreateBasicWallIFCFile
    {
        static void Main(string[] args)
        {
            try
            {
                using (var model = IfcStore.Create(MyVariables.editor, XbimSchemaVersion.Ifc4, XbimStoreType.InMemoryModel))
                {
                    using (var txn = model.BeginTransaction("Hello Wall"))
                    {
                        //there should always be one project in the model
                        var project = model.Instances.New<IfcProject>(p => p.Name = "Basic Wall Creation");

                        // Define basic default units
                        project.Initialize(ProjectUnits.SIUnitsUK);

                        // create a simple wall object
                        var wall = model.Instances.New<IfcWall>(w => w.Name = "The very first wall");

                        // set few basic properties
                        model.Instances.New<IfcRelDefinesByProperties>(rel =>
                        {
                            rel.RelatedObjects.Add(wall);
                            rel.RelatingPropertyDefinition =
                                model.Instances.New<IfcPropertySet>(pset =>
                                {
                                    pset.Name = "Basic set of properties";
                                    pset.HasProperties.AddRange(new[]
                                    {
                                        model.Instances.New<IfcPropertySingleValue>(p =>
                                        {
                                            p.Name = "Text Property";
                                            p.NominalValue = new IfcText("Default Text");
                                        }),
                                        model.Instances.New<IfcPropertySingleValue>(p =>
                                        {
                                            p.Name = "Length Property";
                                            p.NominalValue = new IfcLengthMeasure(100.0);
                                        }),
                                        model.Instances.New<IfcPropertySingleValue>(p =>
                                        {
                                            p.Name = "Number Property";
                                            p.NominalValue = new IfcNumericMeasure(789.2);
                                        }),
                                        model.Instances.New<IfcPropertySingleValue>(p =>
                                        {
                                            p.Name = "Logical Property";
                                            p.NominalValue = new IfcLogical(true);
                                        }),
                                    });
                                });
                        });
                        txn.Commit();
                    }
                    model.SaveAs("D:\\Trainings\\XBimToolKitLibrary\\BasicWall.ifc");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Message: {e.Message}");
                Console.WriteLine($"Source: {e.Source}");
                Console.WriteLine($"StackTrace: {e.StackTrace}");
            }
        }
    }
}