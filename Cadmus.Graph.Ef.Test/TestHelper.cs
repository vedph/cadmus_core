using System.IO;
using System.Reflection;
using System.Text;

namespace Cadmus.Graph.Ef.Test;

internal static class TestHelper
{
    public static Stream GetResourceStream(string name)
    {
        return Assembly.GetExecutingAssembly()!
            .GetManifestResourceStream($"Cadmus.Graph.Ef.Test.Assets.{name}")!;
    }

    public static string LoadResourceText(string name)
    {
        using StreamReader reader = new(GetResourceStream(name),
            Encoding.UTF8);
        return reader.ReadToEnd();
    }
}
