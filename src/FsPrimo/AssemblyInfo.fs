namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("FsPrimo")>]
[<assembly: AssemblyProductAttribute("FsPrimo")>]
[<assembly: AssemblyDescriptionAttribute("Type Provider for wrapping primitive types to restrict possible range of valid values")>]
[<assembly: AssemblyVersionAttribute("1.0")>]
[<assembly: AssemblyFileVersionAttribute("1.0")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "1.0"
