using System;

namespace BFPM.CLI.Exceptions;

public class UnexpectedTokenException : Exception
{
    public UnexpectedTokenException()
        : base("Unexpected token.")
    {
    }
}
