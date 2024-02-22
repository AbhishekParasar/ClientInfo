using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RestAPI2 : MonoBehaviour
{
   
    #region POSTAPI
    public IEnumerator Login(string url)
    {
        JSONNode json = new JSONObject();
        json["emailId"] = "abhi.silverzone@gmail.com";
        json["password"] = "ramramji";
        json["roleId"] = 27;
        json["isSocialMedia"] = false;
        // Convert the JSON object to a string
        string jsonPayload = json.ToString();
        // Create a UnityWebRequest and set the request headers
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("accept", "*/*");
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError("Is Network Error");
        }
        else
        {
            string response = request.downloadHandler.text;
            Debug.Log("Response: " + response);
        }
    }
    #endregion
}


