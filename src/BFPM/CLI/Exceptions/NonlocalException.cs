using System;

namespace BFPM.CLI.Exceptions;

public class NonlocalChangeException : Exception
{
    public NonlocalChangeException()
        : base("Changes through the CLI can only be made to local projects.")
    {
    }
}
