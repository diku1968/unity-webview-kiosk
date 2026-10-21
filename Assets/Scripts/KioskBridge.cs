using UnityEngine;
using Debug = UnityEngine.Debug;
using System.IO;
using System.Collections.Generic;
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
using Gpm.WebView;
#endif

public class KioskBridge : MonoBehaviour
{
    [Header("Kiosk Settings")]
    [Tooltip("The HTML file located inside the Assets/StreamingAssets directory.")]
    public string htmlFileName = "index.html";

    private void Start()
    {
        Debug.Log("[KioskBridge] Kiosk Manager Initialized.");
        LoadLocalKiosk();
    }

    public void LoadLocalKiosk()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, htmlFileName);
        Debug.Log("[KioskBridge] Loading HTML from: " + filePath);

        // If running inside the Unity Editor on Windows, open in default web browser for easy testing
        if (Application.isEditor)
        {
            Debug.Log("[KioskBridge] Running in Unity Editor. Opening local HTML in default web browser.");
            Application.OpenURL("file://" + filePath);
            return;
        }

#if UNITY_ANDROID || UNITY_IOS
        // Configure WebView settings for Mobile Kiosk mode
        GpmWebViewRequest.Configuration config = new GpmWebViewRequest.Configuration
        {
            style = GpmWebViewStyle.FULLSCREEN,
            isNavigationBarVisible = false,
            isBackButtonVisible = false,
            isClearCache = false,
            isClearCookie = false
        };

        List<string> schemes = new List<string> { "uniwebview://", "kiosk://" };

        GpmWebView.ShowHtmlFile(
            filePath,
            config,
            OnWebViewCallback,
            schemes
        );
#else
        Debug.LogWarning("[KioskBridge] GpmWebView is only supported on Android and iOS devices.");
#endif
    }

    private void OnWebViewCallback(
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        GpmWebViewCallback.CallbackType callbackType, string data, GpmWebViewError error
#else
        int callbackType, string data, object error
#endif
    )
    {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        if (error != null)
        {
            Debug.LogError($"[KioskBridge] WebView Error ({error.code}): {error.message}");
            return;
        }

        if (!string.IsNullOrEmpty(data) && (data.StartsWith("uniwebview://") || data.StartsWith("kiosk://")))
        {
            ProcessJavaScriptCommand(data);
            return;
        }

        switch (callbackType)
        {
            case GpmWebViewCallback.CallbackType.Open:
                Debug.Log("[KioskBridge] WebView Opened Successfully.");
                break;
            case GpmWebViewCallback.CallbackType.Close:
                Debug.Log("[KioskBridge] WebView Closed.");
                break;
            case GpmWebViewCallback.CallbackType.PageLoad:
                Debug.Log($"[KioskBridge] Page Load Finished: {data}");
                break;
        }
#endif
    }

    private void ProcessJavaScriptCommand(string url)
    {
        if (string.IsNullOrEmpty(url)) return;

        Debug.Log($"[KioskBridge] Received JS Scheme Command: {url}");

        if (url.Contains("hardware-scan"))
        {
            string itemParam = ExtractQueryParam(url, "item");
            TriggerHardwareScan(itemParam);
        }
        else if (url.Contains("print-receipt"))
        {
            TriggerPrint();
        }
    }

    public void TriggerHardwareScan(string item)
    {
        Debug.Log($"[KioskBridge] Executing Hardware Scan for Item: {item}");
    }

    public void TriggerPrint()
    {
        Debug.Log("[KioskBridge] Executing Thermal Print Command...");
    }

    public void SendStatusToHTML(string statusText)
    {
        string jsCode = $"updateKioskStatus('{statusText}');";
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        GpmWebView.ExecuteJavaScript(jsCode);
#endif
    }

    private string ExtractQueryParam(string url, string paramName)
    {
        if (!url.Contains("?")) return string.Empty;

        string queryString = url.Substring(url.IndexOf('?') + 1);
        string[] pairs = queryString.Split('&');

        foreach (string pair in pairs)
        {
            string[] keyValue = pair.Split('=');
            if (keyValue.Length == 2 && keyValue[0].Equals(paramName, System.StringComparison.OrdinalIgnoreCase))
            {
                return System.Uri.UnescapeDataString(keyValue[1]);
            }
        }

        return string.Empty;
    }

    private void OnDestroy()
    {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        GpmWebView.Close();
#endif
    }
}