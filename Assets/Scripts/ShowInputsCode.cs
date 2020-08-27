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
        }

        gm.uh.codeString.text = codeString;
    }
}
