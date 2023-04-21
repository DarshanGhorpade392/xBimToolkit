// get door details

using System;
using System.Linq;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.SharedBldgElements;

namespace DocumentationExamples
{
    class RetrieveWindowProperties
    {
        static void Main(string[] args)
        {
            var editor = new XbimEditorCredentials
            {
                ApplicationDevelopersName = "xbim developer",
                ApplicationFullName = "xbim toolkit",
                ApplicationIdentifier = "xbim",
                ApplicationVersion = "4.0",
                EditorsFamilyName = "Ghorpade",
                EditorsGivenName = "Darshan Ghorpade",
                EditorsOrganisationName = "Independent Architecture"
            };

            const string fileName = "D:\\Trainings\\XBimToolKitLibrary\\SampleHouse.ifc";


            // IfcStore takes a configuration object XbimEditorCredentials representing current application
            // and user and uses it to maintain OwnerHistory of root entities
            using (var model = IfcStore.Open(fileName, editor, -1))
            {
                //get all doors in the model (using IFC4 interface of IfcWindow this will work both for IFC2x3 and IFC4)
                var allWindows = model.Instances.OfType<IfcWindow>().ToList();

                //get only Windows with defined IIfcTypeObject
                var someWindows = model.Instances.Where<IfcWindow>(d => d.IsTypedBy.Any()).ToList();

                //get one single Window 
                var id = "3cUkl32yn9qRSPvBJVyWcE";
                var theWindow = model.Instances.FirstOrDefault<IIfcWindow>(d => d.GlobalId == id);
                Console.WriteLine($"Window ID: {theWindow.GlobalId}, Name: {theWindow.Name}");

                //get all single-value properties of the window
                var properties = theWindow.IsDefinedBy
                    .Where(r => r.RelatingPropertyDefinition is IIfcPropertySet)
                    .SelectMany(r => ((IIfcPropertySet)r.RelatingPropertyDefinition).HasProperties)
                    .OfType<IIfcPropertySingleValue>();
                foreach (var property in properties)
                    Console.WriteLine($"Property: {property.Name}, Value: {property.NominalValue}");
                Console.ReadKey();
            }
        }
    }
}

/*
 * IFCStore.Instances
 * Returns a list of the handles to only the entities in this model Note this do NOT include entities that are in any federated models
 */

/*
 *  IfcPropertySingleValue: defines a property object which has a single (numeric or descriptive) value assigned
 */

/*
 * The I stands for readonly interface for the original interface
 * IIfcPropertySingleValue
 */
