using System.Numerics;
using Robust.Client.UserInterface.Controls;
using static Robust.Client.UserInterface.Controls.BoxContainer;

namespace Content.Goobstation.Client.UIKit.UserActions.Controls;

[Virtual]
[PublicAPI]
public class IconButton : Button
{
    public readonly TextureRect Icon;
    public new readonly RichTextLabel Label;
    //public readonly PanelContainer HighlightRect;

    public IconButton(string name)
    {
        MinSize = new Vector2(0, 24);
        Margin = new Thickness(1);
        HorizontalAlignment = HAlignment.Left;

        var mainContainer = new BoxContainer
        {
            Orientation = LayoutOrientation.Horizontal,
            //HorizontalExpand = true,
            MinSize = new Vector2(0, 24),
            Margin = new Thickness(1)
        };
        AddChild(mainContainer);

        Icon = new TextureRect
        {
            HorizontalExpand = true,
            VerticalExpand = true,
            HorizontalAlignment = HAlignment.Left,
            VerticalAlignment = VAlignment.Center,
            Stretch = TextureRect.StretchMode.Scale,
            Margin = new Thickness(0, 0, 5, 0),
            TextureScale = new Vector2(1, 1),
            MinSize = new Vector2(24, 24),
            MaxSize = new Vector2(24, 24),
            Visible = true
        };
        mainContainer.AddChild(Icon);

        Label = new RichTextLabel
        {
            HorizontalExpand = true,
            VerticalExpand = true,
            HorizontalAlignment = HAlignment.Left,
            VerticalAlignment = VAlignment.Center,
            Margin = new Thickness(1),
            Text = name,
            Visible = true
        };
        mainContainer.AddChild(Label);
    }
}
