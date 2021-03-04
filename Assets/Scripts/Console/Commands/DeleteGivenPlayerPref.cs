using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DapperDino.UDCT.Utilities.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "DeleteAllPlayerPrefs", menuName = "Utilities/DeveloperConsole/Commands/DeleteGivenPlayerPref Command")]
    public class DeleteGivenPlayerPref : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            string logText = string.Join(" ", args);

            var levelController = GameObject.FindObjectOfType<LevelController>();
            levelController.DeleteGivenPlayerPrefs(logText);

            return true;
        }
    }
}