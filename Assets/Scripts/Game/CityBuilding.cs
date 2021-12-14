using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuilding : MonoBehaviour
{

    [SerializeField] private Building building;

    [SerializeField] private BoxCollider2D area;

    [SerializeField] private SpriteRenderer sprite;

    [SerializeField] public string buildingName;

    public void EnableBuilding()
    {
        sprite.enabled = true;
    }

    private void OnMouseDown()
    {
        if (!sprite.enabled)
        {
            PlayerCity.Instance.buyBtn.gameObject.SetActive(true);
            PlayerCity.Instance.buyBtnText.text = "BUILD " + buildingName.ToUpper();
            PlayerCity.Instance.selected = this;
        }
    }

    private void OnMouseExit()
    {
        //if (!sprite.enabled)
        //{
        //    PlayerCity.Instance.buyBtn.gameObject.SetActive(false);
        //}
    }

}
