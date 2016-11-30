using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class PropertyCard : MonoBehaviour {

    public Image TitleImg;
    public Text Place;
    public Text Cost;
    public Text Rent;
    public Text BuyOrSell;
    public GameObject Card;

	// Use this for initialization
	void Start () {
        Hide();
	}
    public void Hide()
    {
        Card.transform.DOScale(0.0f, 0.5f);
    }
    public void Show(Props prop)
    {
        TitleImg.GetComponent<Image>().color = Color.black;
        Place.text = prop.Name();
        Cost.text = "Cost   $ " + prop.Cost();
        Rent.text = "Rent   $ " + prop.Rent();
        BuyOrSell.text = "Buy";
        Card.transform.DOScale(1.0f, 0.5f);
    }
}
