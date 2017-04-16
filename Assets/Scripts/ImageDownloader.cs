using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ImageDownloader : MonoBehaviour
{
    public static readonly string PicDir = "news";

    private Texture2D _texture;

    private bool _ok;

    void Awake()
    {
        _texture = new Texture2D(2, 2, TextureFormat.PVRTC_RGBA4, false);
    }

    public IEnumerator DownloadImage(string id, string link)
    {
        var imageComponent = GetComponent<Image>();

        var www = new WWW(InfoStorage.Server + InfoStorage.NewsImages + id + "/" + link);

        yield return www;

        if (www.error == null)
        {
            _ok = true;

            _texture = www.texture;

            if (!isActiveAndEnabled) yield break;

            LocalStorage.Save(PicDir + "/" + link, _texture.EncodeToJPG());
        }
        else if (LocalStorage.FileExists(PicDir + "/" + link))
        {
            _ok = true;

            var rawTexture = (byte[]) LocalStorage.Load(PicDir + "/" + link);

            //InfoStorage.TextureBuffer.LoadImage(rawTexture);
            _texture.LoadImage(rawTexture);
        }

        if (_ok)
        {
/*            imageComponent.sprite = Sprite.Create(InfoStorage.TextureBuffer,
                new Rect(0, 0, InfoStorage.TextureBuffer.width, InfoStorage.TextureBuffer.height),
                new Vector2(0.5f, 0.5f), 100F, 0, SpriteMeshType.FullRect);
            imageComponent.preserveAspect = true;
            InfoStorage.RelaseMemory();*/

            imageComponent.sprite = Sprite.Create(_texture,
                new Rect(0, 0, _texture.width, _texture.height),
                new Vector2(0.5f, 0.5f), 100F, 0, SpriteMeshType.FullRect);
            imageComponent.preserveAspect = true;

//            InfoStorage.RelaseMemory();

        }
        else
        {
            imageComponent.sprite = NewsController.Temp;
        }
    }

    void OnDestroy()
    {
        Destroy(_texture);
    }
}