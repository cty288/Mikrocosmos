using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.ServerModels;
using Polyglot;
using UnityEngine;
using UnityEngine.UI;

public class ForgotPasswordPanel : MonoBehaviour {
    [SerializeField] private Button exitButton;
    [SerializeField] private Button page1ContinueButton;
    [SerializeField] private RectTransform panelParent;
    [SerializeField] private Button resendEmailButton;

    [SerializeField] private float resendTimeInterval = 60f;
    private float resendTimer = 0f;
    private Text resendTimerText;

    private InputBoxFormatCheck emailInputBox;
    private LoadingCircle loadingCircle;
    private bool loading = false;
    private Animation animation;
    private string verifiedEmail = "";
    private string titleID = "34FEB";

    private int page = 1;

    void Awake() {
        emailInputBox = GetComponentInChildren<InputBoxFormatCheck>();
        loadingCircle = GetComponentInChildren<LoadingCircle>();
        animation = GetComponent<Animation>();
        resendTimerText = resendEmailButton.GetComponentInChildren<Text>();
    }

    private void OnEnable() {
        animation.clip = animation.GetClip("ForgotPwdPanel");
        panelParent.anchoredPosition = new Vector2(0, 0);
        emailInputBox.InputField.text = "";
    }

    void Start() {
        page1ContinueButton.onClick.AddListener(OnPage1ContinueButtonClicked);
        exitButton.onClick.AddListener(onExitButtonClicked);
        resendEmailButton.onClick.AddListener(OnResendEmailButtonClicked);
    }

    // Update is called once per frame
    void Update() {
        page1ContinueButton.interactable = emailInputBox.IsSatisfied && !loading && emailInputBox.InputField.text.Length>0;
        exitButton.interactable = !loading;
        UpdateResendEmailButtonAndTimer();
    }

    private void UpdateResendEmailButtonAndTimer() {
        resendTimer -= Time.deltaTime;
        resendEmailButton.interactable = resendTimer <= 0;
        if (resendTimer <= 0) {
            resendTimerText.text = Localization.Get("LAUNCHER_ACCOUNT_RECOVERY_RESEND");
        }
        else {
            int remainingTimeWholeNumber = Mathf.RoundToInt(resendTimer);
            resendTimerText.text = Localization.GetFormat("LAUNCHER_ACCOUNT_RECOVERY_RESEND_HAS_TIMER",
                remainingTimeWholeNumber);
        }
    }
    private void onExitButtonClicked() {
        if (!animation.isPlaying) {
            animation.clip = animation.GetClip("ForgotPwdPanelBackwards");
            animation.Play();
        }
    }

    private void OnPage1ContinueButtonClicked() {
        loading = true;
        loadingCircle.StartLoadingCircle();
        Launcher._instance.VerifyEmailAccountExistence(emailInputBox.InputField.text,
            email => {
                StopLoading();
                verifiedEmail = email;
                //next page
                NextPage();
                //send recovery email
                SendRecoveryEmail();
            }, () => {
                StopLoading();
                Launcher._instance.SetErrorMessage("LAUNCHER_EMAIL_NOT_FOUND",emailInputBox.InputField.text);
            });
    }

    private void StopLoading() {
        loadingCircle.StopLoadingCircle();
        loading = false;
    }

    private void OnPanelClose() {
        animation.clip = animation.GetClip("ForgotPwdPanel");
        gameObject.SetActive(false);
        page = 0;
    }

    private void NextPage() {
        page++;
        panelParent.DOAnchorPosX(-1280 * (page - 1), 0.5f);
    }

    private void SendRecoveryEmail() {
        PlayFabClientAPI.SendAccountRecoveryEmail(new SendAccountRecoveryEmailRequest {
            Email = verifiedEmail,
            TitleId = titleID
        }, result => {
            
        }, error => {
            print(error.Error.ToString());
        });
    }

    private void OnResendEmailButtonClicked() {
        resendTimer = resendTimeInterval;
        SendRecoveryEmail();
    }
}
