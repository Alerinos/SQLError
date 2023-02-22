using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace SQLError;

public partial class App : ComponentBase
{
    private IEnumerable<Assembly> Types()
        => GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p =>
                    p.IsClass &&
                    p.IsAbstract == false &&
                    typeof(IAreaWeb).IsAssignableFrom(p)
                )
            .Select(x => x.Assembly);

    public static IEnumerable<Assembly> GetAssemblies()
    {
        var list = new List<string>();
        var stack = new Stack<Assembly>();

        stack.Push(Assembly.GetEntryAssembly());

        do
        {
            var asm = stack.Pop();

            yield return asm;

            foreach (var reference in asm.GetReferencedAssemblies())
                if (!list.Contains(reference.FullName))
                {
                    stack.Push(Assembly.Load(reference));
                    list.Add(reference.FullName);
                }

        }
        while (stack.Count > 0);
    }
}
