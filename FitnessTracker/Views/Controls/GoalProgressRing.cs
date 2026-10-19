using Microsoft.Maui.Graphics;

namespace FitnessTracker.Views.Controls
{
    /// <summary>
    /// Animated circular progress ring that displays a goal title, percentage, and completion state.
    /// </summary>
    /// <remarks>Draws a background track, a progress arc, and center text (percentage or check mark). Exposes
    /// bindable properties: Progress (0.0–1.0), GoalTitle, and IsComplete. Changes to Progress are smoothly animated
    /// and the control updates SemanticProperties.Description for accessibility. The control requests an 88×88 size by
    /// default and uses application color resources ('GoalProgressBlue', 'GoalProgressGreen', 'GoalProgressTrack',
    /// 'AppText') with fallbacks when keys are not present.</remarks>
    public sealed class GoalProgressRing : GraphicsView
    {
        private double animatedProgress;

        public static readonly BindableProperty ProgressProperty =
            BindableProperty.Create(
                nameof(Progress),
                typeof(double),
                typeof(GoalProgressRing),
                0d,
                propertyChanged: OnProgressChanged);

        public static readonly BindableProperty GoalTitleProperty =
            BindableProperty.Create(
                nameof(GoalTitle),
                typeof(string),
                typeof(GoalProgressRing),
                string.Empty,
                propertyChanged: OnAccessibilityTextChanged);

        public static readonly BindableProperty IsCompleteProperty =
            BindableProperty.Create(
                nameof(IsComplete),
                typeof(bool),
                typeof(GoalProgressRing),
                false,
                propertyChanged: OnAccessibilityTextChanged);

        public double Progress
        {
            get => (double)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }

        public bool IsComplete
        {
            get => (bool)GetValue(IsCompleteProperty);
            set => SetValue(IsCompleteProperty, value);
        }

        public string GoalTitle
        {
            get => (string)GetValue(GoalTitleProperty);
            set => SetValue(GoalTitleProperty, value);
        }

        public GoalProgressRing()
        {
            Drawable = new RingDrawable(this);
            WidthRequest = 88;
            HeightRequest = 88;
            UpdateAccessibilityText();
        }

        
        // when the Progress property changes, animate the ring to the new value and update the accessibility text
        private static void OnProgressChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var ring = (GoalProgressRing)bindable;
            var target = Math.Clamp((double)newValue, 0, 1);

            new Animation(
                value =>
                {
                    ring.animatedProgress = value;
                    ring.Invalidate();
                },
                ring.animatedProgress, target).Commit(ring, "ProgressRing", length: 250, easing: Easing.CubicInOut);

            ring.UpdateAccessibilityText();
        }

        // when the GoalTitle or IsComplete properties change, update the accessibility text
        private static void OnAccessibilityTextChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var ring = (GoalProgressRing)bindable;
            ring.UpdateAccessibilityText();
            ring.Invalidate();
        }

        // update the accessibility text based on the current progress, goal title, and completion status
        private void UpdateAccessibilityText()
        {
            var percent = (int)Math.Round(Math.Clamp(Progress, 0, 1) * 100);

            var description = IsComplete ? $"Goal '{GoalTitle}' completed." : $"Goal '{GoalTitle}' progress: {percent}%.";

            SemanticProperties.SetDescription(this, description);
        }

        /// <summary>
        /// A custom drawable for the goal progress ring
        /// draws the background track, the progress arc, and the center text based on the current progress and completion status.
        /// </summary>
        private sealed class RingDrawable(GoalProgressRing owner) : IDrawable
        {
            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                var size = Math.Min(dirtyRect.Width, dirtyRect.Height);
                var inset = 7f;
                var x = dirtyRect.Center.X - size / 2 + inset;
                var y = dirtyRect.Center.Y - size / 2 + inset;
                var diameter = size - inset * 2;

                var progress = (float)Math.Clamp(owner.animatedProgress, 0, 1);
                var startAngle = 90f;
                var endAngle = startAngle + 360f * progress;

                var blue = GetColor("GoalProgressBlue", Color.FromArgb("#3B82F6"));
                var green = GetColor("GoalProgressGreen", Color.FromArgb("#22C55E"));
                var track = GetColor("GoalProgressTrack", Color.FromArgb("#334155"));
                var progressColor = Blend(blue, green, progress);

                canvas.StrokeSize = 8;
                canvas.StrokeColor = track;
                canvas.DrawArc(x, y, diameter, diameter, 0, 360, true, false);

                if (progress > 0)
                {
                    canvas.StrokeColor = owner.IsComplete ? green : progressColor;
                    canvas.DrawArc(x, y, diameter, diameter, startAngle, endAngle, false, false);
                }

                var centerText = owner.IsComplete
                    ? "✓"
                    : $"{(int)Math.Round(progress * 100)}%";

                canvas.FontColor = owner.IsComplete
                    ? green
                    : GetColor("AppText", Color.FromArgb("#D6E9FF"));
                canvas.FontSize = owner.IsComplete ? 27 : 17;
                canvas.DrawString(
                    centerText,
                    dirtyRect,
                    HorizontalAlignment.Center,
                    VerticalAlignment.Center);
            }
        }

        /// Get a color from the application resources, or return a fallback color if the key is not found or the value is not a Color.
        private static Color GetColor(string key, Color fallback)
        {
            return Application.Current?.Resources.TryGetValue(key, out var value) == true && value is Color color ? color : fallback;
        }

        /// Blend two colors based on the specified amount (0 to 1).
        private static Color Blend(Color start, Color end, float amount)
        {
            amount = Math.Clamp(amount, 0, 1);

            return Color.FromRgb(
                start.Red + (end.Red - start.Red) * amount,
                start.Green + (end.Green - start.Green) * amount,
                start.Blue + (end.Blue - start.Blue) * amount
            );
        }
    }

}
