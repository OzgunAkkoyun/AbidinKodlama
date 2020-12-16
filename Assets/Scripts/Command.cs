using System;
using System.Collections.Generic;

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
    public List<Direction> directions;
    public int loopCount;
    public string codeString;

    public override string ToCodeString()
    {
        codeString = "for(var sayi = 0; sayi < " + loopCount + "; sayi++){\n";
        for (int i = 0; i < directions.Count; i++)
        {
            if (directions[i] == Direction.Left)
            {
                codeString += "\tSola Dön(); \n";
            }
            else if (directions[i] == Direction.Right)
            {
                codeString += "\tSağa Dön(); \n";
            }
            else if (directions[i] == Direction.Forward)
            {
                codeString += "\tİlerle(); \n";
            }
            else if (directions[i] == Direction.Backward)
            {
                codeString += "\tGeri(); \n";
            }
        }
        
        codeString += "}\n";
        return codeString;
    }
}

public class PickIfAnyObjectExistsCommand : Command
{
    public string animalName;

    public override string ToCodeString()
    {
        return "if(" + animalName + " ise){\n Fotoğraf Çek();\n}";
    }
}
public class WaitCommand : Command
{
    public int seconds;

    public override string ToCodeString()
    {
        return "Bekle( "+seconds+" saniye);\n";
    }
}

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
