// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses.PolymorphicSerialization;

public class GameScene
{
    [YAXType(typeof(Sword))] public IWeapon? DefaultWeapon { get; set; }

    [YAXType(typeof(Sword))] public IWeapon? AlternativeWeapon { get; set; }

    [YAXCollectionItemType(typeof(Sword))] public IWeapon[]? Weapons { get; set; }

    [YAXCollectionItemType(typeof(Alien))] public List<CharacterBase> Characters { get; set; } = new();

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static GameScene GetSampleInstance()
    {
        return new GameScene {
            Weapons = new IWeapon[] { new Sword(), new Gun() },
            Characters = new List<CharacterBase> { new Human(), new Alien() },
            DefaultWeapon = new Sword(),
            AlternativeWeapon = new Sword()
        };
    }
}

public interface IWeapon
{
    string Name { get; }

    int FatalityFactor { get; }
}

public class Sword : IWeapon
{
    public Sword()
    {
        Name = "XCalibour";
        FatalityFactor = 1;
    }

    public string Name { get; }
    public int FatalityFactor { get; }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }
}

public class Gun : IWeapon
{
    public Gun()
    {
        Name = "Shotgun";
        FatalityFactor = 2;
        Calibre = 9.0;
    }

    public double Calibre { get; set; }

    public string Name { get; }
    public int FatalityFactor { get; }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }
}

public abstract class CharacterBase
{
    public string? Name { get; set; }
    public string? Color { get; set; }
}

public class Human : CharacterBase
{
    public Human()
    {
        Name = "Fred";
        Color = "Green";
        Stamina = 10.0;
    }

    public double Stamina { get; set; }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }
}

public class Alien : CharacterBase
{
    public Alien()
    {
        Name = "XF@#SD";
        Color = "Red";
        NumberOfHands = 4;
    }

    public int NumberOfHands { get; set; }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }
}