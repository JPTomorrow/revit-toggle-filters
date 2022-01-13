using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using  Controls = System.Windows.Controls;

namespace JPMorrow.WPF.Extensions
{
    public static class DataGridExtension
    {
        public static void SelectItem(this Controls.DataGrid grid, object item)
        {
            var idx = grid.Items.IndexOf(item);
            if(idx == -1) return;

            grid.SelectedItem = item;
            grid.ScrollIntoView(item);
            var row = (Controls.DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(idx);
            row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }
    }
}