using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonIconColors : MonoBehaviour
{
    [SerializeField] RawImage SymbolImage;
    [SerializeField] RawImage BGImage;
    [SerializeField] GameObject ThisBtn;
    bool selected = false;

    public void Enter(){
        if(ThisBtn.GetComponent<Button>().interactable){
            if(!selected){
                SymbolImage.color = OBJConverter.HoverHighlight;
                BGImage.color = OBJConverter.HoverBack;
            }
        }
    }

    public void Exit(){
        if(ThisBtn.GetComponent<Button>().interactable){
            if(!selected){
                SymbolImage.color = OBJConverter.ActiveHighlight;
                BGImage.color = OBJConverter.ActiveBack;
            }
        }
    }

    public void Up(){
        if(ThisBtn.GetComponent<Button>().interactable){
            SymbolImage.color = OBJConverter.ActiveHighlight;
            BGImage.color = OBJConverter.ActiveBack;
        }
    }

    public void Down(){
        if(ThisBtn.GetComponent<Button>().interactable){
            SymbolImage.color = OBJConverter.SelectedHighlight;
            BGImage.color = OBJConverter.SelectedBack;
        }
    }

    public void Select(){
        OBJConverter.LastBtn.GetComponent<ButtonIconColors>().Deselect();
        OBJConverter.LastBtn = ThisBtn;
        selected = true;
        SymbolImage.color = OBJConverter.SelectedHighlight;
        BGImage.color = OBJConverter.SelectedBack;
    }

    public void Deselect(){
        selected = false;
        SymbolImage.color = OBJConverter.ActiveHighlight;
        BGImage.color = OBJConverter.ActiveBack;
    }

}


