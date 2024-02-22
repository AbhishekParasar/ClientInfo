using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using System.Collections;
using UnityEngine.Networking;
using SimpleJSON;
using TMPro;
using System.Collections.Generic;

public class ClientDataController : MonoBehaviour
{
    public TMP_Dropdown filterDropdown;
    public GameObject popupWindow;
    public Transform clientListContainer;
    public GameObject clientListItemPrefab;

    [Header("Clients")]
    public List<string> Listlabels;
    public List<string> Listids;
    public List<string> ListofManagerorNot;

    [Header("ClientsData")]
    public List<string> ListofName;
    public List<string> ListofAddress;
    public List<string> ListofPoints;

    public List<GameObject> AllClients;
    public List<GameObject> NonManagers;
    public List<GameObject> AllManagers;

    private string filteredClients;

    private void Start()
    {
        StartCoroutine(FetchClientData());
        filterDropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(filterDropdown);
        });
    }

    IEnumerator FetchClientData()
    {
        string url = "https://qa.sunbasedata.com/sunbase/portal/api/assignment.jsp?cmd=client_data";
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to retrieve data: " + www.error);
                yield break;
            }
            string jsonString = www.downloadHandler.text;
            JSONNode jsonNode = JSON.Parse(jsonString);
            for (int i = 0; i < jsonNode["clients"].Count; i++)
            {
                GameObject temp = Instantiate(clientListItemPrefab, clientListContainer);
                Listlabels.Add(jsonNode["clients"][i]["label"]);
                Listids.Add(jsonNode["clients"][i]["id"]);
                ListofManagerorNot.Add(jsonNode["clients"][i]["isManager"]);
                temp.transform.GetChild(0).GetComponent<TMP_Text>().text = jsonNode["clients"][i]["label"];
                temp.transform.GetChild(1).GetComponent<TMP_Text>().text = jsonNode["clients"][i]["id"];
                int index = i;
                temp.GetComponent<Button>().onClick.AddListener(() => ShowPopUp(index));
                // Add the instantiated object to the appropriate list based on whether it's a manager or not
                if (jsonNode["clients"][i]["isManager"] == "true")
                {
                    AllManagers.Add(temp);
                }
                else if(jsonNode["clients"][i]["isManager"] == "False")
                {
                    NonManagers.Add(temp);
                }
                AllClients.Add(temp);
            }
            for (int i = 0; i < jsonNode["data"].Count; i++)
            {
                ListofName.Add(jsonNode["data"][i]["name"]);
                ListofAddress.Add(jsonNode["data"][i]["address"]);
                ListofPoints.Add(jsonNode["data"][i]["points"]);
            }
        }
    }

    public void ShowPopUp(int index)
    {
        Debug.Log("Index" + index + "Name" + ListofName[index] + "address" + ListofAddress[index] + "points" + ListofPoints[index]);
    }

    void DropdownValueChanged(TMP_Dropdown change)
    {
        switch (change.value)
        {
            case 0:
                FilterClients(AllClients);
                break;
            case 1:
                FilterClients(AllManagers);
                break;
            case 2:
                FilterClients(NonManagers);
                break;
        }
    }

    void FilterClients(List<GameObject> clientsToShow)
    {
        // Set all clients inactive
        foreach (GameObject client in AllClients)
        {
            client.SetActive(false);
        }
        // Set the specified clients active
        foreach (GameObject client in clientsToShow)
        {
            client.SetActive(true);
        }
    }
}
