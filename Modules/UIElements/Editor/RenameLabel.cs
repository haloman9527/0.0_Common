#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 半只龙虾人
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */
#endregion
#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;

public class RenameLabel : BindableElement, INotifyValueChanged<string>
{
    public new class UxmlFactory : UxmlFactory<RenameLabel, UxmlTraits> { }

    public Label label;
    public TextField labelInput;

    public RenameLabel()
    {
        label = new Label("Label");
        Add(label);
        labelInput = new TextField() { value = "Label" };
        Add(labelInput);

        label.StretchToParentSize();
        label.style.unityTextAlign = TextAnchor.MiddleLeft;
        labelInput.visible = false;
        labelInput.RegisterValueChangedCallback(evt =>
        {
            label.text = evt.newValue;
        });
        labelInput.StretchToParentSize();
        labelInput.style.position = Position.Absolute;


        label.RegisterCallback<MouseDownEvent>(evt =>
        {
            if (evt.clickCount == 2)
            {
                label.visible = false;
                labelInput.visible = true;
            }
        });
        labelInput.RegisterCallback<FocusOutEvent>(evt =>
        {
            labelInput.visible = false;
            label.visible = true;
        });
    }

    public RenameLabel(string _label) : base()
    {
        labelInput.value = _label;
    }

    public string value { get => labelInput.value; set => labelInput.value = value; }

    public void SetValueWithoutNotify(string _newValue)
    {
        labelInput.SetValueWithoutNotify(_newValue);
    }
}
#endif