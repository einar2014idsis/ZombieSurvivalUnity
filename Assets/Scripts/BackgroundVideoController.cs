using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class BackgroundVideoController : MonoBehaviour
{
    [Header("Opcional: arrastra aquí el RawImage y su AspectRatioFitter")]
    [SerializeField] private RawImage rawImage;            // BG_Video
    [SerializeField] private AspectRatioFitter fitter;     // AspectRatioFitter de BG_Video

    private VideoPlayer vp;

    void Awake()
    {
        vp = GetComponent<VideoPlayer>();

        // Ajustes seguros
        vp.playOnAwake = true;
        vp.waitForFirstFrame = true;
        vp.skipOnDrop = true;
        vp.isLooping = true;   // loop "blando"
        vp.timeReference = VideoTimeReference.Freerun;
        vp.playbackSpeed = 1f;
        vp.renderMode = VideoRenderMode.RenderTexture;

        // Eventos: ajuste de aspecto + loop "duro"
        vp.prepareCompleted += OnPrepared;
        vp.loopPointReached += OnLoopPointReached;

        if (!vp.isPrepared) vp.Prepare();
    }

    private void OnPrepared(VideoPlayer player)
    {
        // Ajusta relación de aspecto según el clip
        if (fitter != null && player.clip != null && player.clip.height > 0)
        {
            float w = player.clip.width;
            float h = player.clip.height;
            fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent; // cubrir pantalla
            fitter.aspectRatio = w / h; // ej. 1500/1000 = 1.5
        }

        if (rawImage != null && player.targetTexture != null)
            rawImage.texture = player.targetTexture;

        player.Play();
    }

    private void OnLoopPointReached(VideoPlayer player)
    {
        // En algunos H.264 el loop se queda "clavado" al final: forzamos reinicio
        player.frame = 0;
        player.Play();
    }
}
