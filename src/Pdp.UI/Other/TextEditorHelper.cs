using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Windows;

namespace Pdp.UI.Other
{
    public static class TextEditorHelper
    {
        #region BoundText
        public static readonly DependencyProperty BoundTextProperty =
            DependencyProperty.RegisterAttached(
                "BoundText",
                typeof(string),
                typeof(TextEditorHelper),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBoundTextChanged));

        public static void SetBoundText(DependencyObject obj, string value)
        {
            obj.SetValue(BoundTextProperty, value);
        }

        public static string GetBoundText(DependencyObject obj)
        {
            return (string)obj.GetValue(BoundTextProperty);
        }

        private static void OnBoundTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextEditor editor) return;

            editor.TextChanged -= Editor_TextChanged;
            var newValue = e.NewValue as string;
            if (editor.Text != newValue)
            {
                editor.Text = newValue;
            }
            editor.TextChanged += Editor_TextChanged;
        }

        private static void Editor_TextChanged(object? sender, EventArgs e)
        {
            if (sender is TextEditor editor)
            {
                SetBoundText(editor, editor.Text);
            }
        }
        #endregion

        #region BoundHighlight
        public static readonly DependencyProperty BoundHighlightProperty =
            DependencyProperty.RegisterAttached(
                "BoundHighlight",
                typeof(IHighlightingDefinition),
                typeof(TextEditorHelper),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBoundHighlightChanged));
        public static void SetBoundHighlight(DependencyObject obj, IHighlightingDefinition value)
        {
            obj.SetValue(BoundHighlightProperty, value);
        }

        public static IHighlightingDefinition GetBoundHighlight(DependencyObject obj)
        {
            return (IHighlightingDefinition)obj.GetValue(BoundHighlightProperty);
        }

        private static void OnBoundHighlightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextEditor editor) return;

            editor.TextChanged -= Editor_HighlightChanged;
            var newValue = e.NewValue as IHighlightingDefinition;
            if (editor.SyntaxHighlighting != newValue)
            {
                editor.SyntaxHighlighting = newValue;
            }
            editor.TextChanged += Editor_HighlightChanged;
        }
        private static void Editor_HighlightChanged(object? sender, EventArgs e)
        {
            if (sender is TextEditor editor)
            {
                SetBoundHighlight(editor, editor.SyntaxHighlighting);
            }
        }
        #endregion
    }
}
