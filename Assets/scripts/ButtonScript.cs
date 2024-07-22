using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ButtonScript : MonoBehaviour
{
    public Button button;
    public Text buttonText;
    bool allowedMove = false;
    string buttonName="";
    int mode = 0;

    private GameManager gameController;

    public void SetSpace()
    {
        mode= gameController.GetMode();
        if(mode == 3)
        {
            return;
        }else if(mode == 2 && gameController.GetPlayerSide() == "O")
        {
            return;
        }

        buttonName = button.name;
        string[] strlist = buttonName.Split(' ');
        allowedMove = gameController.checkIfAllowd(int.Parse(strlist[0]), int.Parse(strlist[1]));
        if (allowedMove)
        {
            buttonText.text = gameController.GetPlayerSide();
            button.interactable = false;

            if (gameController.GetPlayerSide() == "X")
                button.tag = "PlayerX";
            else
                button.tag = "PlayerO";

            gameController.EndTurn(int.Parse(strlist[0]), int.Parse(strlist[1]));
        }else
        {
            button.interactable = true;
            Debug.Log("move not allowed");
        }
    }

    public void SetGameControllerRefrence(GameManager controller)
    {
        gameController = controller;
    }
    public void DisableButton()
    {
        button.interactable = false;
    }

    public void EnableButton()
    {
        button.interactable = true;
    }
}
