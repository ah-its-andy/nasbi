using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace NasBI.Components
{
    public partial class CardComponent : UserControl
    {
        public CardComponent()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
