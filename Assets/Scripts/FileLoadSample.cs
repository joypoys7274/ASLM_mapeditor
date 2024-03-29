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
            new ExtensionFilter("All Files", "*" ),
        };

        // �t�@�C���_�C�A���O���J��
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
        if (paths.Length > 0 && paths[0].Length > 0)
        {

            StartCoroutine(Load(new System.Uri(paths[0]).AbsoluteUri));

        }
    }

#endif
    // �t�@�C���ǂݍ���
    private IEnumerator Load(string url)
    {
        var request = UnityWebRequest.Get(url);

        var operation = request.SendWebRequest();
        while (!operation.isDone)
        {
            yield return null;
        }

        _loadedText = request.downloadHandler.text;
        Debug.Log(_loadedText);
        outputText.text = _loadedText;
    }

}