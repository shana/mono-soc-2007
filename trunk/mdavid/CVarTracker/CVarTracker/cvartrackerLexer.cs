// $ANTLR 3.0 CVarTracker/cvartracker.g 2007-08-01 18:39:53

using System;
using Antlr.Runtime;
using IList 		= System.Collections.IList;
using ArrayList 	= System.Collections.ArrayList;
using Stack 		= Antlr.Runtime.Collections.StackList;



public class cvartrackerLexer : Lexer 
{
    public const int IDENTIFIER = 5;
    public const int ALPHANUMERIC = 6;
    public const int WHITESPACE = 7;
    public const int Tokens = 8;
    public const int EOF = -1;
    public const int SEMI = 4;

    public cvartrackerLexer() 
    {
		InitializeCyclicDFAs();
    }
    public cvartrackerLexer(ICharStream input) 
		: base(input)
	{
		InitializeCyclicDFAs();
    }
    
    override public string GrammarFileName
    {
    	get { return "CVarTracker/cvartracker.g";} 
    }

    // $ANTLR start SEMI 
    public void mSEMI() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = SEMI;
            // CVarTracker/cvartracker.g:7:8: ( ';' )
            // CVarTracker/cvartracker.g:7:8: ';'
            {
            	Match(';'); 
            
            }
    
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end SEMI

    // $ANTLR start IDENTIFIER 
    public void mIDENTIFIER() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = IDENTIFIER;
            // CVarTracker/cvartracker.g:52:14: ( ( ALPHANUMERIC )+ )
            // CVarTracker/cvartracker.g:52:14: ( ALPHANUMERIC )+
            {
            	// CVarTracker/cvartracker.g:52:14: ( ALPHANUMERIC )+
            	int cnt1 = 0;
            	do 
            	{
            	    int alt1 = 2;
            	    int LA1_0 = input.LA(1);
            	    
            	    if ( ((LA1_0 >= '0' && LA1_0 <= '9') || (LA1_0 >= 'A' && LA1_0 <= 'Z') || LA1_0 == '_' || (LA1_0 >= 'a' && LA1_0 <= 'z')) )
            	    {
            	        alt1 = 1;
            	    }
            	    
            	
            	    switch (alt1) 
            		{
            			case 1 :
            			    // CVarTracker/cvartracker.g:52:15: ALPHANUMERIC
            			    {
            			    	mALPHANUMERIC(); 
            			    
            			    }
            			    break;
            	
            			default:
            			    if ( cnt1 >= 1 ) goto loop1;
            		            EarlyExitException eee =
            		                new EarlyExitException(1, input);
            		            throw eee;
            	    }
            	    cnt1++;
            	} while (true);
            	
            	loop1:
            		;	// Stops C# compiler whinging that label 'loop1' has no statements

            
            }
    
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end IDENTIFIER

    // $ANTLR start WHITESPACE 
    public void mWHITESPACE() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = WHITESPACE;
            // CVarTracker/cvartracker.g:54:14: ( ( '\\t' | ' ' | '\\r' | '\\n' | '\\u000C' )+ )
            // CVarTracker/cvartracker.g:54:14: ( '\\t' | ' ' | '\\r' | '\\n' | '\\u000C' )+
            {
            	// CVarTracker/cvartracker.g:54:14: ( '\\t' | ' ' | '\\r' | '\\n' | '\\u000C' )+
            	int cnt2 = 0;
            	do 
            	{
            	    int alt2 = 2;
            	    int LA2_0 = input.LA(1);
            	    
            	    if ( ((LA2_0 >= '\t' && LA2_0 <= '\n') || (LA2_0 >= '\f' && LA2_0 <= '\r') || LA2_0 == ' ') )
            	    {
            	        alt2 = 1;
            	    }
            	    
            	
            	    switch (alt2) 
            		{
            			case 1 :
            			    // CVarTracker/cvartracker.g:
            			    {
            			    	if ( (input.LA(1) >= '\t' && input.LA(1) <= '\n') || (input.LA(1) >= '\f' && input.LA(1) <= '\r') || input.LA(1) == ' ' ) 
            			    	{
            			    	    input.Consume();
            			    	
            			    	}
            			    	else 
            			    	{
            			    	    MismatchedSetException mse =
            			    	        new MismatchedSetException(null,input);
            			    	    Recover(mse);    throw mse;
            			    	}

            			    
            			    }
            			    break;
            	
            			default:
            			    if ( cnt2 >= 1 ) goto loop2;
            		            EarlyExitException eee =
            		                new EarlyExitException(2, input);
            		            throw eee;
            	    }
            	    cnt2++;
            	} while (true);
            	
            	loop2:
            		;	// Stops C# compiler whinging that label 'loop2' has no statements

            	 channel = HIDDEN; 
            
            }
    
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end WHITESPACE

