using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using PlayFab;
using PlayFab.ClientModels;
using Polyglot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum InputBoxType {
    Email,
    Username,
    Password,
    Playername,
    EmailRecovery
}
public class InputBoxFormatCheck : MonoBehaviour {
    private TMP_InputField inputField;
    public TMP_InputField InputField => inputField;

    [SerializeField] 
    private int minimumText = 0;

    [SerializeField] 
    private InputBoxType inputBoxType;

    private bool isSatisfied = false;
    public bool IsSatisfied => isSatisfied;

    [SerializeField] private float playfabCheckingTimeInterval = 0.5f;

    private float timer = 0;

    /// <summary>
    /// This event is constantly called when the input text satisfies/unsatisfies the condition of this input box
    /// </summary>
    public Action<bool,string> onConditionChanged;

    [SerializeField]
    private string errorMessage;

    public InputBoxType GetInputBoxType() {
        return inputBoxType;
    }
    void Awake() {
        inputField = GetComponentInChildren<TMP_InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inputField.text.Length > 0) {
            switch (inputBoxType)
            {
                case InputBoxType.Email:
                    isSatisfied = IsEmail(inputField.text);
                    break;
                case InputBoxType.Username:
                    isSatisfied = IsUsername(inputField.text);
                    break;
                case InputBoxType.Password:
                    isSatisfied = IsPassword(inputField.text);
                    break;
                case InputBoxType.Playername:
                    break;
                case InputBoxType.EmailRecovery:
                    isSatisfied = IsEmailRecovery(inputField.text);
                    break;
            }

            onConditionChanged?.Invoke(isSatisfied, errorMessage==""?"":Localization.Get(errorMessage));
            
        }
        else {
            errorMessage = "";
            isSatisfied = true;

            noDuplicateUsername = false;
            noDuplicateEmail = false;

            if (lastEmail != "" || lastUserName != "") {
                onConditionChanged?.Invoke(isSatisfied, Localization.Get(errorMessage));
                lastUserName = "";
                lastEmail = "";
            }

            timer = 0;
        }


        timer += Time.deltaTime;
        
    }

    private string lastEmail = "";
    private bool noDuplicateEmail = false;

    private bool IsEmail(string strInput)
    {
        Regex reg = new Regex( @"\w[-\w.+]*@([A-Za-z0-9][-A-Za-z0-9]+\.)+[A-Za-z]{2,14}");

        if (reg.IsMatch(strInput)) {
            
            if (lastEmail != strInput && CheckTimer()) {
                lastEmail = strInput;
                PlayFabClientAPI.LoginWithEmailAddress(new LoginWithEmailAddressRequest
                {
                    Email = strInput,
                    Password = "dwahaiuhwrahuailrnaionaiow"
                }, result => {
                }, error => {
                    print(error.Error.ToString());
                    if (error.Error == PlayFabErrorCode.AccountNotFound)
                    {
                        noDuplicateEmail = true;
                    }
                    else {
                        noDuplicateEmail = false;
                        errorMessage = "LAUNCHER_EMAIL_EXIST";
                    }
                });
            }
            
            return noDuplicateEmail;
        }
        else {
            errorMessage = "LAUNCHER_EMAIL_ERROR_1";
            return false;
        }
    }


    private string lastUserName = "";
    private bool noDuplicateUsername = false;
    private bool IsUsername(string strInput) {
        Regex reg = new Regex(@"^\w+$");
        if (reg.IsMatch(strInput))
        {
            if (lastUserName != strInput&&CheckTimer()) {
                lastUserName = strInput;
                
                PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest
                {
                    Username = strInput,
                    Password = "dwahaiuhwrahuailrnaionaiow"
                }, result => {
                }, error => {
                    print(error.Error.ToString());
                    if (error.Error == PlayFabErrorCode.AccountNotFound)
                    {
                        noDuplicateUsername = true;
                    }
                    else if(error.Error==PlayFabErrorCode.InvalidParams) {
                        noDuplicateUsername = false;
                        errorMessage = "LAUNCHER_USERNAME_ERROR_2";
                    }
                    else {
                        noDuplicateUsername = false;
                        errorMessage = "LAUNCHER_USERNAME_EXIST";
                    }
                });
            }
         
            return noDuplicateUsername;
        }
        else
        {
            errorMessage = "LAUNCHER_USERNAME_ERROR_1";
            return false;
        }
    }

    private bool IsPassword(string strInput)
    {
        Regex reg = new Regex(@"(?=(.*[a-z]))(?=(.*[A-Z]))(?=(.*\d))^.{8,32}$");
        if (reg.IsMatch(strInput))
        {
            return true;
        }
        else
        {
            errorMessage = "LAUNCHER_PASSWORD_DES";
            return false;
        }
    }
    private bool IsEmailRecovery(string strInput)
    {
        Regex reg = new Regex(@"\w[-\w.+]*@([A-Za-z0-9][-A-Za-z0-9]+\.)+[A-Za-z]{2,14}");

        if (reg.IsMatch(strInput))
        {
            return true;
        }
        else
        {
            errorMessage = "LAUNCHER_EMAIL_ERROR_1";
            return false;
        }
    }
    private bool CheckTimer()
    {
        if (timer > playfabCheckingTimeInterval) {
            timer = 0;
            return true;
        }

        return false;
    }
}
