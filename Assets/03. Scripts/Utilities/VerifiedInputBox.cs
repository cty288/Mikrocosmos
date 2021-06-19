using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using Polyglot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputBoxFormatCheck))]
public class VerifiedInputBox : MonoBehaviour {
    private InputBoxFormatCheck checker;

    [SerializeField]
    private Text errorText;

    private bool hasError = true;

    private TMP_InputField inputField;
    public TMP_InputField InputField => inputField;

    /// <summary>
    /// Whether the current input box has any formatting error
    /// </summary>
    public bool HasError => hasError;

    private InputBoxType inputBoxType;
    public InputBoxType InputBoxType => inputBoxType;

    void Awake() {
        inputField = GetComponentInChildren<TMP_InputField>();
        checker = GetComponent<InputBoxFormatCheck>();
        inputBoxType = checker.GetInputBoxType();
        checker.onConditionChanged += HandleOnInputFieldConditionChanged;
    }

    void OnDestroy() {
        checker.onConditionChanged -= HandleOnInputFieldConditionChanged;
    }
    void Update() {
        
    }

    private void HandleOnInputFieldConditionChanged(bool condition, string errorMessage) {
        hasError = !condition;
        if (!condition) {
            errorText.text = errorMessage;
        }
        else {
            errorText.text = "";
        }
    }
}
