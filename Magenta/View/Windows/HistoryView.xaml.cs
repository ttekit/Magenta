using System.Collections.Generic;
using System.Windows;
using Magenta.Core;
using Newtonsoft.Json.Linq;

namespace Magenta.View.Windows;

public partial class HistoryView : Window
{
    public HistoryView(JArray data)
    {
        InitializeComponent();
        var historyItems = new List<HistoryItem>();
        foreach (var token in data)
            historyItems.Add(new HistoryItem
                { Role = token["role"].ToString(), Message = token["content"].ToString() });

        MainHistoryListView.ItemsSource = historyItems;
    }
}