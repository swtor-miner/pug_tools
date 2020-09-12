﻿namespace SlimDXNet {
    using System.Windows.Forms;
    public delegate bool MyWndProc(ref Message m);
    public class D3DForm : Form {
        public MyWndProc MyWndProc;
        protected override void WndProc(ref Message m) {
            if (MyWndProc != null) {
                if (MyWndProc(ref m)) return;
            }
            base.WndProc(ref m);
        }
    }
}