namespace FsPrimo

open ProviderImplementation.ProvidedTypes
open Microsoft.FSharp.Core.CompilerServices
open System.Reflection

[<TypeProvider>]
type PrimoProvider (config : TypeProviderConfig) as this = 
    inherit TypeProviderForNamespaces()

    let ns = "FsPrimo.Provided"
    let asm = Assembly.GetExecutingAssembly()

    let parameters = [
        ProvidedStaticParameter("MinLength", typeof<int>, System.Int32.MinValue)
        ProvidedStaticParameter("MaxLength", typeof<int>, System.Int32.MaxValue)
        ProvidedStaticParameter("Regex", typeof<string>, null)
    ]
    let stringProvider = ProvidedTypeDefinition(asm, ns, "PrimoString", Some(typeof<obj>))
    do stringProvider.DefineStaticParameters(parameters, fun typeName args ->
        let minLength = args.[0] :?> int
        let maxLength = args.[1] :?> int
        let regex = args.[2] :?> string
        let provider = ProvidedTypeDefinition(asm, ns, typeName, Some(typeof<string>))
        let ctor = ProvidedConstructor([ProvidedParameter("v", typeof<string>)])
        let f =             fun (args : Quotations.Expr list) -> <@@ (%%(args.[0]):string) @@>
        ctor.InvokeCode <- f

        provider.AddMember(ctor)
        let optionType = typedefof<Option<_>>.MakeGenericType(provider)
        let create = 
            ProvidedMethod
                ("Create", [ ProvidedParameter("value", typeof<string>) ], optionType)

        create.InvokeCode <- 
            fun args -> 
                <@@ let v = (%%(args.[0]):string) 
                    let x = (%%(f(args)) : string)
                    if v = null then None
                    elif v.Length < minLength then None
                    elif v.Length > maxLength then None
                    elif regex <> null && not (System.Text.RegularExpressions.Regex(regex).IsMatch(v)) then None
                    else Some x @@>
        create.IsStaticMethod <- true
        provider.AddMember(create)
        provider)
    
    let iparameters = [
        ProvidedStaticParameter("Min", typeof<int>, System.Int32.MinValue)
        ProvidedStaticParameter("Max", typeof<int>, System.Int32.MaxValue)
    ]
    let integerProvider = ProvidedTypeDefinition(asm, ns, "PrimoInteger", Some(typeof<obj>))
    do integerProvider.DefineStaticParameters(iparameters, fun typeName args ->
        let min = args.[0] :?> int
        let max = args.[1] :?> int
        let provider = ProvidedTypeDefinition(asm, ns, typeName, Some(typeof<int>))
        let ctor = ProvidedConstructor([ProvidedParameter("v", typeof<int>)])
        let f = fun (args : Quotations.Expr list) -> <@@ (%%(args.[0]):int) @@>
        ctor.InvokeCode <- f
        ctor.IsTypeInitializer <- true
        provider.AddMember(ctor)
        let defCtor = ProvidedConstructor([])
        defCtor.IsTypeInitializer <- true
        provider.AddMember(defCtor)

        let optionType = typedefof<Option<_>>.MakeGenericType(provider)
        let create = ProvidedMethod("Create", [ProvidedParameter("value", typeof<int>)], optionType)
        create.InvokeCode <-
            fun args ->
                <@@
                    let v = (%%(args.[0]):int)
                    let x = (%%(f(args)): int)
                    if v >= min && v <= max then Some x else None
                    @@>
        create.IsStaticMethod <- true
        provider.AddMember(create)
        provider
    )

    do
        this.AddNamespace(ns, [stringProvider; integerProvider])

[<assembly:TypeProviderAssembly>]
do ()