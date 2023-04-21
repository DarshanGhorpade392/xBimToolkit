using System;
using System.Linq;
using DocumentationExamples.Utils;
using Xbim.Ifc;
using Xbim.Ifc4.SharedBldgElements;

namespace DocumentationExamples.RetrieveGeometry
{
    public class WallGeometry
    {
        static void Main(string[] args)
        {
            try
            {
                using (var model = IfcStore.Open(Filepaths.SampleHouse, MyVariables.editor, -1))
                {
                    var allWalls = model.Instances.OfType<IfcWall>().ToList();
                    foreach (var wall in allWalls)
                    {
                        var ifcRepresentations = wall.Representation.Representations;
                        if (ifcRepresentations.Count != 0)
                        {
                            foreach (var ifcRepresentation in ifcRepresentations)
                            {
                                var ifcRepresentationItems = ifcRepresentation.Items;
                                foreach (var ifcRepresentationItem in ifcRepresentationItems)
                                {
                                    var s = ifcRepresentationItem;
                                }
                            }
                        }
                    }
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