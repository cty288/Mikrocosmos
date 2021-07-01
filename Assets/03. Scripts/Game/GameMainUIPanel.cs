using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMainUIPanel : RootPanel
{
    [SerializeField] private LoadingCircle loadingCircle;


    void Start() {

#if UNITY_SERVER
         gameObject.SetActive(false);
#endif
    }
    private void StartLoadingCircle(bool hasloadingMessage = false, bool hasLoadingPeriod = false, string
        loadingMessage = "") {
        loadingCircle.StartLoadingCircle(hasloadingMessage, hasLoadingPeriod, loadingMessage);
    }

    private void StopLoadingCircle()
    {
        loadingCircle.StopLoadingCircle();
    }
}
