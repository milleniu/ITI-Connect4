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

type CaseState
  = Empty
  | PlayerOne
  | PlayerTwocaseString : CaseState -> String

caseString caseState = 
  case caseState of 
    Empty -> 
      "Empty"
    PlayerOne -> 
      "PlayerOne"
    PlayerTwo -> 
      "PlayerTwo"update : Msg -> Model -> Model

update msg model =
  case msg of
    NewGame -> 
      Debug.log("Click")
      (createModel "0")
    AddPawn id ->
      Debug.log(String.fromInt(id))
      (createModel "ATTENTION ADDPAWN PAS NEW GAME")createModel gameID =
  { gameId = gameID,  board =[ [ Empty, Empty, Empty, Empty, Empty, Empty ],[ Empty, Empty, Empty, Empty, Empty, Empty ],[ Empty, Empty, Empty, Empty, Empty, Empty ],[ Empty, Empty, Empty, Empty, Empty, Empty ],[ Empty, Empty, Empty, Empty, Empty, Empty ],[ Empty, Empty, Empty, Empty, Empty, Empty ],[ Empty, Empty, Empty, Empty, Empty, Empty ]] }-- VIEW   view : Model -> Html Msg
view model =
  div [ ]
    [ button
        [ class "buttons", onClick NewGame ] [text "New Game" ]
    , div 
        [ class "gridContainer"
        , style "display" "flex"
        ]
        (List.indexedMap (\i elm -> div 
                            [ class "column", onClick (AddPawn i) ]
                            (List.map(\n -> div
                                              [ 
                                                class (caseString n)
                                              ]
                                              [ 
                                                li [] []
                                              ]) elm)          ) model.board)
    ]
