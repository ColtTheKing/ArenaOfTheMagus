using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MenuScreen : MonoBehaviour
{
    // Start is called before the first frame update
    public virtual void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }

    public virtual void ActivateMenu()
    {
        gameObject.SetActive(true);
    }

    public virtual void SwapMenu(int menuId)
    {
        gameObject.SetActive(false);
    }

    public abstract void PressButton(int buttonId);
}
