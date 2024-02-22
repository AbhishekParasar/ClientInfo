using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using SimpleJSON;
using DG.Tweening;

public class ClientManager : MonoBehaviour
{
    public TMP_Dropdown filterDropdown;
    public GameObject clientListItemPrefab;
    public Transform clientListContainer;
    public GameObject popupWindow;
    public TMP_Text popupNameText;
    public TMP_Text popupPointsText;
    public TMP_Text popupAddressText;

   [SerializeField] private List<GameObject> AllClients = new List<GameObject>();
    [SerializeField] private List<GameObject> AllManagers = new List<GameObject>();
    [SerializeField] private List<GameObject> NonManagers = new List<GameObject>();

    [SerializeField] private List<string> Listlabels = new List<string>();
    [SerializeField] private List<string> Listids = new List<string>();
    [SerializeField] private List<string> ListofManagerorNot = new List<string>();

    [SerializeField] private List<string> ListofName = new List<string>();
    [SerializeField] private List<string> ListofAddress = new List<string>();
    [SerializeField] private List<int> ListofPoints = new List<int>();

    private void Start()
    {
        StartCoroutine(FetchClientData());
        filterDropdown.onValueChanged.AddListener(delegate {
            OnFilterChanged(filterDropdown);
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
            Debug.LogError(jsonNode);
            for (int i = 0; i < jsonNode["clients"].Count; i++)
            {
                GameObject temp = Instantiate(clientListItemPrefab, clientListContainer);
                Listlabels.Add(jsonNode["clients"][i]["label"]);
                Listids.Add(jsonNode["clients"][i]["id"]);
                ListofManagerorNot.Add(jsonNode["clients"][i]["isManager"]);
                temp.transform.GetChild(1).GetComponent<TMP_Text>().text = jsonNode["clients"][i]["label"];
                temp.transform.GetChild(0).GetComponent<TMP_Text>().text = jsonNode["clients"][i]["id"];
                
                int index = i;
                temp.GetComponent<Button>().onClick.AddListener(() => ShowPopUp(index));
                // Add the instantiated object to the appropriate list based on whether it's a manager or not
                if (jsonNode["clients"][i]["isManager"].Value.ToLower() == "true")
                {
                    AllManagers.Add(temp);
                }
                else if (jsonNode["clients"][i]["isManager"].Value.ToLower() == "false")
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

    void OnFilterChanged(TMP_Dropdown dropdown)
    {
        switch (dropdown.value)
        {
            case 0: // All clients
                ShowClients(AllClients);
                break;
            case 1: // Managers only
                ShowClients(AllManagers);
                break;
            case 2: // Non-managers only
                ShowClients(NonManagers);
                break;
        }
    }

    void ShowClients(List<GameObject> clients)
    {
        foreach (GameObject client in AllClients)
        {
            client.SetActive(clients.Contains(client));
        }
    }

    void ShowPopUp(int index)
    {
        // Show popup window
        popupWindow.SetActive(true);
        // Populate popup window with data
        popupNameText.text = ListofName[index];
        popupPointsText.text = ListofPoints[index].ToString();
        popupAddressText.text = ListofAddress[index];
        // Add animations using DOTween
        // For example, you can animate the popup window's scale from zero to one over 0.3 seconds
        popupWindow.transform.GetChild(0).localScale = Vector3.zero;
        popupWindow.transform.GetChild(0).DOScale(Vector3.one, 0.3f);
    }
    public void ClosePopUp()
    {
        popupWindow.transform.GetChild(0).DOScale(Vector3.zero, 0.3f);
        popupWindow.SetActive(false);
    }
}
