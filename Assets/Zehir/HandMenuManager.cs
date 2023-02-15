using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandMenuManager : MonoBehaviour
{
    public int currentTab = 0;
    public Transform imageTab;
    public Transform modelTab;
    public Transform skyboxTab;
    public VirtualEditManager virtualEditManager;
    public GameObject listButton;
    public GameObject imageFlipbox;
    public GameObject modelFlipbox;
    public GameObject skyboxFlipbox;

    // Start is called before the first frame update
    public void Init()
    {
        loadCharacterList();
        loadObjectList();
        loadEffectList();
        loadSkyboxList();
        onTabClicked(imageTab);
    }

    public void loadCharacterList() {
        foreach(GameObject g in virtualEditManager.character2DList) {
            GameObject newItem = (GameObject)Instantiate(listButton, imageFlipbox.transform);
            newItem.name = g.name;
            newItem.GetComponent<ButtonHandler>().buttonTag = "character";
            newItem.GetComponentInChildren<Text>().text = g.name;
        }

        GameObject newI = (GameObject)Instantiate(listButton, imageFlipbox.transform);
        newI.name = "original360";
        newI.GetComponent<ButtonHandler>().buttonTag = "character";
        newI.GetComponentInChildren<Text>().text = "original360";
    }

    public void loadObjectList() {
        foreach(GameObject g in virtualEditManager.object2DList) {
            GameObject newItem = (GameObject)Instantiate(listButton, imageFlipbox.transform);
            newItem.name = g.name;
            newItem.GetComponent<ButtonHandler>().buttonTag = "object";
            newItem.GetComponentInChildren<Text>().text = g.name;
        }
    }

    public void loadEffectList() {
        foreach(GameObject g in virtualEditManager.effectList) {
            GameObject newItem = (GameObject)Instantiate(listButton, modelFlipbox.transform);
            newItem.name = g.name;
            newItem.GetComponent<ButtonHandler>().buttonTag = "effect";
            newItem.GetComponentInChildren<Text>().text = g.name;
        }
    }

    public void loadSkyboxList() {
        foreach(Material mat in virtualEditManager.skyboxList) {
            GameObject newItem = (GameObject)Instantiate(listButton, skyboxFlipbox.transform);
            newItem.name = mat.name;
            newItem.GetComponent<ButtonHandler>().buttonTag = "skybox";
            newItem.GetComponentInChildren<Text>().text = mat.name;
        }

        GameObject newI = (GameObject)Instantiate(listButton, skyboxFlipbox.transform);
        newI.name = "original360";
        newI.GetComponent<ButtonHandler>().buttonTag = "skybox";
        newI.GetComponentInChildren<Text>().text = "original360";
    }

    public void onTabClicked (Transform t) {
        imageTab.Find("Ring").gameObject.SetActive(false);
        modelTab.Find("Ring").gameObject.SetActive(false);
        skyboxTab.Find("Ring").gameObject.SetActive(false);
        imageFlipbox.SetActive(false);
        modelFlipbox.SetActive(false);
        skyboxFlipbox.SetActive(false);
        switch (t.name) {
            case "2D Image Tab":
                imageTab.Find("Ring").gameObject.SetActive(true);
                currentTab = 0;
                imageFlipbox.SetActive(true);
                break;
            case "3D Model Tab":
                modelTab.Find("Ring").gameObject.SetActive(true);
                currentTab = 1;
                modelFlipbox.SetActive(true);
                break;
            case "Skybox Tab":
                skyboxTab.Find("Ring").gameObject.SetActive(true);
                currentTab = 2;
                skyboxFlipbox.SetActive(true);
                break;
            default:
                imageTab.Find("Ring").gameObject.SetActive(true);
                currentTab = 0;
                imageFlipbox.SetActive(true);
                break;
        }
    }
}