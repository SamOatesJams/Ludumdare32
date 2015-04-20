using InControl;

public class CharacterControlActions : PlayerActionSet
{
    public PlayerAction Forward;
    public PlayerAction Backward;
    public PlayerAction Left;
    public PlayerAction Right;

    public PlayerOneAxisAction Vertical;
    public PlayerOneAxisAction Horizontal;

    public PlayerAction LookUp;
    public PlayerAction LookDown;
    public PlayerAction LookLeft;
    public PlayerAction LookRight;

    public PlayerOneAxisAction LookVertical;
    public PlayerOneAxisAction LookHorizontal;

    public PlayerAction Pickup;
    public PlayerAction Interact;
    public PlayerAction Fire;

    public CharacterControlActions()
    {
        this.Forward = CreatePlayerAction("Move Forward");
        this.Backward = CreatePlayerAction("Move Backward");
        this.Left = CreatePlayerAction("Move Left");
        this.Right = CreatePlayerAction("Move Right");

        this.Vertical = CreateOneAxisPlayerAction(Backward, Forward);
        this.Horizontal = CreateOneAxisPlayerAction(Left, Right);

        this.LookUp = CreatePlayerAction("Look Up");
        this.LookDown = CreatePlayerAction("Look Down");
        this.LookLeft = CreatePlayerAction("Look Left");
        this.LookRight = CreatePlayerAction("Look Right");

        this.LookVertical = CreateOneAxisPlayerAction(LookDown, LookUp);
        this.LookHorizontal = CreateOneAxisPlayerAction(LookLeft, LookRight);

        this.Pickup = CreatePlayerAction("Pickup Item");
        this.Interact = CreatePlayerAction("Interact");
        this.Fire = CreatePlayerAction("Fire Weapon");
    }

    public void Setup()
    {
        // Keyboard
        this.Forward.AddDefaultBinding(Key.W);
        this.Backward.AddDefaultBinding(Key.S);
        this.Left.AddDefaultBinding(Key.A);
        this.Right.AddDefaultBinding(Key.D);

        this.LookUp.AddDefaultBinding(Mouse.PositiveY, 0.025f);
        this.LookDown.AddDefaultBinding(Mouse.NegativeY, 0.025f);
        this.LookLeft.AddDefaultBinding(Mouse.NegativeX, 0.025f);
        this.LookRight.AddDefaultBinding(Mouse.PositiveX, 0.025f);

        this.Pickup.AddDefaultBinding(Key.Space);
        this.Interact.AddDefaultBinding(Key.E);
        this.Fire.AddDefaultBinding(Mouse.LeftButton);

        // Controller
        this.Forward.AddDefaultBinding(InputControlType.LeftStickUp);
        this.Backward.AddDefaultBinding(InputControlType.LeftStickDown);
        this.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
        this.Right.AddDefaultBinding(InputControlType.LeftStickRight);

        this.LookUp.AddDefaultBinding(InputControlType.RightStickUp);
        this.LookDown.AddDefaultBinding(InputControlType.RightStickDown);
        this.LookLeft.AddDefaultBinding(InputControlType.RightStickLeft);
        this.LookRight.AddDefaultBinding(InputControlType.RightStickRight);

        this.Pickup.AddDefaultBinding(InputControlType.Action1);
        this.Interact.AddDefaultBinding(InputControlType.Action3);
        this.Fire.AddDefaultBinding(InputControlType.RightTrigger);
    }
}
