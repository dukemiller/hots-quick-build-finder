﻿using System.Windows.Input;

namespace hots_quick_build_finder.Xaml
{
    // https://social.msdn.microsoft.com/Forums/vstudio/en-US/b39b5d98-d039-4e83-8c65-ca434786d6af/mouse-wheel-input-binding?forum=wpf
    public class MouseWheelGesture : MouseGesture
    {
        public WheelDirection Direction { get; set; }

        public static MouseWheelGesture Up => new MouseWheelGesture { Direction = WheelDirection.Up };

        public static MouseWheelGesture Down => new MouseWheelGesture { Direction = WheelDirection.Down };

        public static MouseWheelGesture CtrlUp => new MouseWheelGesture(ModifierKeys.Control) { Direction = WheelDirection.Up };

        public static MouseWheelGesture CtrlDown => new MouseWheelGesture(ModifierKeys.Control) { Direction = WheelDirection.Down };

        public MouseWheelGesture() : base(MouseAction.WheelClick)
        {}

        public MouseWheelGesture(ModifierKeys modifiers) : base(MouseAction.WheelClick, modifiers)
        {}

        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            if (!base.Matches(targetElement, inputEventArgs)) return false;
            if (!(inputEventArgs is MouseWheelEventArgs)) return false;
            var args = (MouseWheelEventArgs)inputEventArgs;
            switch (Direction)
            {
                case WheelDirection.None:
                    return args.Delta == 0;
                case WheelDirection.Up:
                    return args.Delta > 0;
                case WheelDirection.Down:
                    return args.Delta < 0;
                default:
                    return false;
            }
        }

        public enum WheelDirection
        {
            None,
            Up,
            Down,
        }
    }
}
