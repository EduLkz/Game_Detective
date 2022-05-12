using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrimeScene : MonoBehaviour {

    #region Singleton
    private static CrimeScene instance;
    public static CrimeScene Instance { get { return instance; } }

    private void Awake() {
        instance = this;
    }

    #endregion

    List<Transform> photoMakers = new List<Transform>();

    public Transform container;
    public GameObject imageprefab;
    public Camera cam;

    public void AddPhotoMaker(Transform _marker) {
        photoMakers.Add(_marker);
    }

    public int GetPhotoMakerValue() {
        return photoMakers.Count + 1;
    }


    private void LeaveRoom() {
        for(int i = 0; i < photoMakers.Count; i++) {
            TakePhotos(i);
        }

        //for(int i = 0; i < photoMakers.Count; i++) {
        //    GetPhotos(i + 1);
        //}
    }

    private void TakePhotos(int _value) {
        cam.transform.position = photoMakers[_value].position - photoMakers[_value].forward;
        cam.transform.rotation = Quaternion.LookRotation(cam.transform.position - photoMakers[_value].position);


        RenderTexture _renderTexture = RenderTexture.active;
        RenderTexture.active = cam.targetTexture;

        cam.Render();

        Texture2D image = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        image.Apply();
        RenderTexture.active = _renderTexture;

        byte[] bytes = image.EncodeToPNG();
        Destroy(image);

        File.WriteAllBytes(Application.dataPath + "/scenePics/pics" + (_value + 1).ToString("000") + ".png", bytes);
        Debug.Log(Application.dataPath + "/scenePics/pics");


        Texture2D Tex2D;
        Tex2D = new Texture2D(2, 2);
        Tex2D.LoadImage(bytes);

        Sprite _spt = Sprite.Create(Tex2D, new Rect(0, 0, Tex2D.width, Tex2D.height), new Vector2(0, 0), 100);

        GameObject _go = Instantiate(imageprefab);
        _go.GetComponent<Image>().sprite = _spt;

        _go.transform.SetParent(container);
    }

    private void GetPhotos(int _value) {
        Texture2D Tex2D;
        byte[] FileData;

        FileData = File.ReadAllBytes(Application.dataPath + "/scenePics/pics" + _value.ToString("000") + ".png");
        Tex2D = new Texture2D(2, 2);
        Tex2D.LoadImage(FileData);

        Sprite _spt = Sprite.Create(Tex2D, new Rect(0, 0, Tex2D.width, Tex2D.height), new Vector2(0, 0), 100);

        GameObject _go = Instantiate(imageprefab);
        _go.GetComponent<Image>().sprite = _spt;

        _go.transform.SetParent(container);
    }

    private void OnTriggerEnter(Collider other) {
        LeaveRoom();
    }
}