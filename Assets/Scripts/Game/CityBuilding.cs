using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuilding : MonoBehaviour
{

    [SerializeField] public Building building;

    [SerializeField] private BoxCollider2D area;

    [SerializeField] private SpriteRenderer sprite;

    [SerializeField] public string buildingName;

    [SerializeField] public string customInfo;

    public void EnableBuilding()
    {
        sprite.enabled = true;
    }

    public void DisableBuilding()
    {
        sprite.enabled = false;
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.playerBase.client_id != GameManager.Instance.currentBase.client_id)
            return;

        if (!sprite.enabled)
        {
            PlayerCity.Instance.buyBtn.gameObject.SetActive(true);
            PlayerCity.Instance.delBtn.gameObject.SetActive(false);
            PlayerCity.Instance.editBtn.gameObject.SetActive(false);
            PlayerCity.Instance.addBtn.gameObject.SetActive(false);
            PlayerCity.Instance.buyBtnText.text = "BUILD " + buildingName.ToUpper();
            PlayerCity.Instance.selected = this;
            PlayerCity.Instance.edit.SetActive(true);
            PlayerCity.Instance.editText.text = "Name...";
        }
        else
        {
            PlayerCity.Instance.delBtn.gameObject.SetActive(true);
            PlayerCity.Instance.editBtn.gameObject.SetActive(true);
            PlayerCity.Instance.buyBtn.gameObject.SetActive(false);
            PlayerCity.Instance.delBtnText.text = "DESTROY " + buildingName.ToUpper();
            PlayerCity.Instance.selected = this;
            PlayerCity.Instance.edit.SetActive(true);
            PlayerCity.Instance.editText.text = building.name == null? ":)" : building.name;

            if (customInfo != "")
            {
                PlayerCity.Instance.addBtn.gameObject.SetActive(true);
                PlayerCity.Instance.deletePeopleBtn.gameObject.SetActive(true);
                string type = "CITIZEN";
                switch (building.type)
                {
                    case "Barracks":
                        type = "SOLDIERS";
                        break;
                    case "Blacksmith":
                        type = "WORKERS";
                        break;
                }
                PlayerCity.Instance.addBtnText.text = "ADD " + type;
                PlayerCity.Instance.deletePeopleBtnText.text = "DISCARD " + type;
            }
            else
            {
                PlayerCity.Instance.addBtn.gameObject.SetActive(false);
                PlayerCity.Instance.deletePeopleBtn.gameObject.SetActive(false);
            }
        }
    }

    private void OnMouseEnter()
    {
        if (sprite.enabled)
        {
            PlayerCity.Instance.info.gameObject.SetActive(true);
            PlayerCity.Instance.info.SetInfo(building.name, building.type, customInfo);
        }
    }

    private void OnMouseExit()
    {
        if (sprite.enabled)
            PlayerCity.Instance.info.gameObject.SetActive(false);
    }

}
