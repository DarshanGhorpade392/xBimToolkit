using System;
using System.Linq;
using DocumentationExamples.Utils;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.SharedBldgElements;

namespace DocumentationExamples
{
    class RetrieveWallProperties
    {
        static void Main(string[] args)
        {
            // IfcStore takes a configuration object XbimEditorCredentials representing current application
            // and user and uses it to maintain OwnerHistory of root entities
            try
            {
                using (var model = IfcStore.Open(Filepaths.SampleHouse, MyVariables.editor, -1))
                {
                    //get all walls in the model (using IFC4 interface of IfcWall this will work both for IFC2x3 and IFC4)
                    var allWalls = model.Instances.OfType<IfcWall>().ToList();

                    //get only Wall with defined IIfcTypeObject
                    var someWalls = model.Instances.Where<IfcWall>(d => d.IsTypedBy.Any()).ToList();

                    //get one single Wall 
                    var id = "3cUkl32yn9qRSPvBJVyWw5";
                    var theWall = model.Instances.FirstOrDefault<IIfcWall>(d => d.GlobalId == id);
                    Console.WriteLine($"Wall ID: {theWall.GlobalId}, Name: {theWall.Name}");

                    //get all single-value properties of the wall
                    var properties = theWall.IsDefinedBy
                        .Where(r => r.RelatingPropertyDefinition is IIfcPropertySet)
                        .SelectMany(r => ((IIfcPropertySet)r.RelatingPropertyDefinition).HasProperties)
                        .OfType<IIfcPropertySingleValue>();
                    foreach (var property in properties)
                        Console.WriteLine($"Property: {property.Name}, Value: {property.NominalValue}");
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