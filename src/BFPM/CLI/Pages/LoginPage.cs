using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace BFPM.CLI.Pages;

public static class LoginPage
{

    public static KeyValuePair<string, string>? Run(string username = "", string password = "")
    {
        return new(username, password);
    }
}
