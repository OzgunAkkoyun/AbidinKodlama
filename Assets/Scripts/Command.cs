using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

//public enum CommandType
//{
//    If, For, Wait
//}

public abstract class Command
{
    //public CommandType type;

    public abstract string ToCodeString();
}

public class MoveCommand : Command
{
    public Direction direction;

    public override string ToCodeString()
    {
        switch (direction)
        {
            case Direction.Left: return "Sola Dön();";
            case Direction.Right:return "Sağa Dön();";
            case Direction.Forward:return "İlerle();";
            case Direction.Backward:return "Geri();";

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
public class ForCommand : Command
{
    public Direction direction;
    public int loopCount;

    public override string ToCodeString()
    {
        switch (direction)
        {
            case Direction.Left: return "Sola Dön();";
            case Direction.Right: return "Sağa Dön();";
            case Direction.Forward: return "İlerle();";
            case Direction.Backward: return "Geri();";

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
public class WaitCommand : Command
{
    public int seconds;

    public override string ToCodeString()
    {
        throw new NotImplementedException();
    }
}

//public class PickIfObjectMatchesCommand : Command
//{
//    public ObjectType expectedObjectType;
//}

//public class PickIfAnyObjectExistsCommand : Command
//{
//}

//public class PickIfObjectMatchesInAdjacentCellCommand : Command
//{
//    public Direction adjacentCellDirection;
//    public ObjectType expectedObjectType;
//}

//public class PickIfAnyObjectExistsInAdjacentCellCommand : Command
//{
//    public Direction adjacentCellDirection;
//}
    