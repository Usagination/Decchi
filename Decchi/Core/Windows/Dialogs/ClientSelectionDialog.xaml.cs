﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Decchi.ParsingModule;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace Decchi.Core.Windows.Dialogs
{
    public partial class ClientSelectionDialog : BaseMetroDialog
    {
        public SongInfo SongInfo { get; private set; }

        private	List<SongInfo> m_songinfo;
        private TaskCompletionSource<object> m_tcs;
        private CancellationTokenRegistration m_cancel;

        internal ClientSelectionDialog(MetroWindow parentWindow)
            : this(parentWindow, null)
        {
        }
        internal ClientSelectionDialog(MetroWindow parentWindow, MetroDialogSettings settings)
            : base(parentWindow, settings)
        {
            InitializeComponent();

            this.m_songinfo = new List<SongInfo>();
            for (int i = 0; i < SongInfo.SongInfos.Length; ++i)
                if (SongInfo.SongInfos[i].Loaded)
                    this.m_songinfo.Add(SongInfo.SongInfos[i]);

            this.ctlList.ItemsSource = this.m_songinfo;

            this.m_tcs = new TaskCompletionSource<object>();
            this.m_cancel = DialogSettings.CancellationToken.Register(() => this.m_tcs.TrySetResult(null));
        }

        protected override void OnLoaded()
        {
            switch (this.DialogSettings.ColorScheme)
            {
                case MetroDialogColorScheme.Accented:
                    ctlOK.Style = this.FindResource("AccentedDialogHighlightedSquareButton") as Style;
                    break;
            }
        }
        protected override void OnClose()
        {
            this.m_cancel.Dispose();
        }

        public override Task<object> _WaitForButtonPressAsync()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                this.Focus();
            }));
            return this.m_tcs.Task;
        }

        private void BaseMetroDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.m_tcs.TrySetResult(null);
        }

        private void ctlList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ctlOK.IsEnabled = true;
        }

        private void ctlOK_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                this.m_tcs.TrySetResult(ctlList.SelectedItem);
        }
        private void ctlCancel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                this.m_tcs.TrySetResult(null);
        }

        private void ctlOK_Click(object sender, RoutedEventArgs e)
        {
            this.m_tcs.TrySetResult(ctlList.SelectedItem);
            e.Handled = true;
        }
        private void ctlCancel_Click(object sender, RoutedEventArgs e)
        {
            this.m_tcs.TrySetResult(null);
            e.Handled = true;
        }
    }
}