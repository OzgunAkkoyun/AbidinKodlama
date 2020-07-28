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
        for (int i = 0; i < gm.inputs.inputs.Count; i++)
        {
            if (gm.inputs.inputs[i] == GetInputs.code.Left)
            {
                codeString += "Sola Dön(); \n";
            }
            else if (gm.inputs.inputs[i] == GetInputs.code.Right)
            {
                codeString += "Sağa Dön(); \n";
            }
            else if (gm.inputs.inputs[i] == GetInputs.code.Forward)
            {
                codeString += "İlerle(); \n";
            }
            else if (gm.inputs.inputs[i] == GetInputs.code.Backward)
            {
                codeString += "Geri(); \n";
            }
        }

        gm.uh.codeString.text = codeString;
    }
}
