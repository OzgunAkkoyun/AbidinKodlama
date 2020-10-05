using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DapperDino.UDCT.Utilities.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "New Log Command", menuName = "Utilities/DeveloperConsole/Commands/Open Levels Command")]
    public class OpenLevels : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            string logText = string.Join(" ", args);

            //Debug.Log(logText);
            var levelController = GameObject.FindObjectOfType<LevelController>();
            levelController.OpenSenario(1);

            return true;
        }
    }
}
