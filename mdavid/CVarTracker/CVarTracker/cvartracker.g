/*
 This is just a simple test I'm using to learn how to use ANTLR.
*/

/**
 Auto Generated Code by ANTLR
 See ANTLR-LICENSE for license information.
*/

grammar cvartracker;

options {
    language=CSharp;
}

tokens {
	SEMI	=	';' ;
}

@header {
using System.Collections.Generic;
}

@members {
List<string> names = new List<string> ();

public List<string> Names {
	get { return names; }
}
}

/*------------------------------------------------------------------
 * PARSER RULES
 *------------------------------------------------------------------*/

greet
	: (expr)+ EOF
	;

expr
	: IDENTIFIER SEMI
	{
		names.Add ($expr.text);
		System.Console.WriteLine ("Hello {0}", $expr.text);
	}
	;

/*------------------------------------------------------------------
 * LEXER RULES
 *------------------------------------------------------------------*/

IDENTIFIER : (ALPHANUMERIC)+ ;

WHITESPACE : ( '\t' | ' ' | '\r' | '\n'| '\u000C' )+ 	{ $channel = HIDDEN; } ;

fragment ALPHANUMERIC : ('a'..'z'|'A'..'Z'|'_'|'0'..'9') ;
