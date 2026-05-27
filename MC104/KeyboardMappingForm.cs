using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MC104
{
    public sealed class KeyboardMappingForm : Form
    {
        public KeyboardMappingForm(IList<KeyboardBinding> bindings)
        {
            Text = "keyboard mapping";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(360, 280);

            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                MultiSelect = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoGenerateColumns = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Action",
                HeaderText = "Action",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 65
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Key",
                HeaderText = "Key",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 35
            });

            grid.DataSource = (bindings ?? new List<KeyboardBinding>()).ToList();

            var closeButton = new Button
            {
                Text = "Close",
                Dock = DockStyle.Bottom,
                Height = 36
            };

            closeButton.Click += (sender, e) => Close();

            Controls.Add(grid);
            Controls.Add(closeButton);

            AcceptButton = closeButton;
        }
    }
}