using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

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
            case Direction.Left: return "Sola Dön(); \n";
            case Direction.Right: return "Sağa Dön(); \n";
            case Direction.Forward: return "İlerle(); \n";
            case Direction.Backward: return "Geri(); \n";

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
public class ForCommand : Command
{
    public Direction direction;
    public int loopCount;
    public string codeString;

    public override string ToCodeString()
    {
        codeString = "for(var sayi = 0; sayi < " + loopCount + "; sayi++){\n";
        if (direction == Direction.Left)
        {
            codeString += "\tSola Dön(); \n";
        }
        else if (direction == Direction.Right)
        {
            codeString += "\tSağa Dön(); \n";
        }
        else if (direction == Direction.Forward)
        {
            codeString += "\tİlerle(); \n";
        }
        else if (direction == Direction.Backward)
        {
            codeString += "\tGeri(); \n";
        }

        codeString += "}";
        return codeString;
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
    