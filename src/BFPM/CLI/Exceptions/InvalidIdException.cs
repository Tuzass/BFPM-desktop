using System;

namespace BFPM.CLI.Exceptions;

public class InvalidIdException : Exception
{
    public InvalidIdException()
        : base("Invalid ID.")
    {
    }
}
