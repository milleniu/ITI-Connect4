module Main exposing (..)

import Browser
import List
import Html exposing (..)
import List.Extra exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onClick)

-- INTERNAL MODULES

-- import Styles exposing (..)

main =
  Browser.sandbox { init = init, update = update, view = view }


-- MODEL

type alias Model = 
  {
    gameId : String,
    board : List (List CaseState)
  }

init : Model
init = 
  Model
  "0"
  [
    [ Empty, Empty, Empty, Empty, Empty, Empty ],
    [ PlayerOne, Empty, Empty, Empty, Empty, Empty ],
    [ PlayerTwo, Empty, Empty, Empty, Empty, Empty ],
    [ Empty, Empty, Empty, Empty, Empty, Empty ],
    [ Empty, Empty, Empty, Empty, Empty, Empty ],
    [ Empty, Empty, Empty, Empty, Empty, Empty ],
    [ Empty, Empty, Empty, Empty, Empty, Empty ]
  ]

-- UPDATE

type Msg 
  = AddPawn Int
  | NewGame 

type alias Case =
  { state : CaseState
  , id : Int
  }

