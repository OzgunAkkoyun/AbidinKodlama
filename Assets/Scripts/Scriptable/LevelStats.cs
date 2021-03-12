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
        public int senarioIndex;
        [Serializable]
        public class Levels
        {
            public int levelIndex;
            [Serializable]
            public class SubLevels
            {
                public string subLevelName;
                public int subLevelIndex;
                public int pathLenght;
                public int maxIfObjectCount;
                public int ifObjectCount;
                public int dirtCount;
                public bool passed;
               
            }
            public SubLevels[] subLevels;
            public int mapSize;
            public bool levelComplated;
        }
        public Levels[] levels;
        public bool senarioComplated;

    }
    public Senarios[] senarios;

    public Senarios.Levels.SubLevels GetSubLevel(int whichSenario,int whichLevel,string name) => senarios[whichSenario - 1].levels[whichLevel - 1].subLevels.FirstOrDefault(element => element.subLevelName == name);

    public Senarios.Levels GetLevel(int whichSenario, int whichLevel) => senarios[whichSenario - 1].levels[whichLevel - 1];

    public Senarios GetSenario(int whichSenario) => senarios[whichSenario - 1];

    public float GetComplatedAllLevelPercent()
    {
        int completed = 0;
        int allLevelCount = 0;
        for (int i = 0; i < senarios.Length; i++)
        {
            for (int j = 0; j < senarios[i].levels.Length; j++)
            {
                for (int k = 0; k < senarios[i].levels[j].subLevels.Length; k++)
                {
                    allLevelCount++;
                    if (senarios[i].levels[j].subLevels[k].passed)
                    {
                        completed++;
                    }
                }
            }
        }
        return ((float)completed / (float)allLevelCount)*100;
    }

    public float[] GetComplatedLevelPercentBySenario()
    {
        int[] completed = new int[] {0, 0, 0, 0, 0};
        //int[] allLevelCount = new int[] {0, 0, 0, 0, 0};
       
        for (int i = 0; i < senarios.Length; i++)
        {
            for (int j = 0; j < senarios[i].levels.Length; j++)
            {
                for (int k = 0; k < senarios[i].levels[j].subLevels.Length; k++)
                {
                    //allLevelCount[i]++;
                    if (senarios[i].levels[j].subLevels[k].passed)
                    {
                        completed[i]++;
                    }
                }
            }
        }

        float[] complatedPercent = new float[] {0, 0, 0, 0, 0};

        for (int i = 0; i < complatedPercent.Length; i++)
        {
            complatedPercent[i] = completed[i] / 9 * 100;
        }
        return complatedPercent;
    }


}
