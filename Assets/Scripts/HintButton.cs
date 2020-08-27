using UnityEngine;

public class HintButton : MonoBehaviour
{
    public Commander commander;
    public MapGenerator mapGenerator;
    public UIHandler uh;
    public int wrongCommandIndex;
    void Start()
    {
        
    }

    public void Hint()
    {
        if (CheckCommandInputs())
        {
            FindNextCommand();
        }
    }

    private void FindNextCommand()
    {
        var nextCommandIndex = commander.commands.Count+1;
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
                if (!CheckDirections(commandMove.direction, i))
                {
                    wrongCommandIndex = i;
                    uh.ShowWrongCommand(wrongCommandIndex);
                    return false;
                    break;
                }
            }
            else if (type == typeof(ForCommand))
            {
                var commandMove = currentCommand as ForCommand;
                if (!CheckDirections(commandMove.direction, i))
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

    private bool CheckDirections(Direction commandMoveDirection, int i) =>
        mapGenerator.Path[i+1].pathDirection == commandMoveDirection;
}
