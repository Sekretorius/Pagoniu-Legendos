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
public class World
{
    public int id;
}


[Serializable]
public class WorldSection
{
    public int id;
    public float worldPositionX;
    public float worldPositionY;
    public int world_id;
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

    public float localPositionX = 0;

    public float localPositionY = 0;

    public Base(int id, int client_id, int world_section_id, float localPositinionX, float localPositinionY)
    {
        this.id = id;
        this.client_id = client_id;
        this.world_section_id = world_section_id;
        this.localPositionX = localPositinionX;
        this.localPositionY = localPositinionY;
    }
    public Base()
    {

    }
}

[Serializable]
public class Building
{
    public int id;

    public int base_id;

    public int localPositionX = 0;

    public int localPositionY = 0;

    public string type;

    public string name;

    public int price;

    public string build_time;

    public int isBuilt;

    public int health;

    public Building(int id,int base_id, int localPositionX, int localPositionY, string type, string name, int price, string build_time, int isBuilt, int health)
    {
        this.id = id;
        this.base_id = base_id;
        this.localPositionX = localPositionX;
        this.localPositionY = localPositionY;
        this.type = type;
        this.name = name;
        this.price = price;
        this.build_time = build_time;
        this.isBuilt = isBuilt;
        this.health = health;
    }
}

[Serializable]
public class Citizen
{
    public int id;
    public int base_id;
    public int count;
    public int health;
}

[Serializable]
public class Worker
{
    public int id;
    public int base_id;
    public int count;
    public int health;
}

[Serializable]
public class Soldier
{
    public int id;
    public int base_id;
    public int count;
    public int training_cost;
    public string type;
    public int damage;
    public int attack_speed;
    public int move_speed;
    public int health;
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

