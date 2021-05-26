using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner
{
    // All language tokens
    enum TokenType
    {
        NULL, IDENTIFIER, NUMBER,

        // Reserved Words
        RESWORD_MAIN, RESWORD_READ, RESWORD_WRITE, RESWORD_REPEAT, RESWORD_UNTILL, RESWORD_IF, RESWORD_ELSEIF, RESWORD_ELSE, RESWORD_THEN, RESWORD_RETURN, RESWORD_ENDL,

        // Datatypes
        DATATYPE_INT, DATATYPE_FLOAT, DATATYPE_STRING,

        // Arith. operators
        PLUS, MINUS, NEGATIVE, MULTIPLICATION, DIVISION,

        // Assg opearator
        ASSIGNMENT_OPERATOR,

        // Condition operators
        LESS, GREATER, ISEQUAL, NOTEQUAL,

        // Separators
        OPEN_PARENTHESES, CLOSE_PARENTHESES, OPEN_BRACES, CLOSE_BRACES, COMMA, SIMICOLOCN,

        // Boolean operators
        AND, OR,

        // Errors
        UN_RECOGNIZED_CHAR, TYPE_LIMIT_EXCEEDED, FLOATING_POINT_ERROR, RUNAWAY_STRING, RUNAWAY_COMMENT,

        // End of file
        END_OF_FILE
    };


    // Token classes
    // Abstract representing token records
    abstract class Token
    {
        protected string lexeme; // Actual lexeme string extracted from source code by the "Scanner"
        protected TokenType tokentype; // The tokenType to be passed to the "Parser"

        public KeyValuePair<string, string> ShowToken()
        {
            return new KeyValuePair<string, string>(lexeme, tokentype.ToString());
        }
    }

    // Non evaluated value tokens with no real evaluated value like identifiers, reserved words & symbols
    class NonValueToken : Token
    {
        public NonValueToken(string lx, TokenType t)
        {
            lexeme = lx;
            tokentype = t;
        }
    }

    // String value tokens
    class StringToken : Token
    {
        string stringval; // Real string token value after removing the double quotes

        public StringToken(string lx, TokenType t, string s)
        {
            lexeme = lx;
            tokentype = t;
            stringval = s;
        }
    }

    // Numeric value tokens
    class NumToken : Token
    {
        private int inumvalue; // Real integer token numeric value evaluated by the "Evaluator"
        private float fnumvalue; // Real float token numeric value evaluated by the "Evaluator"

        public NumToken(string lx, TokenType t, int n) // For integers
        {
            lexeme = lx;
            tokentype = t;
            inumvalue = n;
        }

        public NumToken(string lx, TokenType t, float f) // For floats
        {
            lexeme = lx;
            tokentype = t;
            fnumvalue = f;
        }
    }

    // Scanning errors
    class Error : Token
    {
        public Error(string e, TokenType et)
        {
            lexeme = e; // Error string
            tokentype = et; // Error type
        }
    }



    // Scanning logic class
    class ScanLogic
    {
        // Tokens list
        private static List<Token> TokensList = new List<Token>();

        // Show tokens method for GUI purpose
        public static List<KeyValuePair<string, string>> ShowTokens()
        {
            List<KeyValuePair<string, string>> L = new List<KeyValuePair<string, string>>();

            foreach (Token t in TokensList)
            {
                if (!(t is Error)) L.Add(t.ShowToken());
            }
            return L;
        }

        // Show Errors method for GUI purpose
        public static List<KeyValuePair<string, string>> ShowErrors()
        {
            List<KeyValuePair<string, string>> E = new List<KeyValuePair<string, string>>();

            foreach (Token er in TokensList)
            {
                if (er is Error) E.Add(er.ShowToken());
            }
            return E;
        }



        // Methods for characters recognition

        //
        //
        //

        // Check for whiteSpace
        private static bool isSpace(char s) { return (s == ' ' || s == '\t' || s == '\n' || s == '\r'); }

        // Check for letter
        private static bool isLetter(char l) { return (l >= 'a' && l <= 'z' || l >= 'A' && l <= 'Z'); }

        // Check for digit
        private static bool isDigit(char d) { return (d >= '0' && d <= '9'); }

        // Check for symbol
        private static bool isSymbol(char c)
        {
            return (c == '+' || c == '-' || c == '*' || c == '/' || c == '=' || c == '<' || c == '>' || c == '(' || c == ')' || c == '{' || c == '}' || c == ',' || c == ';');
        }

        // Assign symbol lexemes to a corresponding token type
        private static TokenType whichSymbol(string s)
        {
            switch (s)
            {
                case ("+"): return TokenType.PLUS;
                case ("-"): return TokenType.MINUS;
                case ("*"): return TokenType.MULTIPLICATION;
                case ("/"): return TokenType.DIVISION;
                case ("="): return TokenType.ISEQUAL;
                case (":="): return TokenType.ASSIGNMENT_OPERATOR;
                case ("<"): return TokenType.LESS;
                case (">"): return TokenType.GREATER;
                case ("<>"): return TokenType.NOTEQUAL;
                case ("&&"): return TokenType.AND;
                case ("||"): return TokenType.OR;
                case ("("): return TokenType.OPEN_PARENTHESES;
                case (")"): return TokenType.CLOSE_PARENTHESES;
                case ("{"): return TokenType.OPEN_BRACES;
                case ("}"): return TokenType.CLOSE_BRACES;
                case (","): return TokenType.COMMA;
                case (";"): return TokenType.SIMICOLOCN;
                default: return TokenType.NULL;
            }
        }

        // Assign identifier lexemes to a corresponding token type
        private static TokenType IdAssign(string k)
        {
            switch (k)
            {
                case ("main"): return TokenType.RESWORD_MAIN;
                case ("read"): return TokenType.RESWORD_READ;
                case ("write"): return TokenType.RESWORD_WRITE;
                case ("repeat"): return TokenType.RESWORD_REPEAT;
                case ("untill"): return TokenType.RESWORD_UNTILL;
                case ("if"): return TokenType.RESWORD_IF;
                case ("elseif"): return TokenType.RESWORD_ELSEIF;
                case ("else"): return TokenType.RESWORD_ELSE;
                case ("then"): return TokenType.RESWORD_THEN;
                case ("return"): return TokenType.RESWORD_RETURN;
                case ("endl"): return TokenType.RESWORD_ENDL;
                case ("int"): return TokenType.DATATYPE_INT;
                case ("float"): return TokenType.DATATYPE_FLOAT;
                case ("string"): return TokenType.DATATYPE_STRING;

                default: return TokenType.IDENTIFIER; // Return IDENTIFIER if the lexeme doesn't match a reserved word
            }
        }

        //
        //
        //


        // Automata states
        private enum States { START, NUM, IDINTIFIER, STRING, COMMENT, ASSIGNMENT, BOOL, SYMBOL, DONE, ERROR };


        // Since the language isn't that big, The Evaluator for each token is combined with the scanner
        // Scanner method
        public static void ScanCode(string c)
        {
            States state = States.START; // Current scanner state initially set to start at the beginning of the each scan
            string lexeme; // Current scanning lexeme
            string errorlex = ""; // For lexical errors
            TokenType errortype = TokenType.NULL; // For lexical errors
            int i = 0; // Source code string index

            TokensList.Clear(); // Reset the list of tokens before each scan

            try // For safety
            {
                while (c.Length != 0 && state != States.DONE)
                {
                    lexeme = ""; // Reset the scanning lexeme each iteration
                    switch (state)
                    {
                        case (States.START):
                            {
                                if (isSpace(c[i]))
                                {
                                    i++;
                                    if (i == c.Length) state = States.DONE;
                                    else state = States.START;
                                }

                                else if (isLetter(c[i]))
                                {
                                    state = States.IDINTIFIER;
                                }

                                else if (isDigit(c[i]))
                                {
                                    state = States.NUM;
                                }

                                else if (i + 1 < c.Length && c[i] == ':' && c[i + 1] == '=')
                                {
                                    state = States.ASSIGNMENT;
                                }

                                else if (i + 1 < c.Length && ((c[i] == '&' && c[i + 1] == '&') || (c[i] == '|' && c[i + 1] == '|')))
                                {
                                    state = States.BOOL;
                                }

                                else if (c[i] == '"')
                                {
                                    state = States.STRING;
                                }

                                else if (i + 1 < c.Length && c[i] == '/' && c[i + 1] == '*')
                                {
                                    state = States.COMMENT;
                                }

                                else if (isSymbol(c[i]))
                                {
                                    state = States.SYMBOL;
                                }

                                else // Any un recognized char that is out of language character set
                                {
                                    state = States.ERROR;
                                    errortype = TokenType.UN_RECOGNIZED_CHAR;
                                }
                                break;
                            }

                        case (States.IDINTIFIER):
                            {
                                while (i < c.Length && (isLetter(c[i]) || isDigit(c[i])))
                                {
                                    lexeme += c[i];
                                    i++;
                                }

                                TokensList.Add(new NonValueToken(lexeme, IdAssign(lexeme)));

                                if (i == c.Length) state = States.DONE;
                                else state = States.START;
                                break;
                            }

                        case (States.NUM):
                            {
                                while (i < c.Length && isDigit(c[i]))
                                {
                                    lexeme += c[i];
                                    i++;
                                }

                                if (i < c.Length && c[i] == '.') // Floating point
                                {
                                    lexeme += c[i]; // Add the floading point to the lexeme
                                    i++; // First digit after the point
                                    if (i < c.Length && isDigit(c[i])) // Float number
                                    {
                                        while (i < c.Length && isDigit(c[i]))
                                        {
                                            lexeme += c[i];
                                            i++;
                                        }

                                        // Float number
                                        try
                                        {
                                            // Real evaluated numeric value included
                                            TokensList.Add(new NumToken(lexeme, TokenType.NUMBER, float.Parse(lexeme)));

                                            if (i == c.Length) state = States.DONE;
                                            else state = States.START;
                                        }
                                        catch (OverflowException)
                                        {
                                            errorlex = lexeme;
                                            state = States.ERROR;
                                            errortype = TokenType.TYPE_LIMIT_EXCEEDED;
                                        }
                                    }

                                    else // Floating point error ...caused by the char after the floating point ... Either EOF or not a digit
                                    {
                                        errorlex = lexeme;
                                        state = States.ERROR;
                                        errortype = TokenType.FLOATING_POINT_ERROR;
                                    }
                                }

                                else // No floating point
                                {
                                    // Integer number
                                    try
                                    {
                                        // Real evaluated numeric value included
                                        TokensList.Add(new NumToken(lexeme, TokenType.NUMBER, int.Parse(lexeme)));

                                        if (i == c.Length) state = States.DONE;
                                        else state = States.START;
                                    }
                                    catch (OverflowException)
                                    {
                                        errorlex = lexeme;
                                        state = States.ERROR;
                                        errortype = TokenType.TYPE_LIMIT_EXCEEDED;
                                    }
                                }

                                break;
                            }

                        case (States.ASSIGNMENT):
                            {
                                lexeme += c[i];
                                lexeme += c[i + 1];
                                TokensList.Add(new NonValueToken(lexeme, whichSymbol(lexeme)));

                                i += 2;

                                if (i == c.Length) state = States.DONE;
                                else state = States.START;
                                break;
                            }

                        case (States.BOOL):
                            {
                                lexeme += c[i];
                                lexeme += c[i + 1];
                                TokensList.Add(new NonValueToken(lexeme, whichSymbol(lexeme)));

                                i += 2;

                                if (i == c.Length) state = States.DONE;
                                else state = States.START;
                                break;
                            }

                        case (States.STRING):
                            {
                                lexeme += c[i]; // Starting double quotes
                                i++; // i stands on the first char of the string
                                while (i < c.Length && c[i] != '"' && c[i] != '\n' && c[i] != '\r')
                                {
                                    lexeme += c[i];
                                    i++;
                                }
                                if (i < c.Length && c[i] == '"') // String ended successfully
                                {
                                    lexeme += c[i]; // Ending double quotes

                                    // Including string value after removing the double quotes
                                    TokensList.Add(new StringToken(lexeme, TokenType.DATATYPE_STRING, lexeme.Substring(1, lexeme.Length - 1)));

                                    i++;
                                    if (i == c.Length) state = States.DONE;
                                    else state = States.START;
                                }
                                else // No closing double quotes found ... Either the line ended or EOF (RUNAWAY_STRING ERROR)
                                {
                                    errorlex = lexeme;
                                    state = States.ERROR;
                                    errortype = TokenType.RUNAWAY_STRING;
                                }

                                break;
                            }

                        case (States.COMMENT):
                            {
                                errorlex += c[i];
                                errorlex += c[i + 1];
                                i += 2;
                                while (i + 1 < c.Length && !(c[i] == '*' && c[i + 1] == '/'))
                                {
                                    errorlex += c[i]; // In case comment failed 
                                    i++; // Skip te comment text
                                }
                                // Comment ended or reached EOF without ending
                                // So we need to check whick case

                                if (i + 1 == c.Length) // Reached EOF without ending (RUNAWAY_COMMENT ERROR)
                                {
                                    errorlex += c[i];
                                    i++;
                                    state = States.ERROR;
                                    errortype = TokenType.RUNAWAY_COMMENT;
                                }

                                else if (i == c.Length)
                                {
                                    state = States.ERROR;
                                    errortype = TokenType.RUNAWAY_COMMENT;
                                }

                                else // Comment ended successfully with the " */ "
                                {
                                    errorlex = ""; // Clear the error lexeme since noo error occured
                                    i += 2; // Skip the ending " */ "
                                    if (i == c.Length) state = States.DONE;
                                    else state = States.START;
                                }

                                // if comment didn't close untill EOF ... the rest of the text is considered as a comment

                                break;
                            }

                        case (States.SYMBOL):
                            {
                                if (c[i] == '<') // Check for the (is equal) sign
                                {
                                    if (i + 1 < c.Length && c[i + 1] == '>')
                                    {
                                        lexeme += c[i];
                                        lexeme += c[i + 1];
                                        i += 2;
                                    }
                                    else
                                    {
                                        lexeme += c[i];
                                        i++;
                                    }
                                }
                                else
                                {
                                    lexeme += c[i];
                                    i++;
                                }
                                TokensList.Add(new NonValueToken(lexeme, whichSymbol(lexeme)));
                                if (i >= c.Length) state = States.DONE;
                                else state = States.START;
                                break;
                            }

                        case (States.ERROR):
                            {
                                switch (errortype)
                                {
                                    case (TokenType.UN_RECOGNIZED_CHAR):
                                        {
                                            errorlex += c[i];
                                            TokensList.Add(new Error(errorlex, TokenType.UN_RECOGNIZED_CHAR));
                                            i++; // Skip the un recognized char
                                            break;
                                        }

                                    case (TokenType.TYPE_LIMIT_EXCEEDED):
                                        {
                                            TokensList.Add(new Error(errorlex, TokenType.TYPE_LIMIT_EXCEEDED));
                                            break;
                                        }

                                    case (TokenType.FLOATING_POINT_ERROR):
                                        {
                                            TokensList.Add(new Error(errorlex, TokenType.FLOATING_POINT_ERROR));
                                            break;
                                        }

                                    case (TokenType.RUNAWAY_STRING):
                                        {
                                            TokensList.Add(new Error(errorlex, TokenType.RUNAWAY_STRING));
                                            break;
                                        }

                                    case (TokenType.RUNAWAY_COMMENT):
                                        {
                                            TokensList.Add(new Error(errorlex, TokenType.RUNAWAY_COMMENT));
                                            break;
                                        }
                                }
                                errorlex = ""; // Reset the error lexeme
                                if (i == c.Length) state = States.DONE;
                                else state = States.START;
                                break;
                            }

                        default:
                            {
                                break;
                            }
                    }
                }
            }
            catch (Exception)
            {
                // Just for safety
                // Catch any exception that could cause failure
                // Prevent the exe from crashing
            }
        }
    }
}