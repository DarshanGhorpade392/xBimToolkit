using System;
using Xbim.Common;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.SharedBldgElements;
using Xbim.IO;

namespace DocumentationExamples
{
    class UpdateElementIFC
    {
        static void Main(string[] args)
        {
            try
            {
                using (var model = IfcStore.Open(fileName, editor, 0))
                {
                    //get existing door from the model
                    var id = "3cUkl32yn9qRSPvBJVyWYp";
                    var theDoor = model.Instances.FirstOrDefault<IfcDoor>(d => d.GlobalId == id);

                    //open transaction for changes
                    using (var txn = model.BeginTransaction("Doors modification"))
                    {
                        //create new property set with two properties
                        var pSetRel = model.Instances.New<IfcRelDefinesByProperties>(r =>
                        {
                            r.GlobalId = Guid.NewGuid();
                            r.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(pSet =>
                            {
                                pSet.Name = "New property set";
                                //all collections are always initialized
                                pSet.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(p =>
                                {
                                    p.Name = "First property";
                                    p.NominalValue = new IfcLabel("First value");
                                }));
                                pSet.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(p =>
                                {
                                    p.Name = "Second property";
                                    p.NominalValue = new IfcLengthMeasure(156.5);
                                }));
                            });
                        });

                        //change the name of the door
                        theDoor.Name += "_checked";
                        //add properties to the door
                        pSetRel.RelatedObjects.Add(theDoor);

                        //commit changes
                        txn.Commit();
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Message: {e.Message}");
                Console.WriteLine($"Source: {e.Source}");
                Console.WriteLine($"StackTrace: {e.StackTrace}");
            }
#if DEBUG
            Console.ReadKey();
#endif
        }
    }
}