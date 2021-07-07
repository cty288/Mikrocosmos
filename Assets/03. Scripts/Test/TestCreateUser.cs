using System;
using System.Collections;
using System.Collections.Generic;
using MikrocosmosDatabase;
using UnityEngine;
/*
public class TestCreateUser : MonoBehaviour {
    public string username;
    public int id;
    public string password;
    public string email;
    private UserTableManager manager;
    void Start() {
        manager = new UserTableManager();
    }
    public void OnCreateUserButtonClicked() {
        User user = new User() { Username = username, Password = password, LastLoginTime = DateTime.Now};
        manager.Add(user);
    }

    public void OnDeleteUserButtonClicked() {
        User user = new User() {Id = id,Username = " ",Password = " "};
        manager.Remove(user);
    }

    public void OnSearchByIdButtonClicked() {
        User result= manager.GetById(id);
        if (result!=null) {
            Debug.Log($"found {result.Username}");
        }
       
    }

    public void OnSearchByFieldNameButtonClicked() {
        string[] fieldNames = new string[] {"Username", "Password"};
        object[] values = new object[] {username, password};
        IList<User> results= manager.SearchByFieldNames(fieldNames, values);
        Debug.Log($"found id {results[0].Id}");
    }
}*/
