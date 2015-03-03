module FsPrimo.Tests

open FsPrimo
open NUnit.Framework

type Name = Provided.PrimoString<MinLength = 2, Regex = @"\w+">

[<Test>]
let ``valid values`` () =
  let name = Name.Create "Tomek"
  match name with
  | Some n -> ()
  | None -> failwith "none"