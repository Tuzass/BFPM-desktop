using System;

namespace BFPM.CLI.Exceptions;

public class NonlocalProjectException : Exception
{
    public NonlocalProjectException()
        : base("Changes through the CLI can only be made to local projects.")
    {
    }
}
