using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using System.Reflection;

namespace SQLError;

public partial class App : ComponentBase
{

    private void OnNavigateAsync(NavigationContext args)
    {
        var url = args.Path.ToLower() switch
        {
            "facebook" => "https://www.facebook.com",
            _ => ""
        };

        if (!string.IsNullOrEmpty(url))
            NavigationManager.NavigateTo(url, true);

        return;
    }

    private IEnumerable<Assembly> GetAssemblies()
    {
        var loadedAssemblies = new List<Assembly>();
        var assembliesToLoad = new Stack<Assembly>();

        assembliesToLoad.Push(Assembly.GetEntryAssembly());

        while (assembliesToLoad.Count > 0)
        {
            var assembly = assembliesToLoad.Pop();

            yield return assembly;

            foreach (var reference in assembly.GetReferencedAssemblies())
            {
                if (!loadedAssemblies.Any(a => a.FullName == reference.FullName))
                {
                    var referencedAssembly = Assembly.Load(reference);
                    assembliesToLoad.Push(referencedAssembly);
                    loadedAssemblies.Add(referencedAssembly);
                }
            }
        }
    }

    private IEnumerable<Assembly> GetAreaWebImplementations()
    {
        return GetAssemblies()
            .SelectMany(a => a.GetExportedTypes())
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IAreaWeb).IsAssignableFrom(t))
            .Select(t => t.Assembly);
    }
}
