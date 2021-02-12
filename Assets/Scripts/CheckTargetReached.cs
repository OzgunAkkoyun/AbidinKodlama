using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

public class CheckTargetReached : MonoBehaviour
{
    public void CheckIfReachedTarget(bool isLastCommand, CharacterMovement characterMovement)
    {
        var currentCoord = new Coord((int) (characterMovement.inputVector.x / characterMovement.mapGenerate.tileSize),
            (int) (characterMovement.inputVector.z / characterMovement.mapGenerate.tileSize));

        if (characterMovement.pathGenarator.Path.Contains(currentCoord))
        {
            var targetVec3 = characterMovement.mapGenerate.CoordToPosition(characterMovement.mapGenerate.currentMap.targetPoint.x,
                characterMovement.mapGenerate.currentMap.targetPoint.y);
            
            if (targetVec3 == characterMovement.inputVector.Vector3toXZ() && isLastCommand)
            {
                if (characterMovement.gm.currentSenario.senarioIndex == 1 ||
                    characterMovement.gm.currentSenario.senarioIndex == 2)
                {
                    characterMovement.isPlayerReachedTarget = true;
                    characterMovement.CharacterAnimationPlay();
                }
                else if (characterMovement.gm.currentSenario.senarioIndex == 3)
                {
                    CheckIfObjectCount(characterMovement);
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
    public void CheckIfObjectCount(CharacterMovement characterMovement)
    {
        if (ScreenShotHandler.instance.collectedAnimalPhoto == characterMovement.gm.currentSubLevel.ifObjectCount)
        {
            characterMovement.isPlayerReachedTarget = true;
            characterMovement.CharacterAnimationPlay();
        }
        else
        {
            if (ShowWrongCleaningTile.instance.wrongIfTiles.Count > 0)
            {
                ShowWrongCleaningTile.instance.ShowWrongIfTiles();
                characterMovement.isPlayerReachedTarget = false;
            }
            else
            {
                characterMovement.isPlayerReachedTarget = false;
                characterMovement.gm.EndGame();
            }
           
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
           if (ShowWrongCleaningTile.instance.wrongWaitTiles.Count > 0)
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