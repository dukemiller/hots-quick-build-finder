﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace hots_quick_build_finder.Xaml
{

    // https://github.com/Nimgoble/WPFTextBoxAutoComplete/
    public static class AutoCompleteBehavior
    {
        public static readonly TextChangedEventHandler onTextChanged = OnTextChanged;

        private static readonly KeyEventHandler OnKeyDown = OnPreviewKeyDown;

        private static bool _isSubscribed;

        /// <summary>
        ///     The collection to search for matches from.
        /// </summary>
        public static readonly DependencyProperty AutoCompleteItemsSource =
            DependencyProperty.RegisterAttached
            (
                "AutoCompleteItemsSource",
                typeof(IEnumerable<string>),
                typeof(AutoCompleteBehavior),
                new UIPropertyMetadata(null, OnAutoCompleteItemsSource)
            );

        /// <summary>
        ///     Whether or not to ignore case when searching for matches.
        /// </summary>
        public static readonly DependencyProperty AutoCompleteStringComparison =
            DependencyProperty.RegisterAttached
            (
                "AutoCompleteStringComparison",
                typeof(StringComparison),
                typeof(AutoCompleteBehavior),
                new UIPropertyMetadata(StringComparison.Ordinal)
            );

        /// <summary>
        ///     Used for moving the caret to the end of the suggested auto-completion text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Tab)
                return;

            var tb = e.OriginalSource as TextBox;

            //If we pressed enter and if the selected text goes all the way to the end, move our caret position to the end
            if (tb?.SelectionLength > 0 && tb.SelectionStart + tb.SelectionLength == tb.Text.Length)
            {
                tb.SelectionStart = tb.CaretIndex = tb.Text.Length;
                tb.SelectionLength = 0;
            }
        }

        /// <summary>
        ///     Search for auto-completion suggestions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if
            (
                (from change in e.Changes where change.RemovedLength > 0 select change).Any() &&
                (from change in e.Changes where change.AddedLength > 0 select change).Any() == false
            )
                return;

            var tb = e.OriginalSource as TextBox;
            if (sender == null)
                return;

            var values = GetAutoCompleteItemsSource(tb);
            //No reason to search if we don't have any values.
            if (values == null)
                return;

            //No reason to search if there's nothing there.
            if (string.IsNullOrEmpty(tb?.Text))
                return;

            var textLength = tb.Text.Length;

            var comparer = GetAutoCompleteStringComparison(tb);
            //Do search and changes here.
            var match = values
                .Where(subvalue => subvalue.Length >= textLength)
                .FirstOrDefault(value => value.Substring(0, textLength).Equals(tb.Text, comparer));

            //Nothing.  Leave 'em alone
            if (string.IsNullOrEmpty(match))
                return;

            tb.TextChanged -= onTextChanged;
            tb.Text = match;
            tb.CaretIndex = textLength;
            tb.SelectionStart = textLength;
            tb.SelectionLength = match.Length - textLength;
            tb.TextChanged += onTextChanged;
        }

        #region Items Source

        public static IEnumerable<string> GetAutoCompleteItemsSource(DependencyObject obj)
        {
            var objRtn = obj.GetValue(AutoCompleteItemsSource);
            return objRtn as IEnumerable<string>;
        }

        public static void SetAutoCompleteItemsSource(DependencyObject obj, IEnumerable<string> value)
        {
            obj.SetValue(AutoCompleteItemsSource, value);
        }

        private static void OnAutoCompleteItemsSource(object sender, DependencyPropertyChangedEventArgs e)
        {
            var tb = sender as TextBox;
            if (sender == null)
                return;

            //If we're being removed, remove the callbacks
            if (e.NewValue == null && _isSubscribed)
            {
                tb.TextChanged -= onTextChanged;
                tb.PreviewKeyDown -= OnKeyDown;
                _isSubscribed = false;
            }
            else if (!_isSubscribed)
            {
                //New source.  Add the callbacks
                tb.TextChanged += onTextChanged;
                tb.PreviewKeyDown += OnKeyDown;
                _isSubscribed = true;
            }
        }

        #endregion

        #region String Comparison

        public static StringComparison GetAutoCompleteStringComparison(DependencyObject obj)
        {
            return (StringComparison) obj.GetValue(AutoCompleteStringComparison);
        }

        public static void SetAutoCompleteStringComparison(DependencyObject obj, StringComparison value)
        {
            obj.SetValue(AutoCompleteStringComparison, value);
        }

        #endregion
    }
}