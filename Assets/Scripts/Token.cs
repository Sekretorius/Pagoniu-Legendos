using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

[Serializable]
public class Token
{
    public User user;

    public string access_token;

    public string refresh_token;
}

[Serializable]
public class User
{

    public int id;

    public string username;

}

[Serializable]
public class Base
{

    public int id;

    public int client_id;

    public int world_section_id;

    public int localPositionX = 0;

    public int localPositionY = 0;

    public Base(int id, int client_id, int world_section_id, int localPositinionX, int localPositinionY)
    {
        this.id = id;
        this.client_id = client_id;
        this.world_section_id = world_section_id;
        this.localPositionX = localPositinionX;
        this.localPositionY = localPositinionY;
    }
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }

    public static string FixJson(string value)
    {
        value = "{\"Items\":" + value + "}";
        return value;
    }
}

