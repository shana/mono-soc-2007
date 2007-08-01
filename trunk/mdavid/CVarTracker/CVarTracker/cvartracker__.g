lexer grammar cvartracker;
options {
  language=CSharp;

}

SEMI : ';' ;

// $ANTLR src "CVarTracker/cvartracker.g" 52
IDENTIFIER : (ALPHANUMERIC)+ ;

// $ANTLR src "CVarTracker/cvartracker.g" 54
WHITESPACE : ( '\t' | ' ' | '\r' | '\n'| '\u000C' )+ 	{ $channel = HIDDEN; } ;

// $ANTLR src "CVarTracker/cvartracker.g" 56
fragment ALPHANUMERIC : ('a'..'z'|'A'..'Z'|'_'|'0'..'9') ;
