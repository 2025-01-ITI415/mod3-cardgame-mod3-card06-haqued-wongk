
using System;

[Serializable]
public class DeckData
{
    public Decorator[] decorators;
    public DeckCard[] cards;
}

[Serializable]
public class Decorator
{
    public string type;
    public Location loc;
    public bool flip;
    public float scale;
}

[Serializable]
public class DeckCard
{
    public int rank;
    public string face;
    public Pip[] pips;
}

[Serializable]
public class Pip
{
    public string type;
    public Location loc;
    public bool flip;
    public float scale;
}

[Serializable]
public class Location
{
    public float x;
    public float y;
    public float z;
}
