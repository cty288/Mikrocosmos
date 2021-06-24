using System.Collections;
using System.Collections.Generic;
using Polyglot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCircle : MonoBehaviour {

    [SerializeField] private Animation animation;
    [SerializeField] private GameObject loadingTextObj;

    private string mainInfo = "";
    private string periodString = "";

    private bool hasLoadingMessage = false;
    private bool hasWaitingPeriod = false;

    [SerializeField] 
    private float waitingPeriodUpdateTimeInterval = 0.3f;

    private float timer = 0;
    private const int periodMaxNumber = 3;
    private int currentPeriodNumber = 0;

    void Update() {
        if (hasWaitingPeriod && loadingTextObj && hasLoadingMessage)
        {
            timer += Time.deltaTime;
            if (timer >= waitingPeriodUpdateTimeInterval)
            {
                timer = 0;
                currentPeriodNumber++;
                currentPeriodNumber %= periodMaxNumber + 1;
            }

            switch (currentPeriodNumber)
            {
                case 0:
                    periodString = "";
                    break;
                case 1:
                    periodString = ".";
                    break;
                case 2:
                    periodString = "..";
                    break;
                case 3:
                    periodString = "...";
                    break;
            }

            loadingTextObj.GetComponent<TMP_Text>().text = mainInfo + periodString;
        }
        else
        {
            loadingTextObj.GetComponent<TMP_Text>().text = mainInfo;
        }
    }



    /// <summary>
    /// Start playing the animation
    /// </summary>
    public void StartLoadingCircle(bool hasloadingMessage=false, bool hasLoadingPeriod=false, string 
        loadingMessageLocalized="") {
        if (animation) {
            animation.gameObject.SetActive(true);
            animation.Play();
        }

        this.hasLoadingMessage = hasloadingMessage;
        this.mainInfo = "";
        this.hasWaitingPeriod = false;

        if (loadingTextObj) {
            loadingTextObj.SetActive(false);
        }
        

        if (this.hasLoadingMessage) {
            this.mainInfo = Localization.Get(loadingMessageLocalized);
            this.hasWaitingPeriod = hasLoadingPeriod;

            if (loadingTextObj.gameObject) {
                loadingTextObj.SetActive(true);
            }

        }

    }

    /// <summary>
    /// Change the loading message
    /// </summary>
    /// <param name="hasLoadingPeriod"></param>
    /// <param name="loadingMessageLocalized"></param>
    public void ChangeLoadingMessage(bool hasLoadingPeriod = false, string loadingMessageLocalized = "") {
        this.mainInfo = Localization.Get(loadingMessageLocalized);
        this.hasWaitingPeriod = hasLoadingPeriod;
    }

    /// <summary>
    /// stop playing the animation
    /// </summary>
    public void StopLoadingCircle() {
        if (animation) {
            animation.gameObject.SetActive(false);
            animation.Stop();
        }

    }
}
