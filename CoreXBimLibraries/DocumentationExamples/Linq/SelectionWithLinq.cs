using System;
using System.Linq;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;

namespace DocumentationExamples
{
    class SelectionWithLinq
    {
        static void Main()
        {
            try
            {
                var model = IfcStore.Open(Filepaths.SampleHouse);
                using (var txn = model.BeginTransaction())
                {
                    //this will iterate over 47309 entities instead of just 9 you need in this case!
                    /*
                    foreach (var entity in model.Instances)
                    {
                        if (entity is IIfcWallStandardCase ||
                            entity is IIfcDoor ||
                            entity is IIfcWindow)
                        {
                            Console.WriteLine(entity.GetType().ToString());
                        }
                    }
                    */

                    // Using Linq
                    var requiredProducts = new IIfcProduct[0]
                        .Concat(model.Instances.OfType<IIfcWallStandardCase>())
                        .Concat(model.Instances.OfType<IIfcDoor>())
                        .Concat(model.Instances.OfType<IIfcWindow>());

                    //This will only iterate over entities you really need (9 in this case)
                    foreach (var product in requiredProducts)
                    {
                        Console.WriteLine(product.GetType().ToString());
                    }
                    txn.Commit();
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
