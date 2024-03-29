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
    // �e�L�X�g�A�E�g�v�b�g
    [SerializeField] private Text outputText;
    [SerializeField] private Image outputImage;

    // �ǂݍ��񂾃e�L�X�g
    private string _loadedText = "";

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //

    // StandaloneFileBrowser�̃u���E�U�X�N���v�g�v���O�C������Ăяo��
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    // �t�@�C�����J��
    public void OnPointerDown(PointerEventData eventData) {
        UploadFile(gameObject.name, "OnFileUpload", ".", false);
    }

    // �t�@�C���A�b�v���[�h��̏���
    public void OnFileUpload(string url) {
        StartCoroutine(Load(url));
    }

#else
    //
    // OS�r���h & Unity editor��
    //
    public void OnPointerDown(PointerEventData eventData) { }

    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(() => OpenFile());
    }

    // �t�@�C�����J��
    public void OpenFile()
    {
        // �g���q�t�B���^
        var extensions = new[] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg" ),
        };

        // �t�@�C���_�C�A���O���J��
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
        Load(paths[0]);
    }

#endif
    // �t�@�C���ǂݍ���
    private void Load(string path)
    {
        byte[] readBinary = ReadPngFile(path);

        int pos = 16; // 16�o�C�g����J�n

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