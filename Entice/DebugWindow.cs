using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Entice
{
    internal partial class DebugWindow : Form
    {
        private readonly ConsoleWriter _cw;

        public DebugWindow()
        {
            InitializeComponent();

            _cw = new ConsoleWriter(this);

            Console.SetOut(_cw);
        }

        private void WriteLine(string text, Color color)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => WriteLine(text, color)));
                return;
            }

            console.Items.Add(text).ForeColor = color;

            console.Items[console.Items.Count - 1].EnsureVisible();
        }

        public class ConsoleWriter : TextWriter
        {
            private readonly DebugWindow _debugWindow;

            public ConsoleWriter(DebugWindow debugWindow)
            {
                _debugWindow = debugWindow;
            }

            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }

            public override void Write(string value)
            {
                _debugWindow.WriteLine(value, Color.Black);

                base.Write(value);
            }

            public override void WriteLine(string value)
            {
                _debugWindow.WriteLine(value, Color.Black);

                base.WriteLine(value);
            }
        }
    }
}