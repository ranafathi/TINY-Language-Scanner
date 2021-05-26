# TINY-Language-Scanner
A program for scanning TINY language syntax into a list of tokens. 
Part 1/2 for CSE439 Design of Compilers course project.

// Team project contributions: Sherif Essam and Andre Osama

The lexical analyzer (scanner) is designed to input code in TINY language either through typing 
in the text area through the GUI or through uploading a text file containing the desired code.

Input code is scanned and white spaces are removed, tokens are identified and a symbol table is created.
Tokens are inserted into symbol table and if any errors are generated it's shown in the error table.

Types of tokens handled:
- Identifiers
- Letters
- Digits
- Operators
- Punctuation

Program GUI was created through Windows Forms on Visual Studio.
