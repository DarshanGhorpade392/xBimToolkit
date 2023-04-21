using Xbim.Ifc;

namespace DocumentationExamples.Utils
{
    class MyVariables
    {
        // Define the credentials of the editor who will be accessing the IFC file
        public static XbimEditorCredentials editor = new XbimEditorCredentials
        {
            ApplicationDevelopersName = "You",
            ApplicationFullName = "Your app",
            ApplicationIdentifier = "Your app ID",
            ApplicationVersion = "4.0",
            //your user
            EditorsFamilyName = "Santini Aichel",
            EditorsGivenName = "Johann Blasius",
            EditorsOrganisationName = "Independent Architecture"
        };
    }
}
