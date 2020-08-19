using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Commander : MonoBehaviour
{
    public List<Command> commands = new List<Command>();

    public GameManager gm;

    public UnityEvent OnNewCommand = new UnityEvent();

    public void AddMoveCommand(Direction direction)
    {
        commands.Add(new MoveCommand { direction = direction });
        OnNewCommand.Invoke();
    }

    public void AddWaitCommand()
    {
        commands.Add(new WaitCommand());
        OnNewCommand.Invoke();
    }
    public void AddForCommand(Direction direction,int loopCount)
    {
        commands.Add(new ForCommand {direction = direction,loopCount = loopCount});
        OnNewCommand.Invoke();
    }

    public void ApplyCommands()
    {
        StartCoroutine(DoApplyCommands());
    }

    private IEnumerator DoApplyCommands()
    {
        yield return new WaitForSeconds(1.5f);

        for (var i = 0; i < commands.Count; i++)
        {
            var command = commands[i];
            var type = command.GetType();
            var isLastCommand = i == commands.Count - 1;

            if (gm.character.isGameFinished)
            {
                break;
            }

            if (type == typeof(MoveCommand))
            {
                var commandMove = command as MoveCommand;
                yield return StartCoroutine(gm.character.ApplyMoveCommand(commandMove.direction, isLastCommand));
            }
            else if (type == typeof(ForCommand))
            {
                var commandFor = command as ForCommand;
                for (int j = 0; j < commandFor.loopCount; j++)
                {
                    yield return StartCoroutine(gm.character.ApplyMoveCommand(commandFor.direction, isLastCommand));
                }
               
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