    // $ANTLR start ALPHANUMERIC 
    public void mALPHANUMERIC() // throws RecognitionException [2]
    {
        try 
    	{
            // CVarTracker/cvartracker.g:56:25: ( ( 'a' .. 'z' | 'A' .. 'Z' | '_' | '0' .. '9' ) )
            // CVarTracker/cvartracker.g:56:25: ( 'a' .. 'z' | 'A' .. 'Z' | '_' | '0' .. '9' )
            {
            	if ( (input.LA(1) >= '0' && input.LA(1) <= '9') || (input.LA(1) >= 'A' && input.LA(1) <= 'Z') || input.LA(1) == '_' || (input.LA(1) >= 'a' && input.LA(1) <= 'z') ) 
            	{
            	    input.Consume();
            	
            	}
            	else 
            	{
            	    MismatchedSetException mse =
            	        new MismatchedSetException(null,input);
            	    Recover(mse);    throw mse;
            	}

            
            }

        }
        finally 
    	{
        }
    }
    // $ANTLR end ALPHANUMERIC

    override public void mTokens() // throws RecognitionException 
    {
        // CVarTracker/cvartracker.g:1:10: ( SEMI | IDENTIFIER | WHITESPACE )
        int alt3 = 3;
        switch ( input.LA(1) ) 
        {
        case ';':
        	{
            alt3 = 1;
            }
            break;
        case '0':
        case '1':
        case '2':
        case '3':
        case '4':
        case '5':
        case '6':
        case '7':
        case '8':
        case '9':
        case 'A':
        case 'B':
        case 'C':
        case 'D':
        case 'E':
        case 'F':
        case 'G':
        case 'H':
        case 'I':
        case 'J':
        case 'K':
        case 'L':
        case 'M':
        case 'N':
        case 'O':
        case 'P':
        case 'Q':
        case 'R':
        case 'S':
        case 'T':
        case 'U':
        case 'V':
        case 'W':
        case 'X':
        case 'Y':
        case 'Z':
        case '_':
        case 'a':
        case 'b':
        case 'c':
        case 'd':
        case 'e':
        case 'f':
        case 'g':
        case 'h':
        case 'i':
        case 'j':
        case 'k':
        case 'l':
        case 'm':
        case 'n':
        case 'o':
        case 'p':
        case 'q':
        case 'r':
        case 's':
        case 't':
        case 'u':
        case 'v':
        case 'w':
        case 'x':
        case 'y':
        case 'z':
        	{
            alt3 = 2;
            }
            break;
        case '\t':
        case '\n':
        case '\f':
        case '\r':
        case ' ':
        	{
            alt3 = 3;
            }
            break;
        	default:
        	    NoViableAltException nvae_d3s0 =
        	        new NoViableAltException("1:1: Tokens : ( SEMI | IDENTIFIER | WHITESPACE );", 3, 0, input);
        
        	    throw nvae_d3s0;
        }
        
        switch (alt3) 
        {
            case 1 :
                // CVarTracker/cvartracker.g:1:10: SEMI
                {
                	mSEMI(); 
                
                }
                break;
            case 2 :
                // CVarTracker/cvartracker.g:1:15: IDENTIFIER
                {
                	mIDENTIFIER(); 
                
                }
                break;
            case 3 :
                // CVarTracker/cvartracker.g:1:26: WHITESPACE
                {
                	mWHITESPACE(); 
                
                }
                break;
        
        }
    
    }


	private void InitializeCyclicDFAs()
	{
	}

 
    
}
