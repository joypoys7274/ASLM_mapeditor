using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class FileLoadSample : MonoBehaviour, IPointerDownHandler
{
    // テキストアウトプット
    [SerializeField] private Text outputText;
    [SerializeField] private Image outputImage;

    // 読み込んだテキスト
    private string _loadedText = "";

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //

    // StandaloneFileBrowserのブラウザスクリプトプラグインから呼び出す
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    // ファイルを開く
    public void OnPointerDown(PointerEventData eventData) {
        UploadFile(gameObject.name, "OnFileUpload", ".", false);
    }

    // ファイルアップロード後の処理
    public void OnFileUpload(string url) {
        StartCoroutine(Load(url));
    }

#else
    //
    // OSビルド & Unity editor上
    //
    public void OnPointerDown(PointerEventData eventData) { }

    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(() => OpenFile());
    }

    // ファイルを開く
    public void OpenFile()
    {
        // 拡張子フィルタ
        var extensions = new[] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg" ),
        };

        // ファイルダイアログを開く
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
        Load(paths[0]);
    }

#endif
    // ファイル読み込み
    private void Load(string path)
    {
        byte[] readBinary = ReadPngFile(path);

        int pos = 16; // 16バイトから開始

        int width = 0;
        for (int i = 0; i < 4; i++)
        {
            width = width * 256 + readBinary[pos++];
        }

        int height = 0;
        for (int i = 0; i < 4; i++)
        {
            height = height * 256 + readBinary[pos++];
        }

        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(readBinary);

        Sprite createdSprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0, 1), 1);
        outputImage.sprite = createdSprite;
    }

    byte[] ReadPngFile(string path)
    {
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        BinaryReader bin = new BinaryReader(fileStream);
        byte[] values = bin.ReadBytes((int)bin.BaseStream.Length);

        bin.Close();

        return values;
    }

    Texture ReadTexture(string path, int width, int height)
    {
        byte[] readBinary = ReadPngFile(path);

        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(readBinary);

        return texture;
    }

}