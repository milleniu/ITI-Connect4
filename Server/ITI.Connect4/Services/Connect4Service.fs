namespace ITI.Connect4.Services

open System
open ITI.Connect4.Models

type Connect4ServiceDependency = {
    NewGame : Guid -> Result<Guid, string>
}

module Connect4Service =
    let newGame (id: Guid) : Result<Guid, string> =
        Ok(id)
