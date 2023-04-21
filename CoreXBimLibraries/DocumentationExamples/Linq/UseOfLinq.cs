using System;
using System.Linq;
using Xbim.Ifc;
using Xbim.Ifc4.SharedBldgElements;

namespace DocumentationExamples
{
    class UseOfLinq
    {
        static void Main(string[] arg)
        {
            try
            {
                using (var model = IfcStore.Open(fileName, editor, -1))
                {
                    // Linq to get ids of wall
                    var ids = from wall in model.Instances.OfType<IfcWall>()
                        where wall.HasOpenings.Any()
                        select wall.GlobalId;

                    //equivalent expression using chained extensions of IEnumerable and lambda expressions
                    var ids1 =
                        model.Instances
                            .Where<IfcWall>(wall => wall.HasOpenings.Any())
                            .Select(wall => wall.GlobalId);

                    var allWindows = model.Instances.OfType<IfcWindow>().ToList();

                    var requiredWall = from wall in model.Instances.OfType<IfcWall>()
                        where wall.GlobalId.Equals("3cUkl32yn9qRSPvBJVyWw5")
                        select wall;
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
