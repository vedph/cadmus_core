using System.IO;
using System.Reflection;
using System.Text;

namespace Cadmus.Index.Sql;

static internal class ResourceHelper
{
    static public string LoadResource(string name)
    {
        using StreamReader reader = new(
            Assembly.GetExecutingAssembly().GetManifestResourceStream(
                $"Cadmus.Index.Sql.Assets.{name}")!, Encoding.UTF8);
        return reader.ReadToEnd();
    }
}
