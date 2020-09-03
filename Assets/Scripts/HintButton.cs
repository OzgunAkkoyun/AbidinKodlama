using System.Collections.Generic;
using UnityEngine;

public class HintButton : MonoBehaviour
{
    public Commander commander;
    public PathGenarator pathGenarator;
    public UIHandler uh;
    public int wrongCommandIndex;

    public void Hint()
    {
        if (CheckCommandInputs())
        {
            FindNextCommand();
        }
    }

    private void FindNextCommand()
    {
        var nextCommandIndex = commander.commands.Count + 1;
        uh.ShowHintCommand(nextCommandIndex);
    }

    private bool CheckCommandInputs()
    {
        wrongCommandIndex = 0;
        //First look command inputs
        for (int i = 0; i < commander.commands.Count; i++)
        {
            var type = commander.commands[i].GetType();
            var currentCommand = commander.commands[i];
            if (type == typeof(MoveCommand))
            {
                var commandMove = currentCommand as MoveCommand;
                if (!CheckDirectionsMove(commandMove.direction, i))
                {
                    wrongCommandIndex = i;
                    uh.ShowWrongCommand(wrongCommandIndex);
                    return false;
                    break;
                }
            }
            else if (type == typeof(ForCommand))
            {
                var commandFor= currentCommand as ForCommand;
                if (!CheckDirectionsFor(commandFor.directions, i,commandFor.loopCount))
                {
                    wrongCommandIndex = i;
                    uh.ShowWrongCommand(wrongCommandIndex);
                    return false;
                    break;
                }
            }
        }

        return true;
    }

    private bool CheckDirectionsFor(List<Direction> commandForDirections, int i, int loopCount)
    {
        var falseCount = 0;
        for (int j = 0; j < loopCount; j++)
        {
            for (int k = 0; k < commandForDirections.Count; k++)
            {
                if (pathGenarator.Path[i + 1].pathDirection == commandForDirections[k])
                {

                }
                else
                {
                    falseCount++;
                    return false;
                    break;
                }
            }
        }

        return true;
    }

    private bool CheckDirectionsMove(Direction commandMoveDirection, int i) =>
        pathGenarator.Path[i+1].pathDirection == commandMoveDirection;
}
