using UnityEngine;
using UnityEngine.Rendering;

// https://www.youtube.com/watch?v=YP723zBCXfk

[RequireComponent(typeof(Camera))]
public class AreaCompute : MonoBehaviour
{
    public ComputeShader compute_shader;
    [HideInInspector] public uint[] data;

    protected ComputeBuffer compute_buffer;

    public float Area
    {
        get => area;
    }

    float area = 0.0f;

    int width, height;

    int handle_init;
    int handle_main;

    void OnEnable()
    {
        handle_init = compute_shader.FindKernel("CSInit");
        handle_main = compute_shader.FindKernel("CSMain");

        compute_buffer = new ComputeBuffer(1, sizeof(uint));
        data = new uint[1];

        RenderPipelineManager.endCameraRendering += ComputeArea;
    }

    void OnDisable()
    {
        RenderPipelineManager.endCameraRendering -= ComputeArea;

        Cleanup();   
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), (area * 100.0f).ToString());
    }

    public void ComputeArea(ScriptableRenderContext context, Camera camera)
    {
        Debug.Log("Compute");

        width = Screen.width;
        height = Screen.height;

        compute_shader.SetTextureFromGlobal(handle_main, "image", "_MaskBuffer");
        compute_shader.SetBuffer(handle_main, "compute_buffer", compute_buffer);
        compute_shader.SetBuffer(handle_init, "compute_buffer", compute_buffer);

        compute_shader.Dispatch(handle_init, 64, 1, 1);
        compute_shader.Dispatch(handle_main, width / 8, height / 8, 1);

        compute_buffer.GetData(data);
        uint result = data[0];

        area = (float)result / ((float)width * (float)height);
    }

    void Cleanup()
    {
        if (compute_buffer != null)
        {
            compute_buffer.Release();
            compute_buffer = null;
        }
    }
}
