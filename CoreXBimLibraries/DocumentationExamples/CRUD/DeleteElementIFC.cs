using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;

namespace DocumentationExamples
{
    class DeleteElementIFC
    {
        static void Main(string[] args)
        {
            try
            {
                using (var model = IfcStore.Open(fileName))
                {
                    //get existing door from the model
                    var id = "3cUkl32yn9qRSPvBJVyWYp";  //use some existing ID from your model
                    var theDoor = model.Instances.FirstOrDefault<IIfcDoor>(d => d.GlobalId == id);

                    //open transaction for changes
                    using (var txn = model.BeginTransaction("Delete the door"))
                    {
                        //delete the door
                        model.Delete(theDoor);
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

            Console.ReadKey();
        }
    }
}
