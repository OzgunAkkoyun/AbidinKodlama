using UnityEngine;

public class ShowInputsCode : MonoBehaviour
{
    private static ShowInputsCode _instance;
    public static ShowInputsCode Instance { get { return _instance; } }
    public string codeString;

    GameManager gm;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    public void ShowCodesString()
    {
        for (int i = 0; i < gm.commander.commands.Count; i++)
        {
            codeString += gm.commander.commands[i].ToCodeString();

            //if (gm.commander.commands[i] == Direction.Left)
            //{
            //    codeString += "Sola Dön(); \n";
            //}
            //else if (gm.commander.commands[i] == Direction.Right)
            //{
            //    codeString += "Sağa Dön(); \n";
            //}
            //else if (gm.commander.commands[i] == Direction.Forward)
            //{
            //    codeString += "İlerle(); \n";
            //}
            //else if (gm.commander.commands[i] == Direction.Backward)
            //{
            //    codeString += "Geri(); \n";
            //}
        }

        gm.uh.codeString.text = codeString;
    }
}
