// $ANTLR 3.0 CVarTracker/cvartracker.g 2007-08-01 18:39:53

using System.Collections.Generic;


using System;
using Antlr.Runtime;
using IList 		= System.Collections.IList;
using ArrayList 	= System.Collections.ArrayList;
using Stack 		= Antlr.Runtime.Collections.StackList;



/**
 Auto Generated Code by ANTLR
 See ANTLR-LICENSE for license information.
*/
public class cvartrackerParser : Parser 
{
    public static readonly string[] tokenNames = new string[] 
	{
        "<invalid>", 
		"<EOR>", 
		"<DOWN>", 
		"<UP>", 
		"SEMI", 
		"IDENTIFIER", 
		"ALPHANUMERIC", 
		"WHITESPACE"
    };

    public const int IDENTIFIER = 5;
    public const int ALPHANUMERIC = 6;
    public const int WHITESPACE = 7;
    public const int EOF = -1;
    public const int SEMI = 4;
    
    
        public cvartrackerParser(ITokenStream input) 
    		: base(input)
    	{
    		InitializeCyclicDFAs();
        }
        

    override public string[] TokenNames
	{
		get { return tokenNames; }
	}

    override public string GrammarFileName
	{
		get { return "CVarTracker/cvartracker.g"; }
	}


    List<string> names = new List<string> ();

    public List<string> Names {
    	get { return names; }
    }


    
    // $ANTLR start greet
    // CVarTracker/cvartracker.g:36:1: greet : ( expr )+ EOF ;
    public void greet() // throws RecognitionException [1]
    {   
        try 
    	{
            // CVarTracker/cvartracker.g:37:4: ( ( expr )+ EOF )
            // CVarTracker/cvartracker.g:37:4: ( expr )+ EOF
            {
            	// CVarTracker/cvartracker.g:37:4: ( expr )+
            	int cnt1 = 0;
            	do 
            	{
            	    int alt1 = 2;
            	    int LA1_0 = input.LA(1);
            	    
            	    if ( (LA1_0 == IDENTIFIER) )
            	    {
            	        alt1 = 1;
            	    }
            	    
            	
            	    switch (alt1) 
            		{
            			case 1 :
            			    // CVarTracker/cvartracker.g:37:5: expr
            			    {
            			    	PushFollow(FOLLOW_expr_in_greet61);
            			    	expr();
            			    	followingStackPointer_--;

            			    
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

            	Match(input,EOF,FOLLOW_EOF_in_greet65); 
            
            }
    
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end greet

    public class expr_return : ParserRuleReturnScope 
    {
    };
    
    // $ANTLR start expr
    // CVarTracker/cvartracker.g:40:1: expr : IDENTIFIER SEMI ;
    public expr_return expr() // throws RecognitionException [1]
    {   
        expr_return retval = new expr_return();
        retval.start = input.LT(1);
    
        try 
    	{
            // CVarTracker/cvartracker.g:41:4: ( IDENTIFIER SEMI )
            // CVarTracker/cvartracker.g:41:4: IDENTIFIER SEMI
            {
            	Match(input,IDENTIFIER,FOLLOW_IDENTIFIER_in_expr76); 
            	Match(input,SEMI,FOLLOW_SEMI_in_expr78); 

            			names.Add (input.ToString(retval.start,input.LT(-1)));
            			System.Console.WriteLine ("Hello {0}", input.ToString(retval.start,input.LT(-1)));
            		
            
            }
    
            retval.stop = input.LT(-1);
    
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end expr


	private void InitializeCyclicDFAs()
	{
	}

 

    public static readonly BitSet FOLLOW_expr_in_greet61 = new BitSet(new ulong[]{0x0000000000000020UL});
    public static readonly BitSet FOLLOW_EOF_in_greet65 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_IDENTIFIER_in_expr76 = new BitSet(new ulong[]{0x0000000000000010UL});
    public static readonly BitSet FOLLOW_SEMI_in_expr78 = new BitSet(new ulong[]{0x0000000000000002UL});

}
