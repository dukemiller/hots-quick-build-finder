using GalaSoft.MvvmLight;

namespace hots_quick_build_finder.Models
{
    public class Square: ObservableObject
    {
        private bool _selected;

        public bool Selected
        {
            get { return _selected; }
            set { Set(() => Selected, ref _selected, value); }
        }
    }
}
