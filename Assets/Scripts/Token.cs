using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

[Serializable]
public class Token
{
    [JsonProperty("token")]
    public string token;
}
