using UnityEngine.UI;
using TMPro;


public class MeshTab : SettingsManager
{
    public TMP_InputField meshScale;

    public Toggle useFlatshading;

    public Slider chunkSizeIndex;
    public Slider flatShadedChunkSizeIndex;

    protected override void Start()
    {
        base.Start();
        meshScale.text = simulationSettings.MeshSettings.MeshScale.ToString();
        useFlatshading.isOn = simulationSettings.MeshSettings.UseFlatShading;
        chunkSizeIndex.value = simulationSettings.MeshSettings.ChunkSizeIndex;
        flatShadedChunkSizeIndex.value = simulationSettings.MeshSettings.FlatShadedChunkSizeIndex;
    }

    public void SetSettings()
    {
        simulationSettings.MeshSettings = new MeshSettings(
            float.Parse(meshScale.text),
            useFlatshading.isOn,
            (int)chunkSizeIndex.value,
            (int)flatShadedChunkSizeIndex.value
        );
    }
}
