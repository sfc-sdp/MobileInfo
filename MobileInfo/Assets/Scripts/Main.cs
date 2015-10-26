using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class Main : MonoBehaviour
{
    public Text txtClock;
    public Text txtAcceleration;
    public Text txtCompass;
    public Text txtKeyInput;
    public Text txtTouch;
    public Text txtVibration;
    public Text txtGyro;
    public Text txtTimer;
    public Text txtGps;
    public Text txtWebCam;
    public Text txtFps;
    public Text txtResolution;
    public Text txtOrientation;

    private int screenWidth;
    private int screenHeight;
    private int prevTouchCount;
    private int frameCount;
    private float Fps;
    private float prevTime;

    void Awake()
    {
        Application.targetFrameRate = 60;
        Time.fixedDeltaTime = 0.1f;
    }

    void Start()
    {
        screenWidth = 0;
        screenHeight = 0;
        prevTouchCount = 0;
        frameCount = 0;
        Fps = 0.0f;
        prevTime = 0.0f;

        Input.gyro.enabled = true;
        Input.compass.enabled = true;
        if (Input.location.isEnabledByUser)
            Input.location.Start();
        else
            txtGps.text = "No GPS";

        if (!SystemInfo.supportsAccelerometer)
            txtAcceleration.text = "No Acce";
        if (!SystemInfo.supportsGyroscope)
            txtGyro.text = "No Gyro";
        if (!Input.touchSupported)
            txtTouch.text = "No Touch";
        if (!SystemInfo.supportsVibration)
            txtVibration.text = "No Vib";
        if (!Input.compass.enabled)
            txtCompass.text = "No Compass";

        int camNum = WebCamTexture.devices.Length;
        txtWebCam.text = camNum == 0 ? "No Camera" : string.Format("{0} camera", camNum);
    }

    void Update()
    {
        // FPS算出
        ++frameCount;
        float time = Time.realtimeSinceStartup - prevTime;
        if (time >= 0.5f) {
            Fps = frameCount / time;
            frameCount = 0;
            prevTime = Time.realtimeSinceStartup;
        }

        // キー入力
        if (Input.anyKeyDown) {
            txtKeyInput.text = "Key: " + Input.inputString;
        }

        // マルチタッチ入力
        int touchCount = Input.touchCount;
        if (touchCount != prevTouchCount) {
            txtTouch.text = string.Format("{0} touch", touchCount);
            prevTouchCount = touchCount;
        }
    }

    void FixedUpdate()
    {
        // 日時
        txtClock.text = DateTime.Now.ToString("MM/dd hh:mm");

        // 加速度センサー
        if (SystemInfo.supportsAccelerometer) {
            Vector3 acce = Input.acceleration;
            txtAcceleration.text = string.Format("X: {0:f1}, Y: {1:f1}, Z:{2:f1}", acce.x, acce.y, acce.z);
        }

        // 方位センサー
        if (Input.compass.enabled) {
            txtCompass.text = string.Format("{0:f2}deg", Input.compass.magneticHeading);
        }

        // ジャイロセンサー
        if (SystemInfo.supportsGyroscope && Input.gyro.enabled)
        {
            Vector3 gyro = Input.gyro.rotationRate;
            txtAcceleration.text = string.Format("X: {0:f1}, Y: {1:f1}, Z:{2:f1}", gyro.x, gyro.y, gyro.z);
        }

        // 経過時間
        if (Time.realtimeSinceStartup < 60.0f) {
            txtTimer.text = string.Format("{0:f1}sec", Time.realtimeSinceStartup);
        } else {
            txtTimer.text = string.Format("{0:f1}min", Time.realtimeSinceStartup / 60);
        }

        // 位置情報
        if (Input.location.isEnabledByUser) {
            switch (Input.location.status) {
                case LocationServiceStatus.Failed: txtGps.text = "GPS Failed"; break;
                case LocationServiceStatus.Initializing: txtGps.text = "GPS Init"; break;
                case LocationServiceStatus.Running: txtGps.text = string.Format("lat: {0:f1}, lng: {1:f1}", Input.location.lastData.latitude, Input.location.lastData.longitude); break;
            }
        }

        // 実フレームレート
        txtFps.text = String.Format("{0:f1}fps", Fps);

        // 解像度、端末の向き
        if (screenWidth != Screen.width || screenHeight != Screen.height) {
            screenWidth = Screen.width;
            screenHeight = Screen.height;
            txtResolution.text = string.Format("{0}x{1}", screenWidth, screenHeight);
            txtOrientation.text = screenWidth < screenHeight ? "Portrait" : screenWidth == screenHeight ? "Square" : "Landscape";
        }
    }

    void OnDisable() {
        Input.location.Stop();
    }

    public void onClickVibration() {
        Handheld.Vibrate();
    }
}
