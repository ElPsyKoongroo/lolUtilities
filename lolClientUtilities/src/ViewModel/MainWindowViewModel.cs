using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LeagueUtilities;

namespace lolClientUtilities.ViewModel;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private League _client = new();

    public string Text
    {
        get => text;
        set
        {
            text = value;
            OnPropertyChange();
        }
    }

    private string text = "b";
    
    public event PropertyChangedEventHandler? PropertyChanged;
 
    public void OnPropertyChange([CallerMemberName]string name = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}