using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public BallMovement PlayerRef;
    public Slider boostUI;
    public GameObject boostSlider;
    public Slider dodgeUI;
    public GameObject dodgeSlider;

    void Awake()
    {
        PlayerRef = GameObject.Find("PlayerBall").GetComponent<BallMovement>();
        hideBoostUI();
        hideDodgeUI();
    }

    void FixedUpdate()
    {
        boostUI.value = PlayerRef.boostLv;
        dodgeUI.value = PlayerRef.dodgeTimer;
    }

    public void showBoostUI()
    {
        boostSlider.SetActive(true);
    }
    public void hideBoostUI()
    {
        boostSlider.SetActive(false);
    }
    public void showDodgeUI()
    {
        dodgeSlider.SetActive(true);
    }
    public void hideDodgeUI()
    {
        dodgeSlider.SetActive(false);
    }
}
