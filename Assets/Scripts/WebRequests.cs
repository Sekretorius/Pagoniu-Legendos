using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WebRequests
{
    public const string Prefix = "https://pagoniu-legend.azurewebsites.net/public/api/";
    public const string RequestHeader = "https://pagoniu-legend.azurewebsites.net/public/api/";
    public const string Login = Prefix + "Login";
    public const string Register = Prefix + "Register";
    public const string GetUser = Prefix + "Users/";
    public const string GetUsers = Prefix + "Users";
    public const string GetBase = Prefix + "Bases/";
    public const string GetBases = Prefix + "Bases";
    public const string AddBase = Prefix + "Bases";
    public const string Bases = Prefix + "Bases";
}
