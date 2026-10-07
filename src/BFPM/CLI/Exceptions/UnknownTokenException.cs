using System;

namespace BFPM.CLI.Exceptions;

public class UnknownTokenException : Exception
{
    public UnknownTokenException()
        : base("Unknown token.")
    {
    }
}
