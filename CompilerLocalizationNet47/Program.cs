using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace CompilerLocalizationNet47
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var options = new CompilerParameters();
            options.GenerateExecutable = false;
            options.GenerateInMemory = true;
            options.ReferencedAssemblies.Add("mscorlib.dll");
            options.ReferencedAssemblies.Add("System.dll");
            options.ReferencedAssemblies.Add("System.Core.dll");
            options.ReferencedAssemblies.Add("System.Data.dll");

            var sources = new string[] { "int x = ;" }; // Error de compilación: falta un valor después del signo igualm
            var compilerResults = CodeDomProvider.CreateProvider("CSharp").CompileAssemblyFromSource(options, sources);

            foreach (CompilerError error in compilerResults.Errors)
            {
                // Cargar el archivo de recursos apropiado
                //var resourceManager = new ResourceManager("System.Core.resources", Assembly.GetExecutingAssembly());
                //var resourceManager = new ResourceManager("System.Core", Assembly.GetAssembly(typeof(System.Linq.Enumerable)));
                //var resourceManager = new ResourceManager("mscorlib", Assembly.GetAssembly(typeof(object)));
                //var resourceManager = new ResourceManager("System", Assembly.GetAssembly(typeof(Uri)));
                //var resourceManager = new ResourceManager("mscorlib", typeof(object).Assembly);
                //var resourceManager = new ResourceManager("mscorlib", Assembly.GetAssembly(typeof(object)).GetSatelliteAssembly(new CultureInfo("en")));
                //var resourceManager = new ResourceManager("System", Assembly.Load("System").GetSatelliteAssembly(new CultureInfo("fr")));
                var resourceManager = new ResourceManager("System", typeof(string).Assembly);




                // Obtener el mensaje de error localizado
                var errorMessage = resourceManager.GetString(error.ErrorNumber);

                // Si el mensaje de error no se encuentra en el archivo de recursos actual, buscar en los archivos de recursos de los ensamblados referenciados
                if (errorMessage == null)
                {
                    foreach (var assembly in options.ReferencedAssemblies)
                    {
                        if (assembly == null)
                        {
                            continue;
                        }

                        var assemblyName = AssemblyName.GetAssemblyName(assembly);
                        var resourceName = assemblyName.Name + ".ErrorMessages";
                        var assemblyRef = Assembly.Load(assemblyName);
                        CultureInfo culture = new CultureInfo("es-ES");

                        // Obtener el mensaje de error localizado
                        errorMessage = new ResourceManager(resourceName, assemblyRef).GetString(error.ErrorNumber, culture);

                        if (errorMessage != null)
                        {
                            break;
                        }
                    }
                }

                // Si aún no se ha encontrado el mensaje de error localizado, usar el mensaje predeterminado
                if (errorMessage == null)
                {
                    errorMessage = error.ErrorText;
                }

                // Mostrar el mensaje de error localizado
                Console.WriteLine("{0}: {1}", error.ErrorNumber, errorMessage);
                Console.ReadLine();
            }
        }

        //static void Main(string[] args)
        //{
        //    // C# source code to compile
        //    string sourceCode = @"
        //    using System;

        //    namespace HelloWorld
        //    {
        //        public class Program
        //        {
        //            public static void Main)
        //            {
        //                Console.WriteLine(""Hello, World!"");
        //            }
        //        }
        //    }";

        //    // Create an instance of the CSharpCodeProvider
        //    using (CSharpCodeProvider provider = new CSharpCodeProvider())
        //    {
        //        // Set up the compiler parameters
        //        CompilerParameters compilerParameters = new CompilerParameters
        //        {
        //            GenerateInMemory = false, // Set to true if you want to generate the assembly in memory
        //            OutputAssembly = "HelloWorld.exe", // Output file name
        //            GenerateExecutable = true // Set to true to generate an executable, false for a DLL
        //        };

        //        // Add any necessary references
        //        compilerParameters.ReferencedAssemblies.Add("System.dll");

        //        // Compile the source code
        //        CompilerResults results = provider.CompileAssemblyFromSource(compilerParameters, sourceCode);

        //        // Check for errors
        //        if (results.Errors.HasErrors)
        //        {
        //            Console.WriteLine("Compilation failed:");
        //            foreach (CompilerError error in results.Errors)
        //            {
        //                Console.WriteLine($"{error.FileName}({error.Line},{error.Column}): error {error.ErrorNumber}: {error.ErrorText}");
        //            }
        //        }
        //        else
        //        {
        //            Console.WriteLine("Compilation succeeded.");
        //        }
        //    }
        //    Console.ReadLine();
        //}
    }
}
