// get door details

using System;
using System.Linq;
using DocumentationExamples.Utils;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;

namespace DocumentationExamples
{
    class RetrieveDoorProperties
    {
        static void Main(string[] args)
        {
            // IfcStore takes a configuration object XbimEditorCredentials representing current application
            // and user and uses it to maintain OwnerHistory of root entities
            try
            {
                using (var model = IfcStore.Open(Filepaths.SampleHouse, MyVariables.editor, -1))
                {
                    //get all doors in the model (using IFC4 interface of IfcDoor this will work both for IFC2x3 and IFC4)
                    var allDoors = model.Instances.OfType<IIfcDoor>().ToList();

                    //get only doors with defined IIfcTypeObject
                    var someDoors = model.Instances.Where<IIfcDoor>(d => d.IsTypedBy.Any()).ToList();

                    //get one single door 
                    var id = "3cUkl32yn9qRSPvBJVyWYp";
                    var theDoor = model.Instances.FirstOrDefault<IIfcDoor>(d => d.GlobalId == id);
                    Console.WriteLine($"Door ID: {theDoor.GlobalId}, Name: {theDoor.Name}");

                    //get all single-value properties of the door
                    var properties = theDoor.IsDefinedBy
                        .Where(r => r.RelatingPropertyDefinition is IIfcPropertySet)
                        .SelectMany(r => ((IIfcPropertySet)r.RelatingPropertyDefinition).HasProperties)
                        .OfType<IIfcPropertySingleValue>();
                    foreach (var property in properties)
                        Console.WriteLine($"Property: {property.Name}, Value: {property.NominalValue}");
#if DEBUG
                    Console.ReadKey();
#endif
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

/*
 * IFCStore.Instances
 * Returns a list of the handles to only the entities in this model Note this do NOT include entities that are in any federated models
 */
