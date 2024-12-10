using System.ComponentModel;

namespace WindowsInput.Tests.UnicodeText;

public partial class UnicodeTestForm : Form
{
    public UnicodeTestForm()
    {
        InitializeComponent();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Expected
    {
        get { return ExpectedTextBox.Text; }
        set { ExpectedTextBox.Text = value; }
    }

    public string Recieved { get { return RecievedTextBox.Text; } }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        RecievedTextBox.Focus();
    }
}
