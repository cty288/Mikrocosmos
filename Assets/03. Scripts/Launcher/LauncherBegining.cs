using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Polyglot;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LauncherBegining : MonoBehaviour
{
    [SerializeField]
    private RectTransform progressbar = null;

    private float startWidth;

    [DllImport("user32.dll")]
    static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    void Start()
    {
        Screen.SetResolution(800, 100, false);
        if (Application.platform != RuntimePlatform.WindowsEditor)
        {
            StartCoroutine(SetWindows());
        }

        startWidth = progressbar.sizeDelta.x;
        LocalizationImporter.onDownloadCustonSheet += OnDownloadComplete;
        StartCoroutine(LocalizationImporter.DownloadCustomSheet(UpdateProgressbar));
    }

    void OnDestroy() {
        LocalizationImporter.onDownloadCustonSheet -= OnDownloadComplete;
    }

    private IEnumerator SetWindows() {
        yield return new WaitForSeconds(0.1f);	
        SetWindowLong(GetForegroundWindow(), -16, 0x800000);
    }

    private void OnDownloadComplete() {
        
        SceneManager.LoadSceneAsync("Launcher");
    }

    private bool UpdateProgressbar(float progress)
    {
        if (progressbar != null)
        {
            progressbar.sizeDelta = new Vector2(progress * startWidth, progressbar.sizeDelta.y);
        }

        return false;
    }
}
