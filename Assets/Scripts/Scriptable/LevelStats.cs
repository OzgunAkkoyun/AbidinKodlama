using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(menuName = "Abidin/LevelStats", order = 1)]
public class LevelStats : ScriptableObject
{
    [Serializable]
    public class Senarios
    {
        public string senarioName;
        [Serializable]
        public class Levels
        {
            public string LevelName;
            [Serializable]
            public class SubLevels
            {
                public string subLevelName;
                public int pathLenght;
                public bool passed;
            }
            public SubLevels[] subLevels;
            public bool levelComplated;
        }
        public Levels[] levels;
        public bool senarioComplated;

    }
    public Senarios[] senarios;

    public Senarios.Levels.SubLevels GetSubLevel(int whichSenario,int whichLevel,string name) => senarios[whichSenario - 1].levels[whichLevel - 1].subLevels.FirstOrDefault(element => element.subLevelName == name);

    public Senarios.Levels GetLevel(int whichSenario, int whichLevel) => senarios[whichSenario - 1].levels[whichLevel - 1];

    public Senarios GetSenario(int whichSenario) => senarios[whichSenario - 1];
}
