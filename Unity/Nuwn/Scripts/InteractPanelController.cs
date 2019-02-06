using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractPanelController : MonoBehaviour {

    public enum Types{
        Interact, ToHold, Pickup, OnHold
    }

    [Serializable]
	public struct CHOpt
    {
        public Sprite none;
        public Sprite InteractImage;
        public Sprite HoldImage;
        public Sprite PickupImage;
    }

    [Header("0. Interact 1.Tohold 2.pickup 3.holding")]
    public CHOpt CrosshairOptions;

    [SerializeField] private Image Panel = default;
    [SerializeField] private Image Crosshair = default;
    [SerializeField] private Image[] Icons = default;
    [SerializeField] private TextMeshProUGUI[] textFields = default;

    bool Visible { get; set; }
    public float FadeRate = 3f;

    private void Start()
    {
        Visible = false;
    }

    void Update()
    {
        fadePanel();
        fadeCrosshair();
        foreach (var t in textFields)
        {
            fadeText(t);
        }
        foreach (var i in Icons)
        {
            fadeIcon(i);
        }
        
    }

    void fadeText(TextMeshProUGUI textField)
    {
        Color curColorText = textField.color;
        float alphaDiff = Mathf.Abs(curColorText.a - ((Visible) ? 255 : 0));
        
        if (alphaDiff > 0.0001f)
        {
            curColorText.a = Mathf.Lerp(curColorText.a, ((Visible) ? 255 : 0), FadeRate * Time.deltaTime);
            textField.color = curColorText;
        }
    }
    void fadeIcon(Image icon)
    {
        Color curColor = icon.color;
        float alphaDiff = Mathf.Abs(curColor.a - ((Visible) ? 255 : 0));

        if (alphaDiff > 0.0001f)
        {
            curColor.a = Mathf.Lerp(curColor.a, ((Visible) ? 255 : 0), FadeRate * Time.deltaTime);
            icon.color = curColor;
        }
    }
    void fadeCrosshair()
    {
        Color curColor = Crosshair.color;
        float alphaDiff = Mathf.Abs(curColor.a - ((Visible) ? 255 : 0));
        if (alphaDiff > 0.0001f)
        {
            curColor.a = Mathf.Lerp(curColor.a, ((Visible) ? 255 : 0), FadeRate * Time.deltaTime);
            Crosshair.color = curColor;
        }
    }
    void fadePanel()
    {
        Color curColor = Panel.color;
        float alphaDiff = Mathf.Abs(curColor.a - ((Visible) ? 255 : 0));
        if (alphaDiff > 0.0001f)
        {
            curColor.a = Mathf.Lerp(curColor.a, ((Visible) ? 255 : 0), FadeRate * Time.deltaTime);
            Panel.color = curColor;
        }
    }


    public void Hide()
    {
        Visible = false;
    }

    public void ShowInfo(int type)
    {
        Visible = true;
        switch ((Types) type)
        {
            case Types.Interact:
                Crosshair.sprite = CrosshairOptions.InteractImage;
                break;
            case Types.ToHold:
                Crosshair.sprite = CrosshairOptions.HoldImage;
                break;
            case Types.Pickup:
                Crosshair.sprite = CrosshairOptions.PickupImage;
                break;
            case Types.OnHold:
                Crosshair.sprite = CrosshairOptions.none;
                break;
        }
            

    }
    
}
