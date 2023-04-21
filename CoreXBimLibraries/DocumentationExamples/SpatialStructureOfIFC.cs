using System;
using System.Linq;
using Serilog;
using Serilog.Core;
using Xbim.Common;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;

namespace DocumentationExamples
{
    public class SpatialStructureOfIFC
    {
        static void Main(string[] args)
        {
            var log = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
            log.Information("Hello, Serilog!");

            Show();
        }

        public static void Show()
        {
            try
            {
                using (var model = IfcStore.Open(Filepaths.SampleHouse))
                {

                    var project = model.Instances.FirstOrDefault<IIfcProject>();
                    PrintHierarchy(project, 0);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void PrintHierarchy(IIfcObjectDefinition o, int level)
        {
            Console.WriteLine(string.Format("{0}{1} [{2}]", GetIndent(level), o.Name, o.GetType().Name));

            // only spatial elements can contain building elements
            if (o is IIfcSpatialElement spatialElement)     // pattern matching
            {
                // using IfcRelContainedInSpatialElement to get contained elements
                var containedElements = spatialElement.ContainsElements.SelectMany(rel => rel.RelatedElements);

                foreach (var element in containedElements)
                {
                    Console.WriteLine($"{GetIndent(level)}    ->{element.Name} [{element.GetType().Name}]");
                }
            }

            // using IfcRelAggregares to get spatial decomposition if spatial structure elements
            foreach (var item in o.IsDecomposedBy.SelectMany(r => r.RelatedObjects))
            {
                PrintHierarchy(item, level + 1);
            }
        }

        private static string GetIndent(int level)
        {
            var indent = "";
            for (int i = 0; i < level; i++)
                indent += "  ";
            return indent;
        }
    }
}