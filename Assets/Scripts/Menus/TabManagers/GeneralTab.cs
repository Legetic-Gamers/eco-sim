using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneralTab : SettingsManager
{
    public TMP_InputField xSize;
    public TMP_InputField ySize;

    protected override void Start()
    {
        base.Start();
        xSize.text = simulationSettings.xFixedSize.ToString();
        ySize.text = simulationSettings.yFixedSize.ToString();
    }

    public void SetSettings()
    {
        simulationSettings.xFixedSize = int.Parse(xSize.text);
        simulationSettings.yFixedSize = int.Parse(ySize.text);
    }

}
