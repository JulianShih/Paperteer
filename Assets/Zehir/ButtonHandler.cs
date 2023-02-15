using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.Extras;

public class ButtonHandler : MonoBehaviour
{
    public ControllerInput controllerInput;
    public Outline outline;
    public string buttonTag;

    void Start()
    {
        outline = GetComponent<Outline>();
        controllerInput = GameObject.Find("Controller Input").GetComponent<ControllerInput>();
        // GetComponent<Button>().onClick.AddListener(delegate(onButtonClick(e)));
    }

    public void onButtonEnter() {
        outline.enabled = true;
    }

    public void onButtonLeave() {
        outline.enabled = false;
    }

    public void onButtonClick(PointerEventArgs e) {
        switch (buttonTag)
        {
            case "character":
                controllerInput.onCharButtonClick(this.name);
                break;
            case "object":
                controllerInput.onObjButtonClick(this.name, e);
                break;
            case "effect":
                controllerInput.virtualEditManager.addEffect(this.name);
                break;
            case "skybox":
                controllerInput.virtualEditManager.replaceSkybox(this.name);
                break;
            default:
                break;
        }
        
    }
}