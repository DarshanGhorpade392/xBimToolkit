using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DocumentationExamples.Utils;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.SharedBldgElements;

namespace DocumentationExamples
{
    public class RetrieveRoofProperties
    {
        static void Main(string[] args)
        {

            try
            {
                // Open the IFC file and create a model object to access its data
                using (var model = IfcStore.Open(Filepaths.SampleHouse, MyVariables.editor, -1))
                {
                    var roofDataList = new List<Dictionary<string, string>>();
                    // Retrieve all IfcRoof objects from the IFC file and store them in a list
                    var allRoofs = model.Instances.OfType<IfcRoof>().ToList();

                    foreach (var roof in allRoofs)
                    {
                        Debug.Assert(roof != null, "roof is null! this could totally be a bug!");

                        var roofData = new Dictionary<string, string>
                        {
                            { "ID", roof.GlobalId }
                        };

                        // Retrieve all the properties of the current roof and store them in a list
                        var properties = roof.IsDefinedBy
                            .Where(r => r.RelatingPropertyDefinition is IIfcPropertySet)
                            .SelectMany(r => ((IIfcPropertySet)r.RelatingPropertyDefinition).HasProperties)
                            .OfType<IIfcPropertySingleValue>();

                        // Output each property of the current roof to the console
                        foreach (var property in properties)
                            roofData[property.Name] = property.NominalValue.ToString();

                        roofDataList.Add(roofData);
                    }

                    foreach (var roofData in roofDataList)
                    {
                        Console.WriteLine($"ID : {roofData["ID"]}");
                        foreach (var property in roofData.Where(p => p.Key != "ID"))
                        {
                            Console.WriteLine($"{property.Key} : {property.Value}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
#if DEBUG
            Console.ReadKey();
#endif
        }
    }
}