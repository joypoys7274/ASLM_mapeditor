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
public class FileSaveSample : MonoBehaviour, IPointerDownHandler
{
    // �e�L�X�g�A�E�g�v�b�g
    [SerializeField] private Text outputText;

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //

    // StandaloneFileBrowser�̃u���E�U�X�N���v�g�v���O�C������Ăяo��
    [DllImport("__Internal")]
    private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);

    // �t�@�C����ۑ�����
    public void OnPointerDown(PointerEventData eventData) {
        var str = outputText.text + "_saved";

        if (str.Length > 0)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            DownloadFile(gameObject.name, "OnFileDownload", "sample_saved.txt", bytes, bytes.Length);
        }
    }

    // �t�@�C���_�E�����[�h��̏���
    public void OnFileDownload() {
        Debug.Log("CSV file saved");
        outputText.text = "File Saved";
    }

#else

    //
    // OS�r���h & Unity editor��
    //

    public void OnPointerDown(PointerEventData eventData) { }

    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(() => SaveFile());
    }

    // �t�@�C����ۑ�����
    public void SaveFile()
    {
        var str = outputText.text + "_saved";

        if (str.Length > 0)
        {
            var path = StandaloneFileBrowser.SaveFilePanel("�t�@�C���̕ۑ�", "", "sample_saved", "txt");
            if (!string.IsNullOrEmpty(path))
            {
                File.WriteAllText(path, str);
                Debug.Log("File saved");
                outputText.text = "File Saved";
            }
        }
    }

#endif

}