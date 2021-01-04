using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

public class CheckTargetReached : MonoBehaviour
{
   public void CheckIfReachedTarget(bool isLastCommand, CharacterMovement characterMovement)
    {
        var currentCoord = new Coord((int)(characterMovement.inputVector.x / characterMovement.mapGenerate.tileSize), (int)(characterMovement.inputVector.z / characterMovement.mapGenerate.tileSize));

        if (characterMovement.pathGenarator.Path.Contains(currentCoord))
        {
            if (characterMovement.mapGenerate.CoordToPosition(characterMovement.mapGenerate.currentMap.targetPoint.x, characterMovement.mapGenerate.currentMap.targetPoint.y) == characterMovement.inputVector.Vector3toXZ())
            {
                if (characterMovement.gm.currentSenario.senarioIndex == 1 || characterMovement.gm.currentSenario.senarioIndex == 2)
                {
                    characterMovement.isPlayerReachedTarget = true;
                    characterMovement.CharacterAnimationPlay();
                }
                else if (characterMovement.gm.currentSenario.senarioIndex == 3)
                {
                    characterMovement.CheckIfObjectCount();
                }
                else if (characterMovement.gm.currentSenario.senarioIndex == 4)
                {
                    CheckWaitObjectsCount(characterMovement);
                }
            }
            else
            {
                if (isLastCommand)
                {
                    characterMovement.isPlayerReachedTarget = false;
                    characterMovement.gm.EndGame();
                }
            }
        }
        else
        {
            characterMovement.isPlayerReachedTarget = false;
            characterMovement.gm.EndGame();
        }
    }

   public void CheckWaitObjectsCount(CharacterMovement characterMovement)
   {
       if (characterMovement.waitObjectsAnimation.howManyDirtCleaned == characterMovement.gm.currentSubLevel.dirtCount )
       {
           characterMovement.isPlayerReachedTarget = true;
           characterMovement.CharacterAnimationPlay();
       }
       else
       {
           if (ShowWrongCleaningTile.instance.wrongCleaningTiles.Count > 0)
           {
               ShowWrongCleaningTile.instance.ShowWrongCleaningTiles();
               characterMovement.isPlayerReachedTarget = false;
           }
           else
           {
               characterMovement.isPlayerReachedTarget = false;
               characterMovement.gm.EndGame();
           }
       }
   }
}