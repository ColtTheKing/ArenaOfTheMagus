using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementButton : MenuButton
{
    private ElementSelectMenu elementSelectMenu;
    private SpellHandler.SpellElement element;
    private bool left;

    public override void Press()
    {
        elementSelectMenu.SetElement(element, left);
    }

    public void Init(int buttonId, SpellHandler.SpellElement element, bool left, ElementSelectMenu elementSelectMenu)
    {
        this.elementSelectMenu = elementSelectMenu;
        this.buttonId = buttonId;
        this.element = element;
        this.left = left;
    }
}
