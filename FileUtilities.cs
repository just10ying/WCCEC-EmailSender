using System.Collections;
using Pechkin;
using System.IO;
using System.Reflection;

namespace EmailSender
{
    class FileUtilities
    {
        private const string TEMPLATE_START_MARKER = "{{";
        private const string TEMPLATE_END_MARKER = "}}";

        public static string PopulateTemplate(string templatePath, Hashtable values)
        {
            string templateContents = ReadFile(templatePath);
            foreach (DictionaryEntry entry in values)
            {
                string toReplace = TEMPLATE_START_MARKER + entry.Key + TEMPLATE_END_MARKER;
                templateContents = templateContents.Replace(toReplace, entry.Value.ToString());
            }
            // Remove remaining unused template markers:
            templateContents.Replace(TEMPLATE_START_MARKER + ".+" + TEMPLATE_END_MARKER, "");
            return templateContents;
        }

        // No longer used, since templates are stored externally and not as a resource
        private static string ReadResource(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private static string ReadFile(string fileName)
        {
            return File.ReadAllText(fileName);
        }

        public static void SaveFile(string path, byte[] fileBytes)
        {
            File.WriteAllBytes(path, fileBytes);
        }
    }
}