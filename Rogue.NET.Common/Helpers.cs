using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Rogue.NET.Common
{
    public static class Helpers
    {
        public static bool IsValidFileName(string fileName)
        {
            Regex containsABadCharacter = new Regex("[" + Regex.Escape(new string(Path.GetInvalidFileNameChars())) + "]");
            if (containsABadCharacter.IsMatch(fileName))
                return false;

            return true;
        }

        public static string ToCamelCase(this string str)
        {
            if (!string.IsNullOrEmpty(str))
                return str.Substring(0, 1).ToLower() + str.Substring(1, str.Length - 1);

            return str;
        }

        public static string FriendlyName(this Type type)
        {
            if (type == typeof(bool))
                return "bool";

            if (type == typeof(int))
                return "int";

            if (type == typeof(float))
                return "float";

            if (type == typeof(double))
                return "double";

            if (type == typeof(string))
                return "string";

            return type.Name;
        }

        public static void GenerateCode(Assembly a)
        {
            var writer = new StringBuilder();
            var types = a.GetTypes();
            foreach (var type in types)
            {
                var properties = type.GetProperties();
                writer.AppendLine("public class " + type.Name);
                writer.AppendLine("{");

                foreach (var property in properties)
                {
                    var privateVariable = "_" + property.Name.ToCamelCase();
                    writer.AppendLine("\tprivate " + property.PropertyType.FriendlyName() + " " + privateVariable + ";");
                }

                foreach (var property in properties)
                {
                    var privateVariable = "_" + property.Name.ToCamelCase();
                    writer.AppendLine("\tpublic " + property.PropertyType.FriendlyName() + " " + property.Name);
                    writer.AppendLine("\t{");
                    writer.AppendLine("\t\tget{ return " + privateVariable + "; }");
                    writer.AppendLine("\t\tset");
                    writer.AppendLine("\t\t{");
                    writer.AppendLine("\t\t\tif(" + privateVariable + " != value)");
                    writer.AppendLine("\t\t\t{");
                    writer.AppendLine("\t\t\t\t" + privateVariable + " = value;");
                    writer.AppendLine("\t\t\t\tOnPropertyChanged(\"" + property.Name + "\");");
                    writer.AppendLine("\t\t\t}");
                    writer.AppendLine("\t\t}");
                    writer.AppendLine("\t}");
                }
                writer.AppendLine("}");
            }
            Clipboard.SetText(writer.ToString());
        }
        public static T FindChild<T>(this FrameworkElement obj, string name)
        {
            DependencyObject dep = obj as DependencyObject;
            T ret = default(T);

            if (dep != null)
            {
                int childcount = VisualTreeHelper.GetChildrenCount(dep);
                for (int i = 0; i < childcount; i++)
                {
                    DependencyObject childDep = VisualTreeHelper.GetChild(dep, i);
                    FrameworkElement child = childDep as FrameworkElement;

                    if (child.GetType() == typeof(T) && child.Name == name)
                    {
                        ret = (T)Convert.ChangeType(child, typeof(T));
                        break;
                    }

                    ret = child.FindChild<T>(name);
                    if (ret != null)
                        break;
                }
            }
            return ret;
        }
    }
}
